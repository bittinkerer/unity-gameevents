using Packages.Estenis.GameData_;
using UnityEngine;

namespace Packages.Estenis.GameEvent_
{
    [CreateAssetMenu(menuName = "GameEvent/GameEventGameData")]
    public class GameEventGameData : GameEvent<GameData>
    {
        //
        // Need this to support Unity input event system which requires no parameters
        public void Raise() =>
            base.Raise(new GameData()); // defaults to global event

        public void Raise(int instanceId) =>
            base.Raise(new GameData(instanceId));

        

    }
}
