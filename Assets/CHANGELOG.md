# Changelog

## [1.0.6] - 2019-08-21

### Improvements

- Ignore publish if signals has not been declared

### Fixes

- Fix prefab path in runtime tests
- Remove unnecessary log

## [1.0.5] - 2019-08-20

### Features

- Annotate `AddComponentMenu`: `Zenject > ZenjectInstancePublisher`

## [1.0.4] - 2019-08-20

### Fixes

- Support Unity 2019.2

### Changes

- Rename Tests directories

## [1.0.3] - 2019-07-12

### Fixes

- Follow upgrades in `package.json`

## [1.0.2] - 2019-07-12

### Upgrade Dependencies

- Upgrade UniRx to v7.1.0

## [1.0.1] - 2019-06-22

### Fixed

- Fixed link to upm command repository in README.md

## [1.0.0] - 2019-06-22

- Initial release :tada:

### Added

- `IInstancePublisher<T>`: Notify context some instances what spawned after resolving phase.
- `IInstanceReceiver<T>`:  Receive instances what spawned after resolving phase.
- `ZenjectInstancePublisher`: Automatically publish some instances controlled by ZenjectBinding.
- `DiContainer.DeclareBrokableInstance<T>()`: Provide method what to bind `IInstancePublisher<T>`, `IInstanceReceiver<T>` and declare signal of `T` as extension methods.
