using Creobe.VoiceMemos.Core.Media;
using Creobe.VoiceMemos.Media.Lame;
using Creobe.VoiceMemos.Media.Wasapi;
using System;
using System.IO;
using System.Windows.Threading;

namespace Creobe.VoiceMemos.Media
{
    public class WasapiRecorder : IRecorder
    {
        #region Private Members

        private DispatcherTimer _timer;

        private int _sampleRate;
        private int _channels;
        private int _bitRate;
        private EncodingFormat _format;
        private int _preset;

        private AudioCapture _capture;
        private Stream _stream;
        private IAudioWriter _writer;

        private int _bytesCaptured;
        private int _size;
        private int _duration;
        private RecorderState _state;
        private bool _isCapturing;

        #endregion

        #region Constructors

        public WasapiRecorder(Stream stream)
            : this(stream, 16000, 16, 1) { }

        public WasapiRecorder(Stream stream, int sampleRate, int bitRate, int channels)
            : this(stream, sampleRate, bitRate, channels, EncodingFormat.Wave, 128) { }

        public WasapiRecorder(Stream stream, int sampleRate, int bitRate, int channels, EncodingFormat format, int preset)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (channels != 1 && channels != 2)
                throw new ArgumentException(string.Format("Unsupported number of channels {0}", channels), "channels");

            if (bitRate != 16)
                throw new ArgumentException(string.Format("Unsupported PCM sample size {0}", bitRate), "bitRate");

            if (sampleRate < 8000 || sampleRate > 48000)
                throw new ArgumentException(string.Format("Unsupported Sample Rate {0}", sampleRate), "sampleRate");

            _stream = stream;
            _sampleRate = sampleRate;
            _bitRate = bitRate;
            _channels = channels;
            _format = format;
            _preset = preset;

            _capture = new AudioCapture();
            _capture.SetCaptureFormat(_sampleRate, (ushort)_channels, (ushort)_bitRate);

            _size = 0;
            _duration = 0;
            _state = RecorderState.Unknown;

            if (_format == EncodingFormat.Wave)
                _writer = new WaveWriter(_stream, _sampleRate, _bitRate, _channels);
            else if (_format == EncodingFormat.MP3)
                _writer = new Mp3Writer(_stream, _sampleRate, _bitRate, _channels, (LAMEPreset)_preset);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(33);
            _timer.Tick += OnSampleReceived;
        }

        #endregion

        #region Private Methods

        private void RaiseSampleReceived(float level)
        {
            if (SampleReceived != null)
                SampleReceived(this, new SampleReceivedEventArgs
                {
                    Level = level,
                    Duration = _duration,
                });
        }

        private void OnSampleReceived(object sender, EventArgs e)
        {
            byte[] buffer = null;
            int bytesRead = _capture.ReadBuffer(out buffer);

            if (bytesRead > 0)
            {
                float peak = PcmHelper.GetPeak(buffer, bytesRead);

                if (_state == RecorderState.Recording && _isCapturing && _writer.CanWrite)
                {
                    if ((SkipSilence && peak > 0.04) || !SkipSilence)
                    {
                        _size += _writer.Write(buffer, 0, bytesRead);
                        _bytesCaptured += bytesRead;
                        _duration = PcmHelper.GetDuration(_bytesCaptured, _sampleRate, _channels, _bitRate);
                    }

                }

                RaiseSampleReceived(peak);
            }
        }

        #endregion

        #region IRecorder Members

        public int SampleRate { get { return _sampleRate; } }
        public int Channels { get { return _channels; } }
        public int BitsRate { get { return _bitRate; } }
        public EncodingFormat EncodingFormat { get { return _format; } }
        public int EncodingPreset { get { return _preset; } }
        public int Size { get { return _size; } }
        public int Duration { get { return _duration; } }
        public RecorderState State { get { return _state; } }
        public bool IsCapturing { get { return _isCapturing; } }
        public bool SkipSilence { get; set; }

        public void Start()
        {
            if (_state != RecorderState.Unknown)
                throw new InvalidOperationException("A recording is already in progress.");

            if (_capture.StartCapture())
            {
                _state = RecorderState.Recording;
                _timer.Start();
                _isCapturing = true;
            }
        }

        public void Stop()
        {
            if (_state == RecorderState.Stopped || _state == RecorderState.Unknown)
                throw new InvalidOperationException("No recording in progress.");

            if (_capture.StopCapture())
            {
                _state = RecorderState.Stopped;
                _timer.Stop();
                _isCapturing = false;

                _writer.Flush();
            }

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
                if (_capture.StopCapture())
                {
                    _timer.Stop();
                    _isCapturing = false;
                }
            }
        }

        public void ResumeCapture()
        {
            if (!_isCapturing)
            {
                if (_capture.StartCapture())
                {
                    _timer.Start();
                    _isCapturing = true;
                }
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
