using System;
using System.Text;

namespace RFramework.Common.IO
{
    /// <summary>
    /// 二进制数据写入器
    /// </summary>
    public class BinaryWriter
    {
        private const int INIT_STRING_BUFFER_SIZE = 64;

        private static byte[] _stringBuffer;
        private static Encoding _encoding;

        protected readonly BinaryBuffer _buffer;

        public uint Pos => _buffer.Pos;

        static BinaryWriter()
        {
            _stringBuffer = new byte[INIT_STRING_BUFFER_SIZE];
            _encoding = new UTF8Encoding();
        }

        public BinaryWriter()
        {
            _buffer = new BinaryBuffer();
        }

        public BinaryWriter(BinaryBuffer buffer)
        {
            _buffer = buffer;
        }

        public BinaryWriter(byte[] buffer)
        {
            _buffer = new BinaryBuffer(buffer);
        }

        public byte[] ToArray()
        {
            return _buffer.ToArray();
        }

        public void WriteByte(byte value)
        {
            _buffer.WriteByte(value);
        }

        public void WriteChar(char value)
        {
            _buffer.WriteByte((byte) value);
        }

        public void WriteBool(bool value)
        {
            _buffer.WriteByte(value ? (byte) 1 : (byte) 0);
        }

        public void WriteInt16(Int16 value)
        {
            WriteUInt16((ushort) value);
        }

        public void WriteUInt16(UInt16 value)
        {
            _buffer.WriteByte2(
                (byte) ((value >> 8) & 0xff),
                (byte) (value & 0xff));
        }

        public void WriteInt32(Int32 value)
        {
            WriteUInt32((UInt32) value);
        }

        public void WriteUInt32(UInt32 value)
        {
            _buffer.WriteByte4(
                (byte) ((value >> 24) & 0xff),
                (byte) ((value >> 16) & 0xff),
                (byte) ((value >> 8) & 0xff),
                (byte) (value & 0xff));
        }

        public void WriteInt64(Int64 value)
        {
            WriteUInt64((UInt64) value);
        }

        public void WriteUInt64(UInt64 value)
        {
            _buffer.WriteByte8(
                (byte) ((value >> 56) & 0xff),
                (byte) ((value >> 48) & 0xff),
                (byte) ((value >> 40) & 0xff),
                (byte) ((value >> 32) & 0xff),
                (byte) ((value >> 24) & 0xff),
                (byte) ((value >> 16) & 0xff),
                (byte) ((value >> 8) & 0xff),
                (byte) (value & 0xff));
        }

        public void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteUInt16(0);
                return;
            }

            var byteCount = _encoding.GetByteCount(value);
            if (byteCount > ushort.MaxValue)
            {
                throw new ArgumentException(
                    $"BinaryWriter WriteString: value is too long ({byteCount}/{ushort.MaxValue})");
            }

            while (_stringBuffer.Length < byteCount)
                _stringBuffer = new byte[(int) (_stringBuffer.Length * 1.5f)];

            WriteUInt16((ushort) byteCount);
            var bytesSize = _encoding.GetBytes(value, 0, value.Length, _stringBuffer, 0);
            _buffer.WriteBytes(_stringBuffer, bytesSize);
        }

        public void SeekBegin()
        {
            _buffer.SeekBegin();
        }

        public void Seek(uint pos)
        {
            _buffer.Seek(pos);
        }

        public void Arrangement(int pos = -1)
        {
            _buffer.Arrangement(pos);
        }
    }
}