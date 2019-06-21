namespace Zenject
{
    public static class DiContainerExtensions
    {
        public static void DeclareBrokableInstance<T>(this DiContainer container)
        {
            container.DeclareSignal<T>();
            container.Bind<IInstancePublisher<T>>().To<InstancePublisher<T>>().AsCached();
            container.Bind<IInstanceReceiver<T>>().To<InstanceReceiver<T>>().AsCached();
        }
    }
}