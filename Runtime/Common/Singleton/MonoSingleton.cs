using UnityEngine;

namespace RFramework.Common.Singleton
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance != null)
                        DontDestroyOnLoad(_instance.gameObject);
                }

                if (_instance != null) return _instance;

                var go = new GameObject(typeof(T) + "_Singleton");
                _instance = go.AddComponent<T>();

                return _instance;
            }
        }

        public static T GetMonoInstance()
        {
            if (_instance != null) return _instance;

            if (Application.isPlaying)
            {
                var go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }

        [SerializeField] private bool autoInit;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                _instance = this as T;
            }

            DontDestroyOnLoad(this);

            if (autoInit)
                Init();
        }

        public void Init()
        {
            OnInit();
        }

        public void Free()
        {
            OnFree();
            _instance = null;
            Destroy(gameObject);
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnFree()
        {
        }
    }
}