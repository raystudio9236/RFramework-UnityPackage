namespace RFramework.Common.Misc
{
    /// <summary>
    /// 自增索引类
    /// </summary>
    public class AutoIndex
    {
        public static ByteAutoIdx Byte => new ByteAutoIdx();

        public static ShortAutoIdx Short => new ShortAutoIdx();

        public static IntAutoIdx Int => new IntAutoIdx();
    }

    public class AutoIndex<T> where T : struct
    {
        protected T Idx;

        public AutoIndex(T start)
        {
            Idx = start;
        }

        public T Get()
        {
            OnIncIdx();
            return Idx;
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
            Idx += 1;
        }
    }

    public class ShortAutoIdx : AutoIndex<short>
    {
        public ShortAutoIdx(short start = 0) : base(start)
        {
        }

        protected override void OnIncIdx()
        {
            Idx += 1;
        }
    }


    public class IntAutoIdx : AutoIndex<int>
    {
        public IntAutoIdx(int start = 0) : base(start)
        {
        }

        protected override void OnIncIdx()
        {
            Idx += 1;
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