# Changelog

## [1.0.0] - 2019-06-21

- Initial release :tada:

### Added

- `IInstancePublisher<T>`: Notify context some instances what spawned after resolving phase.
- `IInstanceReceiver<T>`:  Receive instances what spawned after resolving phase.
- `ZenjectInstancePublisher`: Automatically publish some instances controlled by ZenjectBinding.
- `DiContainer.DeclareBrokableInstance<T>()`: Provide method what to bind `IInstancePublisher<T>`, `IInstanceReceiver<T>` and declare signal of `T` as extension methods.
