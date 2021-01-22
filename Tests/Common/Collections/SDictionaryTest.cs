using NUnit.Framework;
using RFramework.Common.Collections;

namespace RFramework.Tests.Common.Collections
{
    [TestFixture]
    public class SDictionaryTest
    {
        private SDictionary<int, string> _testDic;

        [SetUp]
        public void SetUp()
        {
            _testDic = new SDictionary<int, string>();
        }

        [TearDown]
        public void TearDown()
        {
            _testDic = null;
        }

        [Test]
        public void CtorTest()
        {
            var dic = new SDictionary<int, string>();
            Assert.AreEqual(0, dic.Count);
        }

        [Test]
        public void AddAndGetTest()
        {
            _testDic.Add(0, "hello");
            _testDic.Add(1, "world");
            Assert.AreEqual(2, _testDic.Count);
            Assert.AreEqual(_testDic[0], "hello");
            Assert.AreEqual(_testDic[1], "world");
        }

        [Test]
        public void SetAndGetTest()
        {
            _testDic.Add(0, "hello");
            _testDic.Add(1, "world");
            _testDic[0] = "nihao";
            _testDic[1] = "xiaoming";
            Assert.AreEqual(2, _testDic.Count);
            Assert.AreEqual(_testDic[0], "nihao");
            Assert.AreEqual(_testDic[1], "xiaoming");
        }

        [Test]
        public void ClearTest()
        {
            for (var i = 0; i < 10; i++)
            {
                _testDic.Add(i, i.ToString());
            }

            Assert.AreEqual(10, _testDic.Count);

            _testDic.Clear();

            Assert.AreEqual(0, _testDic.Count);
        }

        [Test]
        public void ContainsKeyTest()
        {
            for (var i = 0; i < 10; i++)
            {
                _testDic.Add(i, i.ToString());
            }

            Assert.IsTrue(_testDic.ContainsKey(5));
            Assert.IsFalse(_testDic.ContainsKey(10));
        }

        [Test]
        public void ContainsKeyValue()
        {
            for (var i = 0; i < 10; i++)
            {
                _testDic.Add(i, i.ToString());
            }

            Assert.IsTrue(_testDic.ContainsValue(5.ToString()));
            Assert.IsFalse(_testDic.ContainsValue(10.ToString()));
        }

        [Test]
        public void RemoveTest()
        {
            for (var i = 0; i < 10; i++)
            {
                _testDic.Add(i, i.ToString());
            }

            Assert.IsTrue(_testDic.ContainsKey(5));

            _testDic.Remove(5);

            Assert.IsFalse(_testDic.ContainsKey(5));
        }

        [Test]
        public void TryGetValueTest()
        {
            for (var i = 0; i < 10; i++)
            {
                _testDic.Add(i, i.ToString());
            }

            Assert.IsTrue(_testDic.TryGetValue(5, out var value));
            Assert.AreEqual("5", value);

            Assert.IsFalse(_testDic.TryGetValue(10, out _));
        }
    }
}