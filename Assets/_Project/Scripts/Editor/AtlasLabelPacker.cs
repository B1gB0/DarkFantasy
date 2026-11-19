#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace _Project.Editor
{
    public class AtlasLabelPacker
    {
        [MenuItem("Tools/Pack Atlas By Label")]
        public static void PackByLabel()
        {
            // Загружаем твой атлас по пути (проверь, что путь правильный!)
            string atlasPath = "Assets/_Project/UIAtlas.spriteatlas";
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

            if (atlas == null)
            {
                Debug.LogError($"Не найден атлас по пути: {atlasPath}");
                return;
            }

            // Ищем ВСЕ спрайты в проекте, у которых есть метка "UISprites"
            string[] guids = AssetDatabase.FindAssets("l:UISprites");
            List<Object> spritesToPack = new List<Object>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    spritesToPack.Add(sprite);
                }
            }

            // Очищаем атлас от старых привязок и добавляем только найденные спрайты
            var currentPackables = atlas.GetPackables();
            atlas.Remove(currentPackables);
            atlas.Add(spritesToPack.ToArray());

            EditorUtility.SetDirty(atlas);
            AssetDatabase.SaveAssets();

            Debug.Log($"Успех! В атлас упаковано {spritesToPack.Count} спрайтов с меткой 'UISprites'.");
        }
    }
}
#endif