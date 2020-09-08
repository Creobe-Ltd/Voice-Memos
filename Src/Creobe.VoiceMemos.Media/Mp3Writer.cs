using Creobe.VoiceMemos.Core.Media;
using Creobe.VoiceMemos.Media.Lame;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Media
{
    public class Mp3Writer : IAudioWriter
    {
        #region Delegates

        delegate int EncodeAction();

        #endregion

        #region Private Members

        private LibMp3Lame _lame;

        private Stream _stream;
        private int _sampleRate;
        private int _bitRate;
        private int _channels;
        private short _blockAlign;
        private int _averageBytesPerSecond;

        private LAMEPreset _preset;

        private ArrayUnion _inBuffer = null;
        private int _inPosition;
        private byte[] _outBuffer;
        private long _inByteCount = 0;
        private long _outByteCount = 0;

        EncodeAction _encode = null;

        #endregion

        #region Constructors

        public Mp3Writer(Stream stream, int sampleRate, int bitRate, int channels, LAMEPreset preset)
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
            _preset = preset;

            _inBuffer = new ArrayUnion(_averageBytesPerSecond);
            _outBuffer = new byte[sampleRate * 5 / 4 + 7200];

            this._lame = new LibMp3Lame();
            this._lame.InputSampleRate = _sampleRate;
            this._lame.NumChannels = _channels;
            this._lame.SetPreset(_preset);

            this._lame.InitParams();

            if (_channels == 1)
                _encode = EncodePcm16Mono;
            else
                _encode = EncodePcm16stereo;
        }

        #endregion

        #region Private Methods

        private int EncodePcm16Mono()
        {
            return _lame.Write(_inBuffer.shorts, _inPosition / 2, _outBuffer, _outBuffer.Length, true);
        }

        private int EncodePcm16stereo()
        {
            return _lame.Write(_inBuffer.shorts, _inPosition / 2, _outBuffer, _outBuffer.Length, false);
        }

        private int EncodeFloatMono()
        {
            return _lame.Write(_inBuffer.floats, _inPosition / 4, _outBuffer, _outBuffer.Length, true);
        }

        private int EncodeFloatStereo()
        {
            return _lame.Write(_inBuffer.floats, _inPosition / 4, _outBuffer, _outBuffer.Length, false);
        }

        private int Encode()
        {
            if (_stream == null || _lame == null)
                throw new InvalidOperationException("Output Stream closed.");

            if (_inPosition < _channels * 2)
                return 0;

            int rc = _encode();

            if (rc > 0)
            {
                _stream.Write(_outBuffer, 0, rc);
                _outByteCount += rc;
            }

            _inByteCount += _inPosition;
            _inPosition = 0;

            return rc;
        }

        #endregion

        #region IAudioWriter Members

        public bool CanWrite
        {
            get { return (_stream != null && _stream.CanWrite && _lame != null); }
        }

        public int Write(byte[] buffer, int offset, int count)
        {
            int rc = 0;

            while (count > 0)
            {
                int blockSize = Math.Min(_inBuffer.nBytes - _inPosition, count);
                Buffer.BlockCopy(buffer, offset, _inBuffer.bytes, _inPosition, blockSize);

                _inPosition += blockSize;
                count -= blockSize;
                offset += blockSize;

                if (_inPosition >= _inBuffer.nBytes)
                    rc = Encode();
            }

            return rc;
        }

        public Task<int> WriteAsync(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            if (_inPosition > 0)
                Encode();

            int rc = _lame.Flush(_outBuffer, _outBuffer.Length);
            if (rc > 0)
                _stream.Write(_outBuffer, 0, rc);

            _stream = null;
        }

        public Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Classes

        [StructLayout(LayoutKind.Explicit)]
        private class ArrayUnion
        {
            [FieldOffset(0)]
            public readonly int nBytes;

            [FieldOffset(16)]
            public readonly byte[] bytes;

            [FieldOffset(16)]
            public readonly short[] shorts;

            [FieldOffset(16)]
            public readonly int[] ints;

            [FieldOffset(16)]
            public readonly long[] longs;

            [FieldOffset(16)]
            public readonly float[] floats;

            [FieldOffset(16)]
            public readonly double[] doubles;

            public int nShorts { get { return nBytes / 2; } }
            public int nInts { get { return nBytes / 4; } }
            public int nLongs { get { return nBytes / 8; } }
            public int nFloats { get { return nBytes / 4; } }
            public int nDoubles { get { return nBytes / 8; } }

            public ArrayUnion(int reqBytes)
            {
                int reqDoubles = (reqBytes + 7) / 8;

                this.doubles = new double[reqDoubles];
                this.nBytes = reqDoubles * 8;
            }
        };

        #endregion
    }
}
