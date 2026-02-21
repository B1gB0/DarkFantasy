using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace _Project.Scripts.Effects
{
    [CreateAssetMenu(menuName = "ParticlesConfig")]
    public class ParticlesConfig : ScriptableObject
    {
        public List<ParticleEffect> Particles = new();

#if UNITY_EDITOR
        [SerializeField] private string _particleLabel = "Particles";

        public void ImportFromAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable settings not found!");
                return;
            }

            Undo.RecordObject(this, "Import from Addressables");
            Particles.Clear();

            ImportByLabel(_particleLabel);

            EditorUtility.SetDirty(this);
            Debug.Log($"Imported from Addressables: {Particles.Count} particles total");
        }

        private void ImportByLabel(string label)
        {
            if (string.IsNullOrEmpty(label)) return;

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entries = new List<AddressableAssetEntry>();
            settings.GetAllAssets(entries, false);

            foreach (var entry in entries)
            {
                if (entry.labels.Contains(label))
                {
                    var particleAsset = AssetDatabase.LoadAssetAtPath<ParticleSystem>(entry.AssetPath);
                    if (particleAsset != null)
                    {
                        var existingParticle
                            = Particles.FirstOrDefault(p => p.ParticleName == particleAsset.name);

                        if (existingParticle == null)
                        {
                            var particle = new ParticleEffect
                            {
                                ParticleName = particleAsset.name
                            };

                            Particles.Add(particle);
                        }
                    }
                }
            }
        }
#endif
    }
}