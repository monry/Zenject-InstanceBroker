namespace Zenject
{
    public static class DiContainerExtensions
    {
        public static DeclareSignalRequireHandlerAsyncTickPriorityCopyBinder DeclareBrokableInstance<T>(this DiContainer container)
        {
            container.Bind<IInstancePublisher<T>>().To<InstancePublisher<T>>().AsCached();
            container.Bind<IInstanceReceiver<T>>().To<InstanceReceiver<T>>().AsCached();
            return container.DeclareSignal<T>();
        }
    }
}