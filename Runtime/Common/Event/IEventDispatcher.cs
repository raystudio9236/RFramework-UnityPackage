namespace RFramework.Common.Event
{
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