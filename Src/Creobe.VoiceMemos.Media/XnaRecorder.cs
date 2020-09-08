using Creobe.VoiceMemos.Core.Media;
using Creobe.VoiceMemos.Media.Lame;
using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;

namespace Creobe.VoiceMemos.Media
{
    public class XnaRecorder : IRecorder
    {
        #region Private Members

        private int _sampleRate;
        private int _channels;
        private int _bitRate;
        private EncodingFormat _format;
        private int _preset;

        private Microphone _microphone;
        private Stream _stream;
        private IAudioWriter _writer;

        private int _bytesCaptured;
        private int _size;
        private int _duration;
        private RecorderState _state;
        private bool _isCapturing;

        #endregion

        #region Constructors

        public XnaRecorder(Stream stream)
            : this(stream, EncodingFormat.Wave, 128) { }

        public XnaRecorder(Stream stream, EncodingFormat format, int preset)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _stream = stream;
            _sampleRate = 16000;
            _bitRate = 16;
            _channels = 1;
            _format = format;
            _preset = preset;

            _size = 0;
            _duration = 0;
            _state = RecorderState.Unknown;

            InitializeMicrophone();

            if (_format == EncodingFormat.Wave)
                _writer = new WaveWriter(_stream, _sampleRate, _bitRate, _channels);
            else if (_format == EncodingFormat.MP3)
                _writer = new Mp3Writer(_stream, _sampleRate, _bitRate, _channels, (LAMEPreset)_preset);

        }

        #endregion

        #region Private Methods

        private void InitializeMicrophone()
        {
            _microphone = Microphone.Default;
            _microphone.BufferDuration = TimeSpan.FromMilliseconds(100);
            _microphone.BufferReady += OnBufferReady;
        }

        private void ReleaseMicrophone()
        {
            if (_microphone.State == MicrophoneState.Started)
                _microphone.Stop();

            _microphone.BufferReady -= OnBufferReady;
            _microphone = null;
        }

        private void RaiseSampleReceived(float level)
        {
            if (SampleReceived != null)
                SampleReceived(this, new SampleReceivedEventArgs
                {
                    Level = level,
                    Duration = _duration
                });
        }

        private void OnBufferReady(object sender, EventArgs e)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[1024];

            while ((bytesRead = this._microphone.GetData(buffer, 0, buffer.Length)) > 0)
            {
                float peak = PcmHelper.GetPeak(buffer, bytesRead);

                if (_state == RecorderState.Recording && _isCapturing && _writer.CanWrite)
                {
                    if ((SkipSilence && peak > 0.04) || !SkipSilence)
                    {
                        _size += _writer.Write(buffer, 0, bytesRead);
                        _bytesCaptured += bytesRead;
                        _duration = (int)_microphone.GetSampleDuration(_bytesCaptured).TotalSeconds;
                    }
                }

                RaiseSampleReceived(peak);
            }
        }

        #endregion

        #region IRecorder Members

        public int SampleRate
        {
            get { return _sampleRate; }
        }

        public int Channels
        {
            get { return _channels; }
        }

        public int BitsRate
        {
            get { return _bitRate; }
        }

        public EncodingFormat EncodingFormat
        {
            get { return _format; }
        }

        public int EncodingPreset
        {
            get { return _preset; }
        }

        public int Size
        {
            get { return _size; }
        }

        public int Duration
        {
            get { return _duration; }
        }

        public RecorderState State
        {
            get { return _state; }
        }

        public bool IsCapturing
        {
            get { return _isCapturing; }
        }

        public bool SkipSilence { get; set; }

        public void Start()
        {
            if (_state != RecorderState.Unknown)
                throw new InvalidOperationException("A recording is already in progress.");

            _microphone.Start();
            _state = RecorderState.Recording;
            _isCapturing = true;
        }

        public void Stop()
        {
            if (_state == RecorderState.Stopped || _state == RecorderState.Unknown)
                throw new InvalidOperationException("No recording in progress.");

            ReleaseMicrophone();

            _state = RecorderState.Stopped;
            _isCapturing = false;
            _writer.Flush();

            _stream.Dispose();
        }

        public void Pause()
        {
            if (_state == RecorderState.Stopped || _state == RecorderState.Unknown)
                throw new InvalidOperationException("No recording in progress.");

            _state = RecorderState.Paused;
        }

        public void PauseCapture()
        {
            if (_isCapturing)
            {
                _microphone.Stop();
                _isCapturing = false;
            }
        }

        public void ResumeCapture()
        {
            if (!_isCapturing)
            {
                _microphone.Start();
                _isCapturing = true;
            }
        }

        public void Resume()
        {
            if (_state == RecorderState.Stopped || _state == RecorderState.Unknown)
                throw new InvalidOperationException("No recording in progress.");

            _state = RecorderState.Recording;
        }

        public event EventHandler<SampleReceivedEventArgs> SampleReceived;

        #endregion
    }
}
