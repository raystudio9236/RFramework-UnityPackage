namespace RFramework.Common.Singleton
{
    /// <summary>
    /// 单例基类
    /// </summary>
    /// <typeparam name="T">单例类型</typeparam>
    public abstract class Singleton<T> where T : class, new()
    {
        private static readonly object LockObject = new object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    lock (LockObject)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }

                return _instance;
            }
        }

        public void Init()
        {
            OnInit();
        }

        public void Free()
        {
            OnFree();
            _instance = null;
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnFree()
        {
        }
    }
}