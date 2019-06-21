using NUnit.Framework;

namespace Zenject
{
    public class InstancePublisherTest : ZenjectUnitTestFixture
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
                .Bind<IInstancePublisher<int>>()
                .To<InstancePublisher<int>>()
                .AsCached();
            Container
                .DeclareSignal<int>()
                .OptionalSubscriber();
            var publisher = Container.Resolve<IInstancePublisher<int>>();
            Assert.DoesNotThrow(
                () =>
                {
                    publisher.Publish(100);
                    publisher.Publish(0);
                    publisher.Publish(-1);
                }
            );
        }

        [Test]
        public void Basic_Class()
        {
            Container
                .Bind<IInstancePublisher<TestClass>>()
                .To<InstancePublisher<TestClass>>()
                .AsCached();
            Container
                .DeclareSignal<TestClass>()
                .OptionalSubscriber();
            var publisher = Container.Resolve<IInstancePublisher<TestClass>>();
            Assert.DoesNotThrow(
                () =>
                {
                    var testClass = new TestClass(10);
                    publisher.Publish(testClass);
                    publisher.Publish(new TestClass(100));
                    publisher.Publish(testClass);
                    publisher.Publish(default);
                    publisher.Publish(null);
                }
            );
        }

        [Test]
        public void Basic_Struct()
        {
            Container
                .Bind<IInstancePublisher<TestStruct>>()
                .To<InstancePublisher<TestStruct>>()
                .AsCached();
            Container
                .DeclareSignal<TestStruct>()
                .OptionalSubscriber();
            var publisher = Container.Resolve<IInstancePublisher<TestStruct>>();
            Assert.DoesNotThrow(
                () =>
                {
                    var testStruct = new TestStruct(10);
                    publisher.Publish(testStruct);
                    publisher.Publish(new TestStruct(100));
                    publisher.Publish(testStruct);
                    publisher.Publish(default);
                }
            );
        }

        [Test]
        public void Basic_Interface()
        {
            Container
                .Bind<IInstancePublisher<ITestInterface>>()
                .To<InstancePublisher<ITestInterface>>()
                .AsCached();
            Container
                .DeclareSignal<ITestInterface>()
                .OptionalSubscriber();
            var publisher = Container.Resolve<IInstancePublisher<ITestInterface>>();
            Assert.DoesNotThrow(
                () =>
                {
                    var testClass = new TestClass(10);
                    var testStruct = new TestStruct(10);
                    publisher.Publish(testClass);
                    publisher.Publish(new TestClass(100));
                    publisher.Publish(testClass);
                    publisher.Publish(default);
                    publisher.Publish(null);
                    publisher.Publish(testStruct);
                    publisher.Publish(new TestStruct(100));
                    publisher.Publish(testStruct);
                    publisher.Publish(default);
                }
            );
        }

        [Test]
        public void Fire_Signal_Correctly()
        {
            Container
                .Bind<IInstancePublisher<TestClass>>()
                .To<InstancePublisher<TestClass>>()
                .AsCached();
            Container
                .Bind<IInstancePublisher<TestStruct>>()
                .To<InstancePublisher<TestStruct>>()
                .AsCached();
            Container
                .Bind<IInstancePublisher<ITestInterface>>()
                .To<InstancePublisher<ITestInterface>>()
                .AsCached();
            Container
                .DeclareSignal<TestClass>()
                .OptionalSubscriber();
            Container
                .DeclareSignal<TestStruct>()
                .OptionalSubscriber();
            Container
                .DeclareSignal<ITestInterface>()
                .OptionalSubscriber();
            var signalBus = Container.Resolve<SignalBus>();
            var classPublisher = Container.Resolve<IInstancePublisher<TestClass>>();
            var structPublisher = Container.Resolve<IInstancePublisher<TestStruct>>();
            var interfacePublisher = Container.Resolve<IInstancePublisher<ITestInterface>>();
            var testClass = new TestClass(10);
            var testStruct = new TestStruct(20);

            var counter = 0;
            var lastId = -1;
            signalBus.Subscribe<TestClass>(_ => counter++);
            signalBus.Subscribe<TestClass>(x => lastId = x.Id);
            signalBus.Subscribe<TestStruct>(_ => counter++);
            signalBus.Subscribe<TestStruct>(x => lastId = x.Id);
            signalBus.Subscribe<ITestInterface>(_ => counter++);
            signalBus.Subscribe<ITestInterface>(x => lastId = x.Id);

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

