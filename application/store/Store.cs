using System;
using System.Collections.Generic;
using System.Linq;

namespace RepairService.Application.Store
{
    public static class Store
    {
        private static readonly Dictionary<string, object?> State = new();
        private static readonly Dictionary<string, List<Action<object?>>> Subscribers = new();

        public static void Set<T>(string key, T? value)
        {
            State[key] = value;
            if (Subscribers.TryGetValue(key, out var list))
                foreach (var sub in list.ToArray())
                    sub(value);
        }

        public static T? Get<T>(string key)
        {
            return State.TryGetValue(key, out var value) && value is T typed
                ? typed
                : default;
        }

        public static void Subscribe<T>(string key, Action<T?> handler)
        {
            if (!Subscribers.ContainsKey(key))
                Subscribers[key] = new List<Action<object?>>();
            Subscribers[key].Add(v => handler((T?)v));
        }

        public static void Clear(string key) => Set<object?>(key, null);
    }
}