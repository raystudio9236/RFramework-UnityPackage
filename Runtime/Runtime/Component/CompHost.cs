using System;
using UnityEngine;

namespace RFramework.Runtime.Component
{
    public class CompHost : EventHost
    {
        public CompBase[] Comps;

#if UNITY_EDITOR
        private void OnValidate()
        {
            Comps = GetComponents<CompBase>();
        }
#endif

        public T GetComp<T>() where T : CompBase
        {
            if (Comps == null) return default;

            foreach (var comp in Comps)
            {
                if (comp.GetType() == typeof(T))
                    return (T) comp;
            }

            return default;
        }

        public CompBase GetComp(Type type)
        {
            if (Comps == null) return null;

            foreach (var comp in Comps)
            {
                if (comp.GetType() == type)
                    return comp;
            }

            return null;
        }

        protected virtual void Awake()
        {
            if (Comps != null)
            {
                foreach (var c in Comps)
                {
                    c.SetHost(this);
                    c.Init();
                }
            }
        }

        protected virtual void Start()
        {
            if (Comps != null)
            {
                foreach (var c in Comps)
                {
                    c.AfterInit();
                }
            }
        }

        protected virtual void Update()
        {
            if (Comps != null)
            {
                foreach (var c in Comps)
                {
                    c.OnUpdate(Time.deltaTime);
                }
            }
        }

        protected void FixedUpdate()
        {
            if (Comps != null)
            {
                foreach (var c in Comps)
                {
                    c.OnFixedUpdate(Time.fixedDeltaTime);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (Comps != null)
            {
                foreach (var c in Comps)
                {
                    c.BeforeFree();
                }

                foreach (var c in Comps)
                {
                    c.Free();
                    c.SetHost(null);
                }
            }

            Dispatcher.Clear();
        }
    }
}