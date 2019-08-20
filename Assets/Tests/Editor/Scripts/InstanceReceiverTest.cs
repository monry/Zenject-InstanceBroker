using NUnit.Framework;
using UniRx;

namespace Zenject
{
    public class InstanceReceiverTest : ZenjectUnitTestFixture
    {
        private interface ITestInterface
        {
            int Id { get; }
        }

        private class TestClass : ITestInterface
        {
            public TestClass(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }

        private struct TestStruct : ITestInterface
        {
            public TestStruct(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }

        [SetUp]
        public void SetUp()
        {
            SignalBusInstaller.Install(Container);
        }

        [Test]
        public void Basic_Primitive()
        {
            Container
                .Bind<IInstanceReceiver<int>>()
                .To<InstanceReceiver<int>>()
                .AsCached();
            Container
                .DeclareSignal<int>();

            var signalBus = Container.Resolve<SignalBus>();
            var receiver = Container.Resolve<IInstanceReceiver<int>>();

            var lastValue = 0;
            receiver.Receive().Subscribe(x => lastValue = x);

            Assert.AreEqual(0, lastValue);

            signalBus.Fire(100);
            Assert.AreEqual(100, lastValue);

            signalBus.Fire(-1);
            Assert.AreEqual(-1, lastValue);

            signalBus.Fire<int>(default);
            Assert.AreEqual(0, lastValue);
        }

        [Test]
        public void Basic_Class()
        {
            Container
                .Bind<IInstanceReceiver<TestClass>>()
                .To<InstanceReceiver<TestClass>>()
                .AsCached();
            Container
                .DeclareSignal<TestClass>()
                .OptionalSubscriber();

            var signalBus = Container.Resolve<SignalBus>();
            var receiver = Container.Resolve<IInstanceReceiver<TestClass>>();

            var lastId = 0;
            receiver.Receive().Subscribe(x => lastId = x.Id);

            var testClass = new TestClass(10);

            signalBus.Fire(testClass);
            Assert.AreEqual(10, lastId);

            signalBus.Fire(new TestClass(100));
            Assert.AreEqual(100, lastId);

            signalBus.Fire(testClass);
            Assert.AreEqual(10, lastId);
        }

        [Test]
        public void Basic_Struct()
        {
            Container
                .Bind<IInstanceReceiver<TestStruct>>()
                .To<InstanceReceiver<TestStruct>>()
                .AsCached();
            Container
                .DeclareSignal<TestStruct>();

            var signalBus = Container.Resolve<SignalBus>();
            var receiver = Container.Resolve<IInstanceReceiver<TestStruct>>();

            var lastId = 0;
            receiver.Receive().Subscribe(x => lastId = x.Id);

            var testStruct = new TestStruct(10);

            signalBus.Fire(testStruct);
            Assert.AreEqual(10, lastId);

            signalBus.Fire(new TestStruct(100));
            Assert.AreEqual(100, lastId);

            signalBus.Fire(testStruct);
            Assert.AreEqual(10, lastId);

            signalBus.Fire<TestStruct>(default);
            Assert.AreEqual(0, lastId);
        }

        [Test]
        public void Basic_Interface()
        {
            Container
                .Bind<IInstanceReceiver<ITestInterface>>()
                .To<InstanceReceiver<ITestInterface>>()
                .AsCached();
            Container
                .DeclareSignal<ITestInterface>();

            var signalBus = Container.Resolve<SignalBus>();
            var receiver = Container.Resolve<IInstanceReceiver<ITestInterface>>();

            var lastId = 0;
            receiver.Receive().Subscribe(x => lastId = x.Id);

            var testClass = new TestClass(10);
            var testStruct = new TestStruct(10);

            signalBus.Fire<ITestInterface>(testClass);
            Assert.AreEqual(10, lastId);

            signalBus.Fire<ITestInterface>(new TestClass(100));
            Assert.AreEqual(100, lastId);

            signalBus.Fire<ITestInterface>(testClass);
            Assert.AreEqual(10, lastId);

            signalBus.Fire<ITestInterface>(testStruct);
            Assert.AreEqual(10, lastId);

            signalBus.Fire<ITestInterface>(new TestStruct(100));
            Assert.AreEqual(100, lastId);

            signalBus.Fire<ITestInterface>(testStruct);
            Assert.AreEqual(10, lastId);
        }
    }
}