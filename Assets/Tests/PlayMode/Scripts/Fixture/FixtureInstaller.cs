namespace Zenject.Fixture
{
    public class FixtureInstaller : MonoInstaller<FixtureInstaller>
    {
        public override void InstallBindings()
        {
            Installer.Install(Container);
        }

        public class Installer : Installer<Installer>
        {
            public override void InstallBindings()
            {
                SignalBusInstaller.Install(Container);

                Container.BindInterfacesTo<Receiver>().AsCached();

                Container.DeclareBrokableInstance<Publisher>();
                Container.DeclareBrokableInstance<IPublisher>();
            }
        }
    }
}