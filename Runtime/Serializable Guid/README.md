<h1 align="center">Serializable Guid</h1>

## About

The Serializable Guid was created to facilitate the viewing of a Guid in the Inspector.
</br>
It allows for the viewing of the Guid as well as an in Inspector way to generate a new Guid or replace an existing one.

## Usage

### Creation

All of the normal Guid constructors are supported as well as a default constructor that will provide a SerializedGuid with an empty Guid.

```c#
public SerializableGuid(); // Creates empty Guid
public SerializableGuid(Guid guid);
public SerializableGuid(byte[] bytes);
public SerializableGuid(string guidString);
public SerializableGuid(int a, short b, short c, byte[] d);
public SerializableGuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k);
public SerializableGuid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k);
```

### Conversion

To allow for the best possible user experience the SerializedGuid allows for implicitly converting to and from a normal Syste.Guid and back again.

### Other Functionality

All of the other functionality that can be found in the System.Guid class has been mirrored into the Serialized version. This way this class can be used in exactly the same way as the original class.
