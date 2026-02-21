#if UNITY_EDITOR
using System.Collections.Generic;
using _Project.Scripts.Effects;
using UnityEditor;
using UnityEngine;

namespace _Project.Editor
{
    [CustomEditor(typeof(ParticlesConfig))]
    public class ParticlesConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Addressables Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_particleLabel"));

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Particles List", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Particles"), true);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Import from Addressables", GUILayout.Height(30)))
            {
                var config = (ParticlesConfig)target;
                config.ImportFromAddressables();
            }

            if (GUILayout.Button("Clear All", GUILayout.Width(100), GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog(
                        "Clear All",
                        "Удалить все звуки из конфига?",
                        "Да", "Нет"))
                {
                    var config = (ParticlesConfig)target;
                    Undo.RecordObject(config, "Clear All");
                    config.Particles.Clear();
                    EditorUtility.SetDirty(config);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Generate Sounds Enum", GUILayout.Height(30)))
            {
                ParticlesEnumGenerator.Generate((ParticlesConfig)target);
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Sort by Name"))
            {
                var config = (ParticlesConfig)target;
                Undo.RecordObject(config, "Sort by Name");

                config.Particles.Sort((a, b) => string.Compare(
                    a.ParticleName,
                    b.ParticleName));

                EditorUtility.SetDirty(config);
            }

            if (GUILayout.Button("Validate"))
            {
                ValidateConfig((ParticlesConfig)target);
            }
        }

        private void ValidateConfig(ParticlesConfig config)
        {
            bool hasErrors = false;
            var particleNames = new HashSet<string>();

            foreach (var particle in config.Particles)
            {
                if (string.IsNullOrEmpty(particle.ParticleName))
                {
                    Debug.LogError($"Sound has empty ClipName!");
                    hasErrors = true;
                }
                else if (!particleNames.Add(particle.ParticleName))
                {
                    Debug.LogError($"Duplicate ClipName found: {particle.ParticleName}");
                    hasErrors = true;
                }
            }

            if (!hasErrors)
            {
                Debug.Log($"AudioConfig is valid. Total sounds: {config.Particles.Count}");
            }
        }
    }
}
#endif