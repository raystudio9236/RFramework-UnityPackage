using System;
using System.Collections.Generic;

namespace RFramework.Common.Collections
{
    /// <summary>
    /// Simple List
    /// </summary>
    public class SList<T>
    {
        private const int DEFAULT_CAPACITY = 4;
        private const float GROW_FACTOR = 1.5f;

        private static readonly T[] Empty = new T[0];

        private T[] _items;
        private int _size;

        public SList()
        {
            _items = Empty;
        }

        public SList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentException("Capacity cannot less than zero");

            _items = capacity == 0 ? Empty : new T[capacity];
        }


        public SList(T[] arr)
        {
            if (arr is null)
                throw new ArgumentException("Array cannot be null");

            var count = arr.Length;
            _items = count == 0 ? Empty : new T[count];
            if (count > 0)
            {
                arr.CopyTo(_items, 0);
                _size = count;
            }
        }

        public SList(SList<T> list)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            var count = list.Count;
            _items = count == 0 ? Empty : new T[count];

            if (count > 0)
            {
                list.CopyTo(_items);
                _size = count;
            }
        }

        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _size)
                    throw new ArgumentOutOfRangeException(nameof(Capacity), "Capacity cannot less than size");

                if (value == _items.Length)
                    return;

                if (value > 0)
                {
                    var newItems = new T[value];

                    if (_size > 0)
                        Array.Copy(_items, 0, newItems, 0, _size);

                    _items = newItems;
                }
                else
                {
                    _items = Empty;
                }
            }
        }

        public int Count => _size;

        public T this[int index]
        {
            get
            {
                if (index >= _size)
                    throw new IndexOutOfRangeException();

                return _items[index];
            }
            set
            {
                if (index >= _size)
                    throw new IndexOutOfRangeException();

                _items[index] = value;
            }
        }

        public void Add(T value)
        {
            CheckSpace(1);

            _items[_size++] = value;
        }

        public void AddRange(SList<T> list)
        {
            InsertRange(_size, list);
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }
        }

        public bool Contains(T item)
        {
            if (item is null)
            {
                for (var i = 0; i < _size; i++)
                    if (_items[i] is null)
                        return true;

                return false;
            }
            else
            {
                var comparer = EqualityComparer<T>.Default;
                for (var i = 0; i < _size; i++)
                    if (comparer.Equals(_items[i], item))
                        return true;

                return false;
            }
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return Array.BinarySearch(_items, index, count, item, comparer);
        }

        public int BinarySearch(T item, IComparer<T> comparer = null)
        {
            return BinarySearch(0, Count, item, comparer);
        }

        public void CopyTo(T[] array, int arrayIndex = 0)
        {
            CopyTo(0, array, arrayIndex, Count);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            Array.Copy(_items, index, array, arrayIndex, count);
        }

        public bool Exists(Predicate<T> predicate)
        {
            return FindIndex(predicate) != -1;
        }

        public T Find(Predicate<T> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            for (var i = 0; i < _size; i++)
                if (predicate(_items[i]))
                    return _items[i];

            return default;
        }

        public SList<T> FindAll(Predicate<T> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var result = new SList<T>();
            for (var i = 0; i < _size; i++)
            {
                if (predicate(_items[i]))
                    result.Add(_items[i]);
            }

            return result;
        }

        public int FindIndex(Predicate<T> predicate)
        {
            return FindIndex(0, _size, predicate);
        }

        public int FindIndex(int startIndex, Predicate<T> predicate)
        {
            return FindIndex(startIndex, _size - startIndex, predicate);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> predicate)
        {
            if (startIndex > _size)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0 || startIndex > _size - count)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var endIndex = startIndex + count;
            for (var i = startIndex; i < endIndex; i++)
            {
                if (predicate(_items[i]))
                    return i;
            }

            return -1;
        }

        public T FindLast(Predicate<T> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            for (var i = _size - 1; i >= 0; i--)
            {
                if (predicate(_items[i]))
                    return _items[i];
            }

            return default;
        }

        public int FindLastIndex(Predicate<T> predicate)
        {
            return FindLastIndex(_size - 1, _size, predicate);
        }

        public int FindLastIndex(int startIndex, Predicate<T> predicate)
        {
            return FindLastIndex(startIndex, startIndex + 1, predicate);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (startIndex > _size)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (_size == 0)
                return -1;

            if (count < 0 || startIndex - count + 1 < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var endIndex = startIndex - count;
            for (var i = startIndex; i > endIndex; i--)
                if (predicate(_items[i]))
                    return i;

            return -1;
        }

        public SList<T> GetRange(int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (_size - index < count)
                throw new ArgumentException(nameof(count));

            var result = new SList<T>();
            Array.Copy(_items, index, result._items, 0, count);
            result._size = count;
            return result;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _size);
        }

        public int IndexOf(T item, int startIndex, int count = -1)
        {
            return Array.IndexOf(_items, item, startIndex, count < 0 ? _size : count);
        }

        public void Insert(int index, T value)
        {
            if (index > _size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (_size == _items.Length)
                CheckSpace(1);

            if (index < _size)
                Array.Copy(_items, index, _items, index + 1, _size - index);

            _items[index] = value;
            _size++;
        }

        public void InsertRange(int index, SList<T> list)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            if (index > _size)
                throw new ArgumentOutOfRangeException(nameof(index));

            var count = list.Count;
            if (count > 0)
            {
                CheckSpace(count);

                if (index < _size)
                    Array.Copy(_items, index, _items, index + count, _size - index);

                if (this == list)
                {
                    Array.Copy(_items, 0, _items, index, index);
                    Array.Copy(_items, index + count, _items, index * 2, _size - index);
                }
                else
                {
                    Array.Copy(list._items, 0, _items, index, count);
                }

                _size += count;
            }
        }

        public int LastIndexOf(T item)
        {
            if (_size == 0)
                return -1;
            else
                return LastIndexOf(item, _size - 1, _size);
        }

        public int LastIndexOf(T item, int index)
        {
            if (index > _size)
                throw new ArgumentOutOfRangeException(nameof(index));

            return LastIndexOf(item, index, index + 1);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || count > index + 1)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (_size == 0)
                return -1;

            return Array.LastIndexOf(_items, item, index, count);
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public int RemoveAll(Predicate<T> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var freeIndex = 0;

            while (freeIndex < _size && !predicate(_items[freeIndex]))
                freeIndex++;

            if (freeIndex >= _size)
                return 0;

            var currentIndex = freeIndex + 1;
            while (currentIndex < _size)
            {
                while (currentIndex < _size && predicate(_items[currentIndex]))
                    currentIndex++;

                if (currentIndex < _size)
                    _items[freeIndex++] = _items[currentIndex++];
            }

            Array.Clear(_items, freeIndex, _size - freeIndex);
            var result = _size - freeIndex;
            _size = freeIndex;
            return result;
        }

        public void RemoveAt(int index)
        {
            if (index >= _size)
                throw new ArgumentOutOfRangeException(nameof(index));

            _size--;

            if (index < _size)
                Array.Copy(_items, index + 1, _items, index, _size - index);

            _items[_size] = default;
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (_size - index < count)
                throw new ArgumentException();

            if (count > 0)
            {
                _size -= count;

                if (index < _size)
                    Array.Copy(_items, index + count, _items, index, _size - index);

                Array.Clear(_items, _size, count);
            }
        }

        public void Reverse()
        {
            Reverse(0, Count);
        }

        public void Reverse(int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (_size - index < count)
                throw new ArgumentException();

            Array.Reverse(_items, index, count);
        }

        public void Sort()
        {
            Sort(0, Count, null);
        }

        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (_size - index < count)
                throw new ArgumentException();

            Array.Sort(_items, index, count, comparer);
        }

        public T[] ToArray()
        {
            var array = new T[_size];
            Array.Copy(_items, 0, array, 0, _size);
            return array;
        }

        private void CheckSpace(int count)
        {
            var newCapacity = _items.Length;
            while (_size + count >= newCapacity)
            {
                newCapacity = newCapacity == 0 ? DEFAULT_CAPACITY : (int) (newCapacity * GROW_FACTOR);
            }

            if (newCapacity != _items.Length)
                Capacity = newCapacity;
        }
    }
}