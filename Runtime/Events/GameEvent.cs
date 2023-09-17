using Packages.Estenis.GameData_;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.Estenis.GameEvent_
{
    [CreateAssetMenu(menuName = "GameEvent/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        private readonly HashSet<Action> _actions =
            new HashSet<Action>();

        public void Raise()
        {
            // copying _actions locally means an action can Unregister event in the middle
            // of the foreach loop and not cause an exception, all actions will execute
            var actions = _actions.ToArray();
            foreach (var action in actions ?? Array.Empty<Action>())
            {
                action();
            }
        }

        public void Register(Action action) =>
            _actions.Add(action);

        public void Unregister(Action action) =>
            _actions.Remove(action);

        public void UnregisterAll() =>
            _actions.Clear();
    }

    public class GameEvent<T> : ScriptableObject where T: GameData
    {
        private readonly Dictionary<int, HashSet<Action<T>>> _handlers = new();

        public void Raise(T data)
        {
            // copying _actions locally means an action can Unregister event in the middle
            // of the foreach loop and not cause an exception; all actions will execute
            if(_handlers == null)
            {
                Debug.LogError($"{nameof(GameEvent)} is null, {data}");
                return;
            }
            if(data == null)
            {
                Debug.LogError($"{nameof(GameEvent)} data is null, {data}");
                return;
            }
            var handlers = _handlers
                .Where(a => a.Key == data.InstanceId || data.InstanceId == int.MinValue || a.Key == int.MinValue)
                .SelectMany(kv => kv.Value)
                .ToArray();
            foreach (var action in handlers ?? Array.Empty<Action<T>>())
            {
                action(data);
            }
        }

        public void Register(int clientId, Action<T> action)
        {
            if(!_handlers.ContainsKey(clientId))
            {
                _handlers[clientId] = new HashSet<Action<T>>();
            }
            _handlers[clientId].Add(action);
        }

        public void Unregister(int clientId, Action<T> action)
        {
            foreach (var item in _handlers.Where(kv => kv.Value.Any(a => a == action) && kv.Key == clientId).ToList())
            {
                _handlers[clientId].Remove(action);
            }
        }

        /// <summary>
        /// Unregister an action_listener from event for matching action_listener ONLY
        /// </summary>
        /// <param name="action"></param>
        public void Unregister(Action<T> action)
        {
            foreach (var item in _handlers.Where(kv => kv.Value.Any(a => a == action)).ToList())
            {
                _handlers[item.Key].Remove(action);
            }
        }

        public void UnregisterAll() =>
            _handlers.Clear();

        public bool IsActive(int clientId, Action<T> action) =>
            _handlers.Any(a => a.Key == clientId && a.Value.Any(a => a == action));

    }
}
