using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEngine;

namespace Project.Scripts.Effects
{
    [CreateAssetMenu(menuName = "ParticlesConfig")]
    public class ParticlesConfig : ScriptableObject
    {
        [SerializeField] private List<ParticleEffect> _particles = new();

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
            _particles.Clear();

            ImportByLabel(_particleLabel);

            EditorUtility.SetDirty(this);
            Debug.Log($"Imported from Addressables: {_particles.Count} particles total");
        }

        public List<ParticleEffect> GetParticles()
        {
            return _particles;
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
                            = _particles.FirstOrDefault(p => p.ParticleName == particleAsset.name);

                        if (existingParticle == null)
                        {
                            var particle = new ParticleEffect();
                            particle.SetName(particleAsset.name);

                            _particles.Add(particle);
                        }
                    }
                }
            }
        }
#endif
    }
}