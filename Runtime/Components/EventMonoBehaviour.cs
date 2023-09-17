using UnityEngine;
using Packages.Estenis.UnityExts_;

namespace Packages.Estenis.GameEvent_
{
    public class EventMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private bool _isGlobal;

        private int _eventId = -1;

        protected virtual void OnEnable()
        {
            _eventId = -1;
        }


        public int EventId { 
            get
            {
                if (_eventId == -1)
                {
                    _eventId = _isGlobal ? int.MinValue : this.gameObject.GetRoot().GetHashCode();
                }
                return _eventId;
            }
        }

        

    }
}
