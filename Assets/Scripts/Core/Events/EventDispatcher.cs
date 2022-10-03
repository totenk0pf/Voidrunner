using System;
using System.Collections.Generic;
using Core.Patterns;
using UnityEngine;

namespace Core.Events {
    public class EventDispatcher : Singleton<EventDispatcher> {
        public Dictionary<EventType, Action<object>> _events = new Dictionary<EventType,Action<object>>();
        
        /// <summary>
        /// Add a listener to an event type.
        /// </summary>
        /// <param name="eventType">Type of event.</param>
        /// <param name="callback">Action to trigger on event.</param>
        public void AddListener(EventType eventType, Action<object> callback) {
            if (_events.ContainsKey(eventType)) {
                _events[eventType] += callback;
            }
            else {
                _events.Add(eventType, callback);
            }
        }

        /// <summary>
        /// Remove a listener from subscribing to an event.
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="callback"></param>
        public void RemoveListener(EventType eventType, Action<object> callback) {
            if (_events.ContainsKey(eventType)) {
                _events[eventType] -= callback;
            }
        }

        /// <summary>
        /// Trigger an event and its callbacks with parameters.
        /// </summary>
        /// <param name="eventType">Event to trigger.</param>
        /// <param name="param">Paramaters</param>
        public void FireEvent(EventType eventType, object param = null) {
            if (_events.ContainsKey(eventType)) {
                var actions = _events[eventType];
                if (actions == null) {
                    _events.Remove(eventType);
                    return;
                }
                actions.Invoke(param);
            }
        }

        public void ClearListeners() {
            _events.Clear();
        }
    }

    /// <summary>
    /// Extensions to help with calling event-related methods.
    /// </summary>
    public static class EventDispatcherExtension {
        public static void AddListener(this MonoBehaviour listener, EventType eventType, Action<object> callback) {
            EventDispatcher.Instance.AddListener(eventType, callback);
        }

        public static void RemoveListener(this MonoBehaviour listener, EventType eventType, Action<object> callback) {
            EventDispatcher.Instance.RemoveListener(eventType, callback);
        }

        public static void FireEvent(this MonoBehaviour listener, EventType eventType, object param = null) {
            EventDispatcher.Instance.FireEvent(eventType, param);
        }
    }
}