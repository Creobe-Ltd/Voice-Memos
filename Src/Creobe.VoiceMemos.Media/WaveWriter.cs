using Creobe.VoiceMemos.Core.Media;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Media
{
    public class WaveWriter : IAudioWriter
    {
        #region Private Members

        private Stream _stream;
        private int _sampleRate;
        private int _bitRate;
        private int _channels;
        private short _blockAlign;
        private int _averageBytesPerSecond;

        #endregion

        #region Constructors

        public WaveWriter(Stream stream, int sampleRate, int bitRate, int channels)
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
            _blockAlign = (short)(channels * (bitRate / 8));
            _averageBytesPerSecond = sampleRate * _blockAlign;

            WriteHeader();
        }

        #endregion

        #region Private Methods

        private void WriteHeader()
        {
            int bytesPerSample = _bitRate / 8;
            var encoding = System.Text.Encoding.UTF8;

            _stream.Write(encoding.GetBytes("RIFF"), 0, 4);
            _stream.Write(BitConverter.GetBytes(0), 0, 4);
            _stream.Write(encoding.GetBytes("WAVE"), 0, 4);
            _stream.Write(encoding.GetBytes("fmt "), 0, 4);
            _stream.Write(BitConverter.GetBytes(16), 0, 4);
            _stream.Write(BitConverter.GetBytes((short)1), 0, 2);
            _stream.Write(BitConverter.GetBytes((short)_channels), 0, 2);
            _stream.Write(BitConverter.GetBytes(_sampleRate), 0, 4);
            _stream.Write(BitConverter.GetBytes(_sampleRate * bytesPerSample * _channels), 0, 4);
            _stream.Write(BitConverter.GetBytes((short)(bytesPerSample * _channels)), 0, 2);
            _stream.Write(BitConverter.GetBytes((short)(_bitRate)), 0, 2);
            _stream.Write(encoding.GetBytes("data"), 0, 4);
            _stream.Write(BitConverter.GetBytes(0), 0, 4);
        }

        private void UpdateHeader()
        {
            if (!_stream.CanSeek)
                throw new Exception("Can't seek stream to update wav header");

            var oldPos = _stream.Position;

            _stream.Seek(4, SeekOrigin.Begin);
            _stream.Write(BitConverter.GetBytes((int)_stream.Length - 8), 0, 4);

            _stream.Seek(40, SeekOrigin.Begin);
            _stream.Write(BitConverter.GetBytes((int)_stream.Length - 44), 0, 4);
            _stream.Seek(oldPos, SeekOrigin.Begin);
        }

        private async Task WriteHeaderAsync()
        {
            await Task.Run(async () =>
            {
                int bytesPerSample = _bitRate / 8;
                var encoding = System.Text.Encoding.UTF8;

                await _stream.WriteAsync(encoding.GetBytes("RIFF"), 0, 4);
                await _stream.WriteAsync(BitConverter.GetBytes(0), 0, 4);
                await _stream.WriteAsync(encoding.GetBytes("WAVE"), 0, 4);
                await _stream.WriteAsync(encoding.GetBytes("fmt "), 0, 4);
                await _stream.WriteAsync(BitConverter.GetBytes(16), 0, 4);
                await _stream.WriteAsync(BitConverter.GetBytes((short)1), 0, 2);
                await _stream.WriteAsync(BitConverter.GetBytes((short)_channels), 0, 2);
                await _stream.WriteAsync(BitConverter.GetBytes(_sampleRate), 0, 4);
                await _stream.WriteAsync(BitConverter.GetBytes(_sampleRate * bytesPerSample * _channels), 0, 4);
                await _stream.WriteAsync(BitConverter.GetBytes((short)(bytesPerSample * _channels)), 0, 2);
                await _stream.WriteAsync(BitConverter.GetBytes((short)(_bitRate)), 0, 2);
                await _stream.WriteAsync(encoding.GetBytes("data"), 0, 4);
                await _stream.WriteAsync(BitConverter.GetBytes(0), 0, 4);
            });
        }

        private async Task UpdateHeaderAsync()
        {
            if (!_stream.CanSeek)
                throw new Exception("Can't seek stream to update wav header");

            await Task.Run(async () =>
            {
                var oldPos = _stream.Position;

                _stream.Seek(4, SeekOrigin.Begin);
                await _stream.WriteAsync(BitConverter.GetBytes((int)_stream.Length - 8), 0, 4);

                _stream.Seek(40, SeekOrigin.Begin);
                await _stream.WriteAsync(BitConverter.GetBytes((int)_stream.Length - 44), 0, 4);
                _stream.Seek(oldPos, SeekOrigin.Begin);
            });
        }

        #endregion

        #region IAudioWriter Members

        public bool CanWrite
        {
            get { return (_stream != null && _stream.CanWrite); }
        }

        public int Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
            return count;
        }

        public async Task<int> WriteAsync(byte[] buffer, int offset, int count)
        {
            await _stream.WriteAsync(buffer, offset, count);
            return count;
        }

        public void Flush()
        {
            UpdateHeader();
        }

        public async Task FlushAsync()
        {
            await UpdateHeaderAsync();
            await _stream.FlushAsync();
        }


        #endregion
    }
}
