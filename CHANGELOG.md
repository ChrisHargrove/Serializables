# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog][Keep a Changelog] and this project adheres to [Semantic Versioning][Semantic Versioning].

## [Unreleased]

---

## [Released]

## [0.2.0] - 2021-07-30

### Added

- SerializableQueue that allows for C# Queue&lt;T&gt; to be seen in the Unity Inspector.
- SerializableQueuePropertyDrawer that defines how to draw the SerializableQueue.
- SerializableStack that allows for C# Stack&lt;T&gt; to be seen in the Unity Inspector.
- SerializableStackPropertyDrawer that defines how to draw the SerializableStack.
- SerializableHashSet that allows for C# HashSet&lt;T&gt; to be seen in the Unity Inspector.
- SerializableHashSetPropertyDrawer that defines how to draw the SerializableHashSet.
- SerializableUri that allows for C# Uri to be seen in the Unity Inspector.
- SerializableUriPropertyDrawer that defines how to draw the SerializableUri.

### Changed

- Changed the SerializableDictionaryConflict to inherit from SerializableConflict base class that can be used for other data structures that can have conflicts that do not use a key.

## [0.1.0] - 2021-07-28

### Added

- SerializableGuid that allows for Guid's to be serialized in Unity Inspector.
- SerializableGuidPropertyDrawer that defines how to draw the SerializableGuid.
- SerializableDictionary that allows for Dictionaries to be serialized by Unity Inspector.
- SerializableDictionaryPropertyDrawer that defines how to draw the SerializableDictionary.
- package.json to allow for git url package, and use inside Unity's package manager.
- CHANGELOG.md for keeping track of changes.
- README.md for explaining how to use the package.
- LISCENCE.md holds the projects liscence information.

---

<!-- Links -->
[Keep a Changelog]: https://keepachangelog.com/
[Semantic Versioning]: https://semver.org/

<!-- Versions -->
[Released]: https://github.com/ChrisHargrove/Serializables/releases
[0.2.0]: https://github.com/ChrisHargrove/Serializables/releases/v0.2.0
[0.1.0]: https://github.com/ChrisHargrove/Serializables/releases/v0.1.0
