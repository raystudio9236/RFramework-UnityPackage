using UnityEngine;

namespace RFramework.Runtime.Component
{
    public class CompBase : MonoBehaviour, ICompLifeCycle
    {
        protected CompHost Host;

        public void SetHost(CompHost host)
        {
            Host = host;
        }

        public virtual void Init()
        {
        }

        public virtual void AfterInit()
        {
        }

        public virtual void BeforeFree()
        {
        }

        public virtual void Free()
        {
        }

        public virtual void OnUpdate(float dt)
        {
        }

        public virtual void OnFixedUpdate(float dt)
        {
        }
    }
}