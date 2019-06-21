namespace Zenject
{
    public class InstancePublisher<T> : IInstancePublisher<T>
    {
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