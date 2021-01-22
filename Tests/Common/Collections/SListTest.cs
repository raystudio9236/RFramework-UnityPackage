using System;
using NUnit.Framework;
using RFramework.Common.Collections;

namespace RFramework.Tests.Common.Collections
{
    [TestFixture]
    public class SListTest
    {
        private SList<int> _testList;

        private class TestClass
        {
        }

        [SetUp]
        public void SetUp()
        {
            _testList = new SList<int>(new[] {1, 2, 3, 4, 5});
        }

        [TearDown]
        public void TearDown()
        {
            _testList = null;
        }

        [Test]
        public void CtorTest1()
        {
            var list = new SList<int>();

            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(0, list.Capacity);
        }

        [Test]
        public void CtorTest2()
        {
            var list = new SList<int>(100);

            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(100, list.Capacity);
        }

        [Test]
        public void CtorTest3()
        {
            var intArr = new[] {1, 2, 3, 4, 5};

            var list = new SList<int>(intArr);

            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(5, list.Capacity);
        }

        [Test]
        public void CtorTest4()
        {
            var intArr = new[] {1, 2, 3, 4, 5};

            var list = new SList<int>(intArr);
            var list2 = new SList<int>(list);

            Assert.AreEqual(5, list2.Count);
            Assert.AreEqual(5, list2.Capacity);
        }

        [Test]
        public void CapacityTest()
        {
            var list = new SList<int>();
            Assert.AreEqual(0, list.Capacity);

            list.Capacity = 100;
            Assert.AreEqual(100, list.Capacity);

            for (var i = 0; i < 10; i++)
                list.Add(i);
            Assert.Catch<ArgumentOutOfRangeException>(() => { list.Capacity = 5; });
            Assert.AreEqual(100, list.Capacity);
        }

        [Test]
        public void CountTest()
        {
            var list = new SList<int>();
            Assert.AreEqual(0, list.Count);

            for (var i = 0; i < 10; i++)
            {
                list.Add(i);
                Assert.AreEqual(i + 1, list.Count);
            }
        }

        [Test]
        public void GetTest()
        {
            var list = new SList<int>();
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                var unused = list[0];
            });

            for (var i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(i, list[i]);
            }
        }

        [Test]
        public void SetTest()
        {
            var list = new SList<int>();
            Assert.Catch<IndexOutOfRangeException>(() => { list[0] = 10; });

            for (var i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            for (var i = 0; i < 10; i++)
            {
                list[i] = i + 100;
            }

            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(i + 100, list[i]);
            }
        }

        [Test]
        public void AddTest()
        {
            var list = new SList<int>();
            Assert.AreEqual(0, list.Count);

            for (var i = 0; i < 10; i++)
                list.Add(i);

            Assert.AreEqual(10, list.Count);

            for (var i = 0; i < 10; i++)
                Assert.AreEqual(i, list[i]);
        }

        [Test]
        public void AddRangeTest()
        {
            var list = new SList<int>(new[] {1, 2, 3});
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);

            var list2 = new SList<int>(new[] {7, 8, 9});
            list.AddRange(list2);

            Assert.AreEqual(6, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(7, list[3]);
            Assert.AreEqual(8, list[4]);
            Assert.AreEqual(9, list[5]);
        }

        [Test]
        public void ClearTest()
        {
            Assert.AreEqual(5, _testList.Count);

            _testList.Clear();

            Assert.AreEqual(0, _testList.Count);
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                var unused = _testList[0];
            });
        }

        [Test]
        public void ContainsTest()
        {
            Assert.IsTrue(_testList.Contains(1));
            Assert.IsFalse(_testList.Contains(10));

            var list = new SList<TestClass>();
            var obj = new TestClass();
            var obj2 = new TestClass();
            list.Add(obj);

            Assert.IsTrue(list.Contains(obj));
            Assert.IsFalse(list.Contains(obj2));
        }
    }
}