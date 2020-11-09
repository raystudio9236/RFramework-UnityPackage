namespace RFramework.Common.Misc
{
    public class AutoIndex
    {
        public static ByteAutoIdx Byte
        {
            get
            {
                return new ByteAutoIdx();
            }
        }

        public static ShortAutoIdx Short
        {
            get
            {
                return new ShortAutoIdx();
            }
        }

        public static IntAutoIdx Int
        {
            get
            {
                return new IntAutoIdx();
            }
        }
    }

    public class AutoIndex<T> where T : struct
    {
        protected T _idx;

        public AutoIndex(T start)
        {
            _idx = start;
        }

        public T Get()
        {
            OnIncIdx();
            return _idx;
        }

        protected virtual void OnIncIdx()
        {
        }
    }

    public class ByteAutoIdx : AutoIndex<byte>
    {
        public ByteAutoIdx(byte start = 0) : base(start)
        {
        }

        protected override void OnIncIdx()
        {
            _idx += 1;
        }
    }

    public class ShortAutoIdx : AutoIndex<short>
    {
        public ShortAutoIdx(short start = 0) : base(start)
        {
        }

        protected override void OnIncIdx()
        {
            _idx += 1;
        }
    }


    public class IntAutoIdx : AutoIndex<int>
    {
        public IntAutoIdx(int start = 0) : base(start)
        {
        }

        protected override void OnIncIdx()
        {
            _idx += 1;
        }
    }

    public static class AutoIndexEx
    {
        public static T Idx<T>(this AutoIndex<T> autoIndex) where T : struct
        {
            return autoIndex.Get();
        }
    }
}