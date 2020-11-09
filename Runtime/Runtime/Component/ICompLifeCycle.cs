namespace RFramework.Runtime.Component
{
    public interface ICompLifeCycle
    {
        void Init();

        void AfterInit();

        void BeforeFree();

        void Free();

        void OnUpdate(float dt);

        void OnFixedUpdate(float dt);
    }
}