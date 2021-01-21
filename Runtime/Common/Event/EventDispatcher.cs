using System;
using System.Collections.Generic;
using RFramework.Common.Log;

namespace RFramework.Common.Event
{
    using EventDic = Dictionary<short, EventDispatcher.EventData>;
    using EventDataList = List<EventDispatcher.EventData>;

    /// <summary>
    /// 无参数的事件回调
    /// </summary>
    /// <param name="eventType">事件类型</param>
    public delegate void EventHandler(short eventType);

    /// <summary>
    /// 带参数的事件回调
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="data">参数</param>
    /// <typeparam name="T">参数类型</typeparam>
    public delegate void EventHandler<in T>(short eventType, T data);

    /// <summary>
    /// 事件派发器
    /// </summary>
    public class EventDispatcher
    {
        /// <summary>
        /// 事件数据
        /// </summary>
        public class EventData
        {
            /// <summary>
            /// 事件类型
            /// </summary>
            public short EventType { get; }

            private readonly List<Delegate> _handlers;

            public EventData(short eventType, int size = 4)
            {
                EventType = eventType;
                _handlers = new List<Delegate>(size);
            }

            /// <summary>
            /// 发送事件
            /// </summary>
            public void Send()
            {
                foreach (var handler in _handlers)
                {
                    if (handler == null)
                        continue;

                    var eventHandler = (EventHandler) handler;
                    eventHandler(EventType);
                }
            }

            /// <summary>
            /// 发送事件，带参数
            /// </summary>
            /// <param name="msgData">参数</param>
            /// <typeparam name="T">参数类型</typeparam>
            public void Send<T>(T msgData)
            {
                foreach (var handler in _handlers)
                {
                    if (handler == null)
                        continue;

                    var eventHandler = (EventHandler<T>) handler;
                    eventHandler(EventType, msgData);
                }
            }

            /// <summary>
            /// 清理事件回调列表
            /// </summary>
            public void Clear()
            {
                _handlers.Clear();
            }

            /// <summary>
            /// 清理事件回调列表中空回调
            /// </summary>
            public void ClearNull()
            {
                var len = _handlers.Count;
                var newLen = 0;
                for (var i = 0; i < len; i++)
                {
                    if (_handlers[i] == null)
                        continue;

                    if (newLen != i)
                        _handlers[newLen] = _handlers[i];

                    newLen++;
                }

                _handlers.RemoveRange(newLen, len - newLen);
            }

            /// <summary>
            /// 尝试添加回调
            /// </summary>
            /// <param name="handler">回调</param>
            /// <returns>是否添加成功</returns>
            public bool TryAdd(Delegate handler)
            {
                var newEvent = true;

#if DEBUG
                if (_handlers.Contains(handler))
                {
                    RLog.LogError($"重复添加事件 {handler}");
                    newEvent = false;
                }
#endif

                if (newEvent)
                {
                    _handlers.Add(handler);
                }

                return newEvent;
            }

            /// <summary>
            /// 尝试移除事件回调
            /// </summary>
            /// <param name="handler">回调</param>
            /// <returns>是否移除成功</returns>
            public bool TryRemove(Delegate handler)
            {
                var index = _handlers.IndexOf(handler);
                if (index != -1)
                {
                    _handlers[index] = null;
                    return true;
                }

                return false;
            }
        }

        private readonly EventDic _eventDatas = new EventDic();

        private readonly EventDataList _waitClearList = new EventDataList();

        /// <summary>
        /// 添加事件回调
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="handler">回调</param>
        public void AddHandler(short type, EventHandler handler)
        {
            InnerAddHandler(type, handler);
        }

        /// <summary>
        /// 添加带参数事件回调
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="handler">回调</param>
        /// <typeparam name="T">参数类型</typeparam>
        public void AddHandler<T>(short type, EventHandler<T> handler)
        {
            InnerAddHandler(type, handler);
        }

        /// <summary>
        /// 移除事件回调
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="handler">回调</param>
        public void RemoveHandler(short type, EventHandler handler)
        {
            InnerRemoveHandler(type, handler);
        }

        /// <summary>
        /// 移除带参数事件回调
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="handler">回调</param>
        /// <typeparam name="T">参数类型</typeparam>
        public void RemoveHandler<T>(short type, EventHandler<T> handler)
        {
            InnerRemoveHandler(type, handler);
        }

        /// <summary>
        /// 发送无参数事件
        /// </summary>
        /// <param name="type">事件类型</param>
        public void Send(short type)
        {
            var eventData = GetEventData(type);
            eventData?.Send();
        }

        /// <summary>
        /// 发送带参数事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="msgData">参数</param>
        /// <typeparam name="T">参数类型</typeparam>
        public void Send<T>(short type, T msgData)
        {
            var eventData = GetEventData(type);
            eventData?.Send(msgData);
        }

        /// <summary>
        /// 获取某一个事件对应的事件数据
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="autoCreate">如果当前没有是否自动创建</param>
        /// <returns>事件数据</returns>
        public EventData GetEventData(short type, bool autoCreate = false)
        {
            if (!_eventDatas.TryGetValue(type, out var ret) && autoCreate)
            {
                ret = new EventData(type);
                _eventDatas.Add(type, ret);
            }

            return ret;
        }

        /// <summary>
        /// 清理所有事件回调
        /// </summary>
        public void Clear()
        {
            foreach (var eventData in _eventDatas)
            {
                eventData.Value.Clear();
            }

            _eventDatas.Clear();
        }

        /// <summary>
        /// 清理所有事件回调列表中的空回调
        /// </summary>
        public void ClearNull()
        {
            foreach (var eventData in _waitClearList)
            {
                eventData.ClearNull();
            }

            _waitClearList.Clear();
        }

        /// <summary>
        /// 预创建事件回调列表
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="size">预创建回调个数</param>
        public void PreCreate(short type, int size)
        {
            if (!_eventDatas.ContainsKey(type))
                _eventDatas.Add(type, new EventData(type, size));
        }

        private void InnerAddHandler(short type, Delegate handler)
        {
            var eventData = GetEventData(type, true);
            eventData?.TryAdd(handler);
        }

        private void InnerRemoveHandler(short type,
            Delegate handler)
        {
            var eventData = GetEventData(type);
            if (eventData == null) return;

            if (eventData.TryRemove(handler))
            {
                if (_waitClearList.Contains(eventData))
                    _waitClearList.Add(eventData);
            }
        }
    }
}