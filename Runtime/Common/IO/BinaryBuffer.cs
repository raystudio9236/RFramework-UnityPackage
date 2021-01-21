using System;
using RFramework.Common.Log;

namespace RFramework.Common.IO
{
    /// <summary>
    /// 变长二进制数据缓存类
    /// </summary>
    public class BinaryBuffer
    {
        private const int INIT_SIZE = 4;
        private const int WARNING_SIZE = 1024 * 1024 * 128;

        private const float GROW_FACTOR = 1.5f;

        private byte[] _buffer;
        private uint _pos;

        public uint Pos => _pos;

        public BinaryBuffer() : this(INIT_SIZE)
        {
        }

        public BinaryBuffer(int size) : this(new byte[size])
        {
        }

        public BinaryBuffer(byte[] buffer)
        {
            _buffer = buffer;
        }

        public byte ReadByte()
        {
            if (_pos >= _buffer.Length)
                throw new IndexOutOfRangeException($"BinaryBuffer ReadByte, pos out of range ({_pos}/{_buffer.Length})");

            return _buffer[_pos++];
        }

        public void ReadBytes(ref byte[] outBuffer, uint count)
        {
            if (_pos + count >= _buffer.Length)
                throw new IndexOutOfRangeException(
                    $"BinaryBuffer ReadByte, pos out of range ({_pos + count}/{_buffer.Length})");

            if (outBuffer == null)
                outBuffer = new byte[count];

            Array.Copy(_buffer, _pos, outBuffer, 0, count);

            _pos += count;
        }

        public void WriteByte(byte value)
        {
            CheckSpace(1);
            _buffer[_pos] = value;
            _pos++;
        }

        public void WriteByte2(byte value1, byte value2)
        {
            CheckSpace(2);
            _buffer[_pos] = value1;
            _buffer[_pos + 1] = value2;
            _pos += 2;
        }

        public void WriteByte4(
            byte value1, byte value2,
            byte value3, byte value4)
        {
            CheckSpace(4);
            _buffer[_pos] = value1;
            _buffer[_pos + 1] = value2;
            _buffer[_pos + 2] = value3;
            _buffer[_pos + 3] = value4;
            _pos += 4;
        }

        public void WriteByte8(
            byte value1, byte value2,
            byte value3, byte value4,
            byte value5, byte value6,
            byte value7, byte value8)
        {
            CheckSpace(8);
            _buffer[_pos] = value1;
            _buffer[_pos + 1] = value2;
            _buffer[_pos + 2] = value3;
            _buffer[_pos + 3] = value4;
            _buffer[_pos + 4] = value5;
            _buffer[_pos + 5] = value6;
            _buffer[_pos + 6] = value7;
            _buffer[_pos + 7] = value8;
            _pos += 8;
        }

        public void WriteBytes(byte[] data, int count = -1)
        {
            if (count < 0)
                count = data.Length;

            CheckSpace((uint) count);

            Array.Copy(data, 0, _buffer, _pos, count);
            _pos += (uint) count;
        }

        public byte[] ToArray()
        {
            var result = new byte[_pos];
            Array.Copy(_buffer, result, _pos);
            return result;
        }

        public void SeekBegin()
        {
            _pos = 0;
        }

        public void Seek(uint pos)
        {
            SeekBegin();
            CheckSpace(pos);
            _pos = pos;
        }

        public void Arrangement(int pos = -1)
        {
            pos = pos < 0 ? (int) _pos : pos;

            var len = _buffer.Length;
            if (pos > len)
                pos = len;

            var size = len - pos;

            Buffer.BlockCopy(_buffer, pos, _buffer, 0, size);
            _pos = 0;
        }

        private void CheckSpace(uint count)
        {
            if (_pos + count < _buffer.Length)
                return;

            var newLen = (int) (_buffer.Length * GROW_FACTOR);
            while (_pos + count >= newLen)
            {
                newLen = (int) (newLen * GROW_FACTOR);
                RLog.Assert(newLen <= WARNING_SIZE);
            }

            var newArr = new byte[newLen];
            _buffer.CopyTo(newArr, 0);
            _buffer = newArr;
        }
    }
}