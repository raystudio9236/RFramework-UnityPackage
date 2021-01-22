using System;
using System.Collections.Generic;
using RFramework.Common.Extension;

namespace RFramework.Common.Collections
{
    /// <summary>
    /// Simple Dictionary
    /// </summary>
    public class SDictionary<TKey, TValue>
    {
        private const int HashMask = 0x7FFFFFFF;

        private static class HashHelper
        {
            public const int HashPrime = 101;
            public const int MaxPrimeArrayLength = 0x7FEFFFFD;

            private static readonly int[] Primes =
            {
                3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107,
                131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
                1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049,
                4861, 5839, 7013, 8419, 10103, 12143, 14591,
                17519, 21023, 25229, 30293, 36353, 43627, 52361,
                62851, 75431, 90523, 108631, 130363, 156437,
                187751, 225307, 270371, 324449, 389357, 467237,
                560689, 672827, 807403, 968897, 1162687, 1395263,
                1674319, 2009191, 2411033, 2893249, 3471899, 4166287,
                4999559, 5999471, 7199369
            };

            public static bool IsPrime(int candidate)
            {
                if ((candidate & 1) != 0)
                {
                    var limit = (int) Math.Sqrt(candidate);
                    for (var divisor = 3; divisor <= limit; divisor += 2)
                        if (candidate % divisor == 0)
                            return false;
                    return true;
                }

                return candidate == 2;
            }

            public static int GetPrime(int min)
            {
                if (min < 0)
                    throw new ArgumentOutOfRangeException(nameof(min));

                foreach (var p in Primes)
                    if (p >= min)
                        return p;

                for (var i = (min | 1); i < int.MaxValue; i += 2)
                    if (IsPrime(i) && (i - 1) % HashPrime != 0)
                        return i;

                return min;
            }

            public static int ExpandPrime(int oldSize)
            {
                var newSize = 2 * oldSize;
                if ((uint) newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
                    return MaxPrimeArrayLength;
                else
                    return GetPrime(newSize);
            }
        }

        public struct Entry
        {
            public int HashCode;
            public int Next;
            public TKey Key;
            public TValue Value;
        }

        private int[] _buckets;
        private Entry[] _entries;
        private int _count;
        private int _freeList;
        private int _freeCount;

        public IEqualityComparer<TKey> Comparer { get; }

        public int Count => _count - _freeCount;

        public Entry[] Entries => _entries;

        public SDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer)
        {
        }

        public SDictionary(int capacity = 0, IEqualityComparer<TKey> comparer = null)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            if (capacity > 0) Init(capacity);

            Comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public TValue this[TKey key]
        {
            set => Insert(key, value, false);
            get
            {
                var index = FindEntry(key);
                if (index >= 0)
                    return _entries[index].Value;
                else
                    throw new KeyNotFoundException($"Key: {key}");
            }
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public void Clear()
        {
            if (_count <= 0)
                return;

            _buckets.Fill(-1);
            Array.Clear(_entries, 0, _count);
            _freeList = -1;
            _count = 0;
            _freeCount = 0;
        }

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            if (value is null)
            {
                for (var i = 0; i < _count; i++)
                    if (_entries[i].HashCode >= 0 && _entries[i].Value is null)
                        return true;
            }
            else
            {
                var comparer = EqualityComparer<TValue>.Default;
                for (var i = 0; i < _count; i++)
                    if (_entries[i].HashCode >= 0 && comparer.Equals(_entries[i].Value, value))
                        return true;
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            if (_buckets == null)
                return false;

            var hashCode = Comparer.GetHashCode(key) & HashMask;
            var bucket = hashCode % _buckets.Length;
            var last = -1;
            for (var i = _buckets[bucket]; i >= 0; last = i, i = _entries[i].Next)
            {
                if (_entries[i].HashCode == hashCode && Comparer.Equals(_entries[i].Key, key))
                {
                    if (last < 0)
                        _buckets[bucket] = _entries[i].Next;
                    else
                        _entries[last].Next = _entries[i].Next;

                    _entries[i] = new Entry
                    {
                        HashCode = -1,
                        Next = _freeList,
                        Key = default,
                        Value = default
                    };
                    _freeList = i;
                    _freeCount++;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var index = FindEntry(key);
            if (index >= 0)
            {
                value = _entries[index].Value;
                return true;
            }

            value = default;
            return false;
        }

        private void Init(int capacity)
        {
            var size = HashHelper.GetPrime(capacity);
            _buckets = new int[size];
            _buckets.Fill(-1);
            _entries = new Entry[size];
            _freeList = -1;
        }

        private void Insert(TKey key, TValue value, bool isAdd)
        {
            if (_buckets is null)
                Init(0);

            var hashCode = Comparer.GetHashCode(key) & HashMask;
            var targetBucket = hashCode % _buckets.Length;

            for (var i = _buckets[targetBucket]; i >= 0; i = _entries[i].Next)
            {
                var entry = _entries[i];
                if (entry.HashCode == hashCode && Comparer.Equals(entry.Key, key))
                {
                    if (isAdd)
                        throw new ArgumentException($"Key {key} is already in dictionary");

                    _entries[i].Value = value;
                    return;
                }
            }

            int index;
            if (_freeCount > 0)
            {
                index = _freeList;
                _freeList = _entries[index].Next;
                _freeCount--;
            }
            else
            {
                if (_count == _entries.Length)
                {
                    Resize();
                    targetBucket = hashCode % _buckets.Length;
                }

                index = _count;
                _count++;
            }

            _entries[index] = new Entry
            {
                HashCode = hashCode,
                Next = _buckets[targetBucket],
                Key = key,
                Value = value
            };
            _buckets[targetBucket] = index;
        }

        private void Resize()
        {
            Resize(HashHelper.ExpandPrime(_count));
        }

        private void Resize(int newSize, bool forceNewHashCode = false)
        {
            if (newSize < _entries.Length)
                throw new ArgumentException("New size cannot less than current length", nameof(newSize));

            var newBuckets = new int[newSize];
            newBuckets.Fill(-1);
            var newEntries = new Entry[newSize];
            Array.Copy(_entries, 0, newEntries, 0, _count);
            if (forceNewHashCode)
            {
                for (var i = 0; i < _count; i++)
                    if (newEntries[i].HashCode != -1)
                        newEntries[i].HashCode = Comparer.GetHashCode(newEntries[i].Key) & HashMask;
            }

            for (var i = 0; i < _count; i++)
            {
                if (newEntries[i].HashCode >= 0)
                {
                    var bucket = newEntries[i].HashCode % newSize;
                    newEntries[i].Next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }

            _buckets = newBuckets;
            _entries = newEntries;
        }

        private int FindEntry(TKey key)
        {
            if (_buckets == null)
                return -1;

            var hashCode = Comparer.GetHashCode(key) & HashMask;
            for (var i = _buckets[hashCode % _buckets.Length]; i >= 0; i = _entries[i].Next)
                if (_entries[i].HashCode == hashCode && Comparer.Equals(_entries[i].Key, key))
                    return i;

            return -1;
        }
    }
}