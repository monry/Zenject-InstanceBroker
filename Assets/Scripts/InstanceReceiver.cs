using System;
using JetBrains.Annotations;
using UniRx;

namespace Zenject
{
    public class InstanceReceiver<T> : IInstanceReceiver<T>
    {
        [UsedImplicitly]
        public InstanceReceiver() : this(new ReplaySubject<T>())
        {
        }

        public InstanceReceiver(ISubject<T> subject)
        {
            Subject = subject;
        }

        private ISubject<T> Subject { get; }

        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            signalBus.Subscribe<T>(x => Subject.OnNext(x));
        }

        IObservable<T> IInstanceReceiver<T>.Receive()
        {
            return Subject.Where(x => x != null);
        }
    }
}