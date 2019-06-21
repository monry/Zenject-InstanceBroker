namespace Zenject
{
    public interface IInstancePublisher<in T>
    {
        void Publish(T instance);
    }
}