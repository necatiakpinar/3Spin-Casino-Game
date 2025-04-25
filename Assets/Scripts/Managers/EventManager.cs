using System;
using System.Collections.Generic;
using Enums;

namespace Managers
{
    public static class EventManager
    {
        private static Dictionary<string, List<Action<object[]>>> _observers = new();
        private static Dictionary<string, Func<object[], object>> _returnObservers = new();

        static EventManager()
        {
            _observers = new();
            _returnObservers = new();
        }

        public static void Subscribe<T>(ActionType eventName, Action<T[]> action)
        {
            if (!_observers.ContainsKey(eventName.ToString()))
            {
                _observers[eventName.ToString()] = new List<Action<object[]>>();
            }

            _observers[eventName.ToString()].Add(args => action(Array.ConvertAll(args, item => (T)item)));
        }

        public static void Subscribe<T>(FunctionType eventName, Func<T[], object> func)
        {
            _returnObservers[eventName.ToString()] = args => func(Array.ConvertAll(args, item => (T)item));
        }

        public static void Unsubscribe(ActionType eventName)
        {
            if (_observers.ContainsKey(eventName.ToString()))
            {
                _observers.Remove(eventName.ToString());
            }
        }

        public static void Unsubscribe(FunctionType eventName)
        {
            if (_returnObservers.ContainsKey(eventName.ToString()))
            {
                _returnObservers.Remove(eventName.ToString());
            }
        }

        public static void Notify(ActionType eventName, params object[] data)
        {
            if (_observers.TryGetValue(eventName.ToString(), out var observer))
            {
                foreach (var action in observer)
                {
                    action(data);
                }
            }
        }

        public static T NotifyWithReturn<T>(FunctionType eventName, params object[] data)
        {
            if (_returnObservers.ContainsKey(eventName.ToString()))
            {
                return (T)_returnObservers[eventName.ToString()](data);
            }

            return default(T);
        }
    
    }
}