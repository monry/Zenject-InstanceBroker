# Zenject InstanceBroker

Publish and receive some instances what spawned after resolving phase. 

## Installation

```bash
upm add package dev.monry.upm.zenject-instancebroker
```

Note: [`upm`](https://github.com/upm-packages/upm) command is command line interface for Unity Package Manager

## Usages

### Prepare

Add reference to `Zenject.InstanceBroker.asmdef` in your project's asmdef.

### Bind types and Declare signals

```C#
using Zenject;

public class SampleInstaller : Installer<SampleInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareBrokableInstance<PublishSample>();
    }
}
```

`DiContainer.DeclareBrokableInstance<T>()` binds types and declare signals.

- Bind below types
    - `IInstancePublisher<T>`
    - `IInstanceReceiver<T>`
- Declare signal of `T`

### Publish

```C#
using UnityEngine;
using Zenject;

public class Sample : MonoBehaviour
{
    [Inject] private IInstancePublisher<Sample> Publisher { get; }

    [Inject]
    private void Initialize()
    {
        Publisher.Publish(this);
    }
    
    public void Foo()
    {
        Debug.Log("Foo");
    }
}
```

Invoke `IInstancePublisher<T>.Publish()` after constructed.

### Receive

```C#
using Zenject;
using UniRx;

public class ReceiveSample : IInitializable
{
    [Inject] private IInstanceReceiver<Sample> Receiver { get; }
    
    public void Initialize()
    {
        Receiver.Receive().Subscribe(x => x.Foo()); 
    }
}
```

Will stream instance of `Sample` when instantiated it.

### `ZenjectInstancePublisher`

<img width="360" alt="ZenjectInstancePublisher" src="https://user-images.githubusercontent.com/838945/59932297-d662c500-9481-11e9-8554-15d4881891f9.png">

Invoke `IInstancePublisher<T>.Publish()` automatically according to the settings of `ZenjectBinding`.

1. Attach `ZenjectInstancePublisher` to a GameObject that contains the Component you want to publish
1. Drag &amp; Drop Component you want to publish into `Components` field on `ZenjectBinding`
1. You can limit the scope in which instances are exchanged using `Context` field of `ZenjectBinding`
1. Change `Bind Type` field of `ZenjectBinding` if you want to publish other than instance of Component itself such as interface
