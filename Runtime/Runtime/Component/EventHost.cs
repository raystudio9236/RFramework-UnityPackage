using UnityEngine;
using RFramework.Common.Event;

namespace RFramework.Runtime.Component
{
    public class EventHost : MonoBehaviour
    {
        public EventDispatcher Dispatcher = new EventDispatcher();

        public void AddEventHandler(short type, EventHandler handler)
        {
            Dispatcher.AddHandler(type, handler);
        }

        public void RemoveEventHandler(short type, EventHandler handler)
        {
            Dispatcher.RemoveHandler(type, handler);
        }

        public void AddEventHandler<T>(short type, EventHandler<T> handler)
        {
            Dispatcher.AddHandler(type, handler);
        }

        public void RemoveEventHandler<T>(short type, EventHandler<T> handler)
        {
            Dispatcher.RemoveHandler(type, handler);
        }

        public void SendEvent(short type)
        {
            Dispatcher.Send(type);
        }

        public void SendEvent<T>(short type, T msg)
        {
            Dispatcher.Send(type, msg);
        }

        public EventDispatcher.EventData GetEventHandlerList(short type)
        {
            return Dispatcher.GetEventData(type);
        }

        public void ClearEvent()
        {
            Dispatcher.Clear();
        }

        public void PresizeEventHandler(short type, int size)
        {
            Dispatcher.PreCreate(type, size);
        }

        public void ClearNullEventHandler()
        {
            Dispatcher.ClearNull();
        }
    }
}