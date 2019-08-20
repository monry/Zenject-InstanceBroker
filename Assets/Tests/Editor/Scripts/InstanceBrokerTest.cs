using NUnit.Framework;
using UniRx;

namespace Zenject
{
    public class InstanceBrokerTest : ZenjectUnitTestFixture
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
        public void Basic()
        {
            Container.DeclareBrokableInstance<TestClass>();
            Container.DeclareBrokableInstance<TestStruct>();
            Container.DeclareBrokableInstance<ITestInterface>();

            var classPublisher = Container.Resolve<IInstancePublisher<TestClass>>();
            var structPublisher = Container.Resolve<IInstancePublisher<TestStruct>>();
            var interfacePublisher = Container.Resolve<IInstancePublisher<ITestInterface>>();
            var classReceiver = Container.Resolve<IInstanceReceiver<TestClass>>();
            var structReceiver = Container.Resolve<IInstanceReceiver<TestStruct>>();
            var interfaceReceiver = Container.Resolve<IInstanceReceiver<ITestInterface>>();
            var testClass = new TestClass(10);
            var testStruct = new TestStruct(20);

            var counter = 0;
            var lastId = 0;
            classReceiver.Receive().Subscribe(_ => counter++);
            classReceiver.Receive().Subscribe(x => lastId = x.Id);
            structReceiver.Receive().Subscribe(_ => counter++);
            structReceiver.Receive().Subscribe(x => lastId = x.Id);
            interfaceReceiver.Receive().Subscribe(_ => counter++);
            interfaceReceiver.Receive().Subscribe(x => lastId = x.Id);

            classPublisher.Publish(testClass);
            Assert.AreEqual(10, lastId);

            structPublisher.Publish(testStruct);
            Assert.AreEqual(20, lastId);

            interfacePublisher.Publish(testClass);
            Assert.AreEqual(10, lastId);

            interfacePublisher.Publish(testStruct);
            Assert.AreEqual(20, lastId);

            classPublisher.Publish(new TestClass(100));
            Assert.AreEqual(100, lastId);

            structPublisher.Publish(new TestStruct(200));
            Assert.AreEqual(200, lastId);

            interfacePublisher.Publish(new TestClass(100));
            Assert.AreEqual(100, lastId);

            interfacePublisher.Publish(new TestStruct(200));
            Assert.AreEqual(200, lastId);

            classPublisher.Publish(testClass);
            Assert.AreEqual(10, lastId);

            structPublisher.Publish(testStruct);
            Assert.AreEqual(20, lastId);

            interfacePublisher.Publish(testClass);
            Assert.AreEqual(10, lastId);

            interfacePublisher.Publish(testStruct);
            Assert.AreEqual(20, lastId);

            // Do not change if published null value
            classPublisher.Publish(default);
            Assert.AreEqual(20, lastId);

            // Publish default value of struct
            structPublisher.Publish(default);
            Assert.AreEqual(0, lastId);

            // Do not change if published null value
            interfacePublisher.Publish(default);
            Assert.AreEqual(0, lastId);

            // Do not change if published null value
            classPublisher.Publish(null);
            Assert.AreEqual(0, lastId);

            // Do not change if published null value
            interfacePublisher.Publish(null);
            Assert.AreEqual(0, lastId);

            // null value does not fired
            Assert.AreEqual(13, counter);
        }
    }
}