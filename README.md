<h1 align="center">Serializables</h1>

<div align="center">

[![GitHub Issues](https://img.shields.io/github/issues/ChrisHargrove/Serializables?style=flat-square)](https://github.com/ChrisHargrove/Serializables/issues)
[![GitHub Pull Requests](https://img.shields.io/github/issues-pr/ChrisHargrove/Serializables?style=flat-square)](https://github.com/ChrisHargrove/Serializables/pulls)
[![License](https://img.shields.io/github/license/ChrisHargrove/Serializables?style=flat-square)](/LICENSE)

</div>

---

<p align="center"> This project aims to wrap all the non-serialisable types in the Unity Engine so that that can be viewed in the Inspector and so that they can be saved into prefabs, scenes etc.
    <br> 
</p>

## 📝 Table of Contents

- [About](#about)
- [Installation](#installation)
- [Current Serializable Types](#serializable_types)
- [Contributing](#contributing)
- [TODO](../TODO.md)
- [Contributing](../CONTRIBUTING.md)
- [Authors](#authors)

## 🧐 About <a name = "about"></a>

Many of the available C# data structures available are not immediately serialisable in the Unity Inspector. This is incredibly annoying. </br>
</br>This project aims to remedy this fact by creating wrapper classes that define how the data will be morphed during the serialization/deserialization process, as well as creating custom property drawers that define how the data types are going to be displayed to the user in the Inspector.

## 🚀 Installation <a name="installation"></a>

To install the package into your Unity project there are 3 possible routes:

1. Add the git url to the manifest.json in your unity project.

    ```json
    {
     "dependencies": {
            "com.unquestionablegames.serializables": "git@github.com:ChrisHargrove/Serializables.git",
        ...
        }
    }
    ```

2. Open the Unity Package Manager and add the git url there. [How To](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

3. Download the Unity .unitypackage from the Repositories Releases section [Here](https://github.com/ChrisHargrove/Serializables/releases). Whilst the project is open, double click the .unityproject and inport the package.


## 🎈 Current Serializable Types <a name="serializable_types"></a>

- [Serializable Guid](https://github.com/ChrisHargrove/Serializables/tree/main/Runtime/Serializable%20Guid)
- [Serializable Dictionary](https://github.com/ChrisHargrove/Serializables/tree/main/Runtime/Serializable%20Dictionary) (with List&lt;T&gt; as value.)

## 📚 Contributing <a name = "contributing"></a>

To setup a development environment to start contributing follow the following steps:</br>

1. Start a new Unity project.
2. Delete all the items in the Assets folder.
3. Clone the repo into the Assets folder.

### Prerequisites

- Unity 2019.3 (or higher)
- VS Code or other Integrated Development Environment

## ✍️ Authors <a name = "authors"></a>

- [@Chris Hargrove](https://github.com/ChrisHargrove)

See also the list of [contributors](https://github.com/kylelobo/The-Documentation-Compendium/contributors) who participated in this project.
