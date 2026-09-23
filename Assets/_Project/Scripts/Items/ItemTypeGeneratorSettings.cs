#if UNITY_EDITOR
using System.IO;
using _Project.Scripts.DataBase;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Items
{
    [CreateAssetMenu(menuName = "Configs/Item Type Generator Settings")]
    public class ItemTypeGeneratorSettings : ScriptableObject
    {
        private const string DefaultPath = "Assets/_Project/Editor/ItemTypeGeneratorSettings.asset";
        private const string EditorPrefsKey = "ItemTypeGenerator.SettingsGuid";

        [SerializeField] private SpreadsheetContainer _container;
        [SerializeField] private string _outputPath = "Assets/_Project/Scripts/Items/ItemType.cs";

        public SpreadsheetContainer Container => _container;
        public string OutputPath => _outputPath;

        public static ItemTypeGeneratorSettings GetOrCreate()
        {
            string guid = EditorPrefs.GetString(EditorPrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(guid))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(path))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<ItemTypeGeneratorSettings>(path);
                    if (asset != null) return asset;
                }
            }
            
            var guids = AssetDatabase.FindAssets($"t:{nameof(ItemTypeGeneratorSettings)}");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var asset = AssetDatabase.LoadAssetAtPath<ItemTypeGeneratorSettings>(path);
                if (asset != null)
                {
                    EditorPrefs.SetString(EditorPrefsKey, guids[0]);
                    return asset;
                }
            }
            
            var newAsset = CreateInstance<ItemTypeGeneratorSettings>();
            Directory.CreateDirectory(Path.GetDirectoryName(DefaultPath)!);
            AssetDatabase.CreateAsset(newAsset, DefaultPath);
            AssetDatabase.SaveAssets();

            EditorPrefs.SetString(EditorPrefsKey,
                AssetDatabase.AssetPathToGUID(DefaultPath));

            Debug.Log($"[ItemTypeGenerator] Created settings at {DefaultPath}. " +
                      $"Assign your SpreadsheetContainer in the inspector.");
            return newAsset;
        }
    }
}
#endif