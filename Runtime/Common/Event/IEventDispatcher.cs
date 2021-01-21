namespace RFramework.Common.Event
{
    /// <summary>
    /// 事件派发器接口
    /// 实现该接口即可只用对应类直接进行事件操作
    /// </summary>
    public interface IEventDispatcher
    {
        EventDispatcher EventDispatcher { get; }

        void AddEventHandler(short type, EventHandler handler);

        void AddEventHandler<T>(short type, EventHandler<T> handler);

        void RemoveEventHandler(short type, EventHandler handler);

        void RemoveEventHandler<T>(short type, EventHandler<T> handler);

        void SendEvent(short type);

        void SendEvent<T>(short type, T msg);
    }
}