using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.Estenis.GameEvent_ {

  [CreateAssetMenu( menuName = "GameEvent/GenericGameEvent" )]
  public class GameEvent<T> : ScriptableObject {
    public static int GlobalId => int.MinValue;
    private Dictionary<int, HashSet<ActionWrapper<GameObject, T>>> _handlers;

    private void OnEnable( ) {
      _handlers = new Dictionary<int, HashSet<ActionWrapper<GameObject, T>>>();
    }

    /// <summary>
    /// Raise global event with no sender or data.
    /// </summary>
    public void Raise( ) {
      Raise( int.MinValue, null, default );
    }

    public void Raise( int instanceId, GameObject sender, T data ) {
      // copying _actions locally means an action can Unregister event in the middle
      // of the foreach loop and not cause an exception; all actions will execute
      if ( _handlers == null ) {
        Debug.LogError( $"{nameof( GameEvent<T> )} is null, {data}" );
        return;
      }

      var handlers = _handlers
                .Where(a => instanceId == a.Key || instanceId == int.MinValue || a.Key == int.MinValue)
                .SelectMany(kv => kv.Value)
                .ToArray();
      foreach ( var handler in handlers ?? Array.Empty<ActionWrapper<GameObject, T>>() ) {
        ( (Action<GameObject, T>) handler )( sender, data );
      }
    }

    public void Register( int instanceId, ActionWrapper<GameObject, T> action ) {
      if ( !_handlers.ContainsKey( instanceId ) ) {
        _handlers[instanceId] = new HashSet<ActionWrapper<GameObject, T>>();
      }
      _handlers[instanceId].Add( action );
    }

    public void Unregister( int instanceId, ActionWrapper<GameObject, T> action ) {
      var handlers = _handlers
                        .Where(kv => (kv.Key == instanceId || instanceId == GlobalId) && kv.Value.Any(a => a == action))
                        .ToList() ?? new List<KeyValuePair<int, HashSet<ActionWrapper<GameObject, T>>>>();
      foreach ( var item in handlers ) {
        _handlers[instanceId].Remove( action );
      }
    }

    public void UnregisterAll( ) =>
        _handlers.Clear();

    public bool IsActive( int clientId, ActionWrapper<object, T> action ) =>
        _handlers.Any( a => a.Key == clientId && a.Value.Any( a => a == action ) );

  }
}