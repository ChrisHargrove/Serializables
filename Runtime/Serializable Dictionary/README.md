<h1 align="center">Serializable Dictionary</h1>

## About

The SerializableDictionary has been created to facilitate the viewing of the C# Dictionary Data Structure as by default Unity does not support the serialization of this data type.<br>
One of the major considerations whilst creating this data structure was to avoid the solution that has been seen on the asset store previously where a custom property drawer has to be defined on a per defined dictionary type.<br>
This means that we can now just declare a SerializableDictionary and it will get automatically serialized and displayed.

## Usage <a name = "usage"></a>

### Creation

Declaring a SerializableDictionary is as simple as a regular C# Dictionary...

```c#
public SerializableDictionary<int, string> TestDictionary = new SerializableDictionary();
```

### Inspector Display

Depending on the type of key used will change how the Inspector will show the SerializableDictionary. For example if the Key type is an Enum then when the SerializableDictionary is serialized the system ensures that there is a value in the dictionary for each enum value, and if a value wasnt provided for that key a defualt value for the value type is provided.

SerializableDictionary will also not allow the addition or removal of extra elements if the Key is an Enum.
