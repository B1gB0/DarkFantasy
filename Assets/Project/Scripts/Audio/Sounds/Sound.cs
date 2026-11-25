using System;
using UnityEngine;

namespace Project.Scripts.Audio.Sounds
{
    [Serializable]
    public class Sound
    {
        [SerializeField] private string _clipName;
        [SerializeField] private AudioClip _clip;
        [SerializeField] private bool _isLoop;

        [SerializeField] [Range(0f, 1f)] private float _volume = 1f;

        public string ClipName => _clipName;
        public AudioClip Clip => _clip;
        public bool IsLoop => _isLoop;
        public float Volume => _volume;

        public void SetClipName(string clipName)
        {
            _clipName = clipName;
        }

        public void SetClip(AudioClip clip)
        {
            _clip = clip;
        }

        public void SetIsLoop(bool isLoop)
        {
            _isLoop = isLoop;
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
        }
    }
}