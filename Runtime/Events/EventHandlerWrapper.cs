using System;

namespace Packages.Estenis.GameEvent_
{
    
    /// <summary>
    /// Allow EventHandler to be copied by reference
    /// rather than by value. 
    /// </summary>
    public class EventHandlerWrapper<T>
    {
        public event EventHandler<T> Handler;

        public void RemoveHandler(int instanceId)
        {
            var handlers = Handler?.GetInvocationList() ?? Array.Empty<Delegate>();                
            foreach (var handler in handlers)
            {
                if (handler.Target.GetHashCode() == instanceId)
                {
                    Handler -= (EventHandler<T>)handler;
                }
            }
        }

        public void Invoke(object sender, T eventArgs)
        {
            var handlers = Handler.GetInvocationList();
            Handler?.Invoke(sender, eventArgs);
        }
    }
}
