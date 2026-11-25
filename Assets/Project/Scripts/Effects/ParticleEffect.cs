using System;
using UnityEngine;

namespace Project.Scripts.Effects
{
    [Serializable]
    public class ParticleEffect
    {
        [SerializeField] private string _particleName;

        public string ParticleName => _particleName;

        public void SetName(string particleName)
        {
            _particleName = particleName;
        }
    }
}