using UnityEngine.Scripting;

namespace Zenject
{
    public class InstancePublisher<T> : IInstancePublisher<T>
    {
        [Preserve]
        public InstancePublisher()
        {
        }

        [Inject] private SignalBus SignalBus { get; }

        void IInstancePublisher<T>.Publish(T instance)
        {
            if (instance == null)
            {
                return;
            }
            SignalBus.Fire(instance);
        }
    }
}