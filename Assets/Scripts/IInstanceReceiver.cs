using System;

namespace Zenject
{
    public interface IInstanceReceiver<out T>
    {
        IObservable<T> Receive();
    }
}