using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Zenject
{
    [RequireComponent(typeof(ZenjectBinding))]
    [AddComponentMenu("Zenject/ZenjectInstancePublisher", 100)]
    public class ZenjectInstancePublisher : MonoBehaviour
    {
        private ZenjectBinding cachedZenjectBinding;
        private ZenjectBinding ZenjectBinding => cachedZenjectBinding ? cachedZenjectBinding : cachedZenjectBinding = GetComponent<ZenjectBinding>();

        [Inject] private DiContainer Container { get; }

        [Inject]
        private void Initialize()
        {
            foreach (var component in ZenjectBinding.Components)
            {
                switch (ZenjectBinding.BindType)
                {
                    case ZenjectBinding.BindTypes.Self:
                        InvokePublish(component.GetType(), component);
                        break;
                    case ZenjectBinding.BindTypes.AllInterfaces:
                        component
                            .GetType()
                            .GetInterfaces()
                            .ToList()
                            .ForEach(type => InvokePublish(type, component));
                        break;
                    case ZenjectBinding.BindTypes.AllInterfacesAndSelf:
                        component
                            .GetType()
                            .GetInterfaces()
                            .ToList()
                            .ForEach(type => InvokePublish(type, component));
                        InvokePublish(component.GetType(), component);
                        break;
                    case ZenjectBinding.BindTypes.BaseType:
                        if (component.GetType().BaseType != default(Type))
                        {
                            InvokePublish(component.GetType().BaseType, component);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void InvokePublish(Type type, object instance)
        {
            if (
                !(bool) Container
                    .GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .First(
                        mi =>
                            mi.MemberType == MemberTypes.Method
                            && mi.Name == "HasBinding"
                            && mi.IsGenericMethodDefinition
                            && mi.GetParameters().Length == 0
                    )
                    .MakeGenericMethod(typeof(IInstancePublisher<>).MakeGenericType(type))
                    .Invoke(Container, null)
            )
            {
                return;
            }

            var instancePublisher = Container
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(
                    mi =>
                        mi.MemberType == MemberTypes.Method
                        && mi.Name == "Resolve"
                        && mi.IsGenericMethodDefinition
                        && mi.GetParameters().Length == 0
                )
                .MakeGenericMethod(typeof(IInstancePublisher<>).MakeGenericType(type))
                .Invoke(Container, null);
            instancePublisher
                .GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(
                    mi =>
                        mi.Name.EndsWith("Publish") // Explicit method has prefix with interface name
                        && mi.GetParameters().Length == 1
                )
                .Invoke(
                    instancePublisher,
                    new[]
                    {
                        instance
                    }
                );
        }
    }
}