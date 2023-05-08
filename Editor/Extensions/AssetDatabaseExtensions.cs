using UnityEditor;

namespace BatteryAcid.Serializables.Editor
{
    internal static class AssetDatabaseExtensions
    {
        public static T LoadFirstAsset<T>(string name)
            where T : UnityEngine.Object
        {
            string[] assets = AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}");
            if (assets.Length == 0)
            {
                return default;
            }
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assets[0]));
        }
    }
}
