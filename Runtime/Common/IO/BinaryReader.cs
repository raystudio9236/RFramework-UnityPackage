using System;
using System.Text;

namespace RFramework.Common.IO
{
    /// <summary>
    /// 二进制数据读取器
    /// </summary>
    public class BinaryReader
    {
        private const int INIT_STRING_BUFFER_SIZE = 64;

        private static byte[] _stringBuffer;
        private static readonly Encoding _encoding;

        protected readonly BinaryBuffer _buffer;

        public uint Pos => _buffer.Pos;

        static BinaryReader()
        {
            _stringBuffer = new byte[INIT_STRING_BUFFER_SIZE];
            _encoding = new UTF8Encoding();
        }

        public BinaryReader()
        {
            _buffer = new BinaryBuffer();
        }

        public BinaryReader(BinaryWriter writer)
        {
            _buffer = new BinaryBuffer(writer.ToArray());
        }

        public BinaryReader(byte[] buffer)
        {
            _buffer = new BinaryBuffer(buffer);
        }

        public byte ReadByte()
        {
            return _buffer.ReadByte();
        }

        public Int16 ReadInt16()
        {
            return (Int16) ReadUInt16();
        }

        public UInt16 ReadUInt16()
        {
            UInt16 value = 0;
            value |= (UInt16) (_buffer.ReadByte() << 8);
            value |= _buffer.ReadByte();
            return value;
        }

        public Int32 ReadInt32()
        {
            return (Int32) ReadUInt32();
        }

        public UInt32 ReadUInt32()
        {
            UInt32 value = 0;
            value |= (UInt32) (_buffer.ReadByte() << 24);
            value |= (UInt32) (_buffer.ReadByte() << 16);
            value |= (UInt32) (_buffer.ReadByte() << 8);
            value |= _buffer.ReadByte();
            return value;
        }

        public Int64 ReadInt64()
        {
            return (Int64) ReadUInt64();
        }

        public UInt64 ReadUInt64()
        {
            UInt64 value = 0;
            value |= (UInt64) _buffer.ReadByte() << 56;
            value |= (UInt64) _buffer.ReadByte() << 48;
            value |= (UInt64) _buffer.ReadByte() << 40;
            value |= (UInt64) _buffer.ReadByte() << 32;
            value |= (UInt64) _buffer.ReadByte() << 24;
            value |= (UInt64) _buffer.ReadByte() << 16;
            value |= (UInt64) _buffer.ReadByte() << 8;
            value |= _buffer.ReadByte();
            return value;
        }

        public bool ReadBool()
        {
            var value = ReadByte();
            return value == 1;
        }

        public char ReadChar()
        {
            return (char) ReadByte();
        }

        public byte[] ReadBytes(uint count, byte[] buffer = null)
        {
            _buffer.ReadBytes(ref buffer, count);
            return buffer;
        }

        /// <summary>
        /// 读取字符串
        /// 格式：size | bytes
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            var size = ReadUInt16();
            if (size == 0)
                return string.Empty;

            while (size > _stringBuffer.Length)
                _stringBuffer = new byte[(int) (_stringBuffer.Length * 1.5f)];

            _buffer.ReadBytes(ref _stringBuffer, size);

            return _encoding.GetString(_stringBuffer, 0, size);
        }

        public void SeekBegin()
        {
            _buffer.SeekBegin();
        }

        public void Seek(uint pos)
        {
            _buffer.Seek(pos);
        }

        public void Skip(int count)
        {
            for (var i = 0; i < count; i++)
                _buffer.ReadByte();
        }
    }
}