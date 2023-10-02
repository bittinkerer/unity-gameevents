using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.Estenis.GameEvent_
{

    [CreateAssetMenu(menuName = "GameEvent/GenericGameEvent")]
    public class GameEvent<T> : ScriptableObject
    {
        private Dictionary<int, HashSet<Action<object, T>>> _handlers; 

        private void OnEnable()
        {
            _handlers = new Dictionary<int, HashSet<Action<object, T>>>();
        }

        public void Raise(int instanceId, object sender, T data)
        {
            // copying _actions locally means an action can Unregister event in the middle
            // of the foreach loop and not cause an exception; all actions will execute
            if(_handlers == null)
            {
                Debug.LogError($"{nameof(GameEvent<T>)} is null, {data}");
                return;
            }
            
            var handlers = _handlers
                .Where(a => instanceId == a.Key || instanceId == int.MinValue || a.Key == int.MinValue)
                .SelectMany(kv => kv.Value)
                .ToArray();
            foreach (var handler in handlers ?? Array.Empty<Action<object, T>>())
            {
                handler(sender, data);
            }
        }

        public void Register(int instanceId, Action<object, T> action)
        {
            if(!_handlers.ContainsKey(instanceId))
            {
                _handlers[instanceId] = new HashSet<Action<object, T>>();
            }
            _handlers[instanceId].Add(action);
        }

        public void Unregister(int instanceId, Action<object, T> action)
        {
            foreach (var item in _handlers.Where(kv => kv.Value.Any(a => a == action) && kv.Key == instanceId).ToList())
            {
                _handlers[instanceId].Remove(action);
            }
        }

        /// <summary>
        /// Unregister an action_listener from event for matching action_listener ONLY
        /// </summary>
        /// <param name="action"></param>
        public void Unregister(Action<object, T> action)
        {
            foreach (var item in _handlers.Where(kv => kv.Value.Any(a => a == action)).ToList())
            {
                _handlers[item.Key].Remove(action);
            }
        }

        public void UnregisterAll() =>
            _handlers.Clear();

        public bool IsActive(int clientId, Action<object, T> action) =>
            _handlers.Any(a => a.Key == clientId && a.Value.Any(a => a == action));

    }
}
