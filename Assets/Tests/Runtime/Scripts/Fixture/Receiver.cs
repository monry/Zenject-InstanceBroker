using System;

namespace Zenject.Fixture
{
    public interface IReceiver
    {
        IObservable<Publisher> ReceiveClass();
        IObservable<IPublisher> ReceiveInterface();
    }

    public class Receiver : IReceiver
    {
        [Inject] private IInstanceReceiver<Publisher> ClassReceiver { get; }
        [Inject] private IInstanceReceiver<IPublisher> InterfaceReceiver { get; }

        IObservable<Publisher> IReceiver.ReceiveClass()
        {
            return ClassReceiver.Receive();
        }

        IObservable<IPublisher> IReceiver.ReceiveInterface()
        {
            return InterfaceReceiver.Receive();
        }
    }
}