using NUnit.Framework;
using RFramework.Common.IO;

namespace RFramework.Tests.Common.IO
{
    [TestFixture]
    public class BinaryBufferTest
    {
        private BinaryBuffer _buffer;

        [SetUp]
        public void BeforeTest()
        {
            var bytes = new byte[]
            {
                1, 2, 3, 4, 5,
                6, 7, 8, 9, 10
            };

            _buffer = new BinaryBuffer(bytes);
        }

        [TearDown]
        public void AfterTest()
        {
            _buffer = null;
        }

        [Test]
        public void ReadByteTest()
        {
            Assert.AreEqual(1, _buffer.ReadByte());
            Assert.AreEqual(2, _buffer.ReadByte());
            Assert.AreEqual(3, _buffer.ReadByte());
            Assert.AreEqual(4, _buffer.ReadByte());

            Assert.AreEqual(4, _buffer.Pos);
        }

        [Test]
        public void ReadBytesTest()
        {
            byte[] result = null;
            _buffer.ReadBytes(ref result, 4);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
            Assert.AreEqual(4, result[3]);

            Assert.AreEqual(4, _buffer.Pos);
        }

        [Test]
        public void WriteByteTest()
        {
            _buffer.SeekBegin();
            _buffer.WriteByte(100);
            Assert.AreEqual(1, _buffer.Pos);
            _buffer.SeekBegin();
            Assert.AreEqual(100, _buffer.ReadByte());
        }

        [Test]
        public void WriteByte2Test()
        {
            _buffer.SeekBegin();
            _buffer.WriteByte2(100, 200);
            Assert.AreEqual(2, _buffer.Pos);
            _buffer.SeekBegin();
            Assert.AreEqual(100, _buffer.ReadByte());
            Assert.AreEqual(200, _buffer.ReadByte());
        }

        [Test]
        public void WriteByte4Test()
        {
            _buffer.SeekBegin();
            _buffer.WriteByte4(100, 200, 220, 240);
            Assert.AreEqual(4, _buffer.Pos);
            _buffer.SeekBegin();
            Assert.AreEqual(100, _buffer.ReadByte());
            Assert.AreEqual(200, _buffer.ReadByte());
            Assert.AreEqual(220, _buffer.ReadByte());
            Assert.AreEqual(240, _buffer.ReadByte());
        }

        [Test]
        public void WriteByte8Test()
        {
            _buffer.SeekBegin();
            _buffer.WriteByte8(100, 200, 220, 240, 244, 246, 248, 250);
            Assert.AreEqual(8, _buffer.Pos);
            _buffer.SeekBegin();
            Assert.AreEqual(100, _buffer.ReadByte());
            Assert.AreEqual(200, _buffer.ReadByte());
            Assert.AreEqual(220, _buffer.ReadByte());
            Assert.AreEqual(240, _buffer.ReadByte());
            Assert.AreEqual(244, _buffer.ReadByte());
            Assert.AreEqual(246, _buffer.ReadByte());
            Assert.AreEqual(248, _buffer.ReadByte());
            Assert.AreEqual(250, _buffer.ReadByte());
        }

        [Test]
        public void WriteBytesTest()
        {
            _buffer.SeekBegin();
            var bytes = new byte[]
            {
                20, 40, 60, 80, 100
            };

            _buffer.WriteBytes(bytes, 5);
            Assert.AreEqual(5, _buffer.Pos);
            _buffer.SeekBegin();
            Assert.AreEqual(20, _buffer.ReadByte());
            Assert.AreEqual(40, _buffer.ReadByte());
            Assert.AreEqual(60, _buffer.ReadByte());
            Assert.AreEqual(80, _buffer.ReadByte());
            Assert.AreEqual(100, _buffer.ReadByte());
        }

        [Test]
        public void ToArrayTest()
        {
            for (var i = 0; i < 10; i++)
                _buffer.ReadByte();

            var array = _buffer.ToArray();
            Assert.AreEqual(10, array.Length);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);
            Assert.AreEqual(6, array[5]);
            Assert.AreEqual(7, array[6]);
            Assert.AreEqual(8, array[7]);
            Assert.AreEqual(9, array[8]);
            Assert.AreEqual(10, array[9]);
        }

        [Test]
        public void SeekBeginTest()
        {
            for (var i = 0; i < 10; i++)
                _buffer.ReadByte();
            Assert.AreEqual(10, _buffer.Pos);
            _buffer.SeekBegin();
            Assert.AreEqual(0, _buffer.Pos);
        }

        [Test]
        public void SeekTest()
        {
            for (var i = 0; i < 10; i++)
                _buffer.ReadByte();
            Assert.AreEqual(10, _buffer.Pos);
            _buffer.Seek(2);
            Assert.AreEqual(2, _buffer.Pos);
            _buffer.Seek(100);
            Assert.AreEqual(100, _buffer.Pos);
        }

        [Test]
        public void ArrangementTest()
        {
            _buffer.Seek(2);
            _buffer.Arrangement();
            _buffer.SeekBegin();
            Assert.AreEqual(3, _buffer.ReadByte());
        }
    }
}