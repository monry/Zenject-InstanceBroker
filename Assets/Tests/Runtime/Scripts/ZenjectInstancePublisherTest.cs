using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject.Fixture;

namespace Zenject
{
    public class ZenjectInstancePublisherTest : ZenjectIntegrationTestFixture
    {
        [SetUp]
        public void SetUp()
        {
            PreInstall();

            FixtureInstaller.Installer.Install(Container);
        }

        [UnityTest]
        public IEnumerator Fundamental()
        {
            var sceneContext = Object.FindObjectOfType<SceneContext>();
            var zenjectBinding = sceneContext.gameObject.AddComponent<ZenjectBinding>();
            var publisher = sceneContext.gameObject.AddComponent<Publisher>();
            var zenjectInstancePublisher = sceneContext.gameObject.AddComponent<ZenjectInstancePublisher>();
            zenjectBinding.GetType().GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(zenjectBinding, new [] { publisher });
            zenjectBinding.GetType().GetField("_useSceneContext", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(zenjectBinding, true);
            zenjectBinding.GetType().GetField("_bindType", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(zenjectBinding, ZenjectBinding.BindTypes.AllInterfacesAndSelf);
            Container.QueueForInject(zenjectInstancePublisher);

            var receiver = Container.Resolve<IReceiver>();
            var classObservable = receiver.ReceiveClass();
            var interfaceObservable = receiver.ReceiveInterface();

            PostInstall();

            var counter = 0;
            var lastValueForClass = 0;
            var lastValueForInterface = 0;

            classObservable.Subscribe(_ => counter++);
            classObservable.Subscribe(x => lastValueForClass = x.Value);
            interfaceObservable.Subscribe(_ => counter++);
            interfaceObservable.Subscribe(x => lastValueForInterface = x.Value);

            Assert.AreEqual(10, lastValueForClass);
            Assert.AreEqual(10, lastValueForInterface);
            Assert.AreEqual(2, counter);

            yield break;
        }

        [UnityTest]
        public IEnumerator PublishDynamically()
        {
            Container
                .Bind<Publisher>()
                .WithId("Self")
                .FromComponentInNewPrefab(AssetDatabase.LoadAssetAtPath<Publisher>("Assets/Tests/PlayMode/Prefabs/SelfPublisher.prefab"))
                .AsCached();
            Container
                .Bind<IPublisher>()
                .WithId("Interface")
                .FromComponentInNewPrefab(AssetDatabase.LoadAssetAtPath<Publisher>("Assets/Tests/PlayMode/Prefabs/InterfacePublisher.prefab"))
                .AsCached();
            Container
                .Bind(typeof(Publisher), typeof(IPublisher))
                .WithId("InterfaceAndSelf")
                .FromComponentInNewPrefab(AssetDatabase.LoadAssetAtPath<Publisher>("Assets/Tests/PlayMode/Prefabs/InterfaceAndSelfPublisher.prefab"))
                .AsCached();

            var receiver = Container.Resolve<IReceiver>();
            var classObservable = receiver.ReceiveClass();
            var interfaceObservable = receiver.ReceiveInterface();

            PostInstall();

            var counter = 0;
            var lastValueForClass = 0;
            var lastValueForInterface = 0;

            classObservable.Subscribe(_ => counter++);
            classObservable.Subscribe(x => lastValueForClass = x.Value);
            interfaceObservable.Subscribe(_ => counter++);
            interfaceObservable.Subscribe(x => lastValueForInterface = x.Value);

            Container.ResolveId<Publisher>("Self");

            Assert.AreEqual(20, lastValueForClass);
            Assert.AreEqual(0, lastValueForInterface);
            Assert.AreEqual(1, counter);

            Container.ResolveId<IPublisher>("Interface");

            Assert.AreEqual(20, lastValueForClass);
            Assert.AreEqual(30, lastValueForInterface);
            Assert.AreEqual(2, counter);

            Container.ResolveId<Publisher>("InterfaceAndSelf");
            Container.ResolveId<IPublisher>("InterfaceAndSelf");

            Assert.AreEqual(40, lastValueForClass);
            Assert.AreEqual(40, lastValueForInterface);
            Assert.AreEqual(4, counter);

            yield break;
        }
    }
}

