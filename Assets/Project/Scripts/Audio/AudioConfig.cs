using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Audio.Sounds;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEngine;

namespace Project.Scripts.Audio
{
    [CreateAssetMenu(menuName = "AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        [SerializeField] private List<Sound> _sounds = new();

#if UNITY_EDITOR
        [SerializeField] private string _soundLabel = "Sound";
        [SerializeField] private string _musicLabel = "Music";

        public void ImportFromAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable settings not found!");
                return;
            }

            Undo.RecordObject(this, "Import from Addressables");
            _sounds.Clear();

            ImportByLabel(_soundLabel);
            ImportByLabel(_musicLabel);

            EditorUtility.SetDirty(this);
            Debug.Log($"Imported from Addressables: {_sounds.Count} sounds total");
        }

        public List<Sound> GetSounds()
        {
            return _sounds;
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
                    var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(entry.AssetPath);
                    if (clip != null)
                    {
                        var existingSound = _sounds.FirstOrDefault(sound => sound.ClipName == clip.name);
                        if (existingSound == null)
                        {
                            var sound = new Sound();

                            sound.SetClipName(clip.name);
                            sound.SetVolume(1f);

                            if (label == _musicLabel)
                                sound.SetIsLoop(true);

                            _sounds.Add(sound);
                        }
                    }
                }
            }
        }
#endif
    }
}