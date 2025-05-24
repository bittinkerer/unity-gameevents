using UnityEngine;

namespace Packages.Estenis.GameEvent_ {
  [CreateAssetMenu( menuName = "GameEvent/GameEventObject" )]
  public class GameEventObject : GameEvent<object> {
#if DEBUG
    public string _goNameForManualTrigger;
#endif
  }
}
