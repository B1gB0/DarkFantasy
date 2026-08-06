using System.Collections.Generic;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class DeathState : EnemyState
    {
        private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

        private const float DissolveDelay = 4f;
        private const float DissolveDuration = 3f;
        private const float TotalDeathDuration = 10f;

        private float _deathTimer;
        private bool _deathProcessed;
        private bool _dissolveStarted;
        private float _dissolveTimer;

        private MaterialPropertyBlock _propertyBlock;
        private List<Renderer> _dissolveRenderers = new List<Renderer>();

        public override void Enter()
        {
            _deathTimer = TotalDeathDuration;
            _deathProcessed = false;
            _dissolveStarted = false;
            _dissolveTimer = 0f;

            if (Agent != null && Agent.isOnNavMesh)
            {
                Agent.isStopped = true;
                Agent.enabled = false;
            }

            foreach (var col in Enemy.GetComponentsInChildren<Collider>())
                col.enabled = false;

            var rb = Enemy.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;
            
            AnimStateMachine.EnterIn<DeathAnimatedState>();
            
            _dissolveRenderers.Clear();
            foreach (var rend in Enemy.GetComponentsInChildren<Renderer>())
            {
                if (rend.sharedMaterial.HasProperty(DissolveAmountID))
                    _dissolveRenderers.Add(rend);
            }

            _propertyBlock = new MaterialPropertyBlock();
            
            SetDissolveAmount(0f);

            // Эффекты можно запустить здесь
            // ParticleEffectsService.PlayEffect(ParticleType.DeathEffect, Enemy.transform.position);
            // AudioSoundsService.PlaySound(SoundType.EnemyDeath);
        }

        public override void Update()
        {
            _deathTimer -= Time.deltaTime;
            
            if (!_dissolveStarted && _deathTimer <= TotalDeathDuration - DissolveDelay)
            {
                _dissolveStarted = true;
                _dissolveTimer = 0f;
            }
            
            if (_dissolveStarted)
            {
                _dissolveTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(_dissolveTimer / DissolveDuration);
                SetDissolveAmount(progress);
                
                if (progress >= 1f && !_deathProcessed)
                {
                    _deathProcessed = true;
                    Enemy.gameObject.SetActive(false);
                }
            }
            
            if (_deathTimer <= 0f && !_deathProcessed)
            {
                _deathProcessed = true;
                Enemy.gameObject.SetActive(false);
            }
        }

        public override void Exit()
        {
            SetDissolveAmount(0f);
        }

        private void SetDissolveAmount(float amount)
        {
            foreach (var rend in _dissolveRenderers)
            {
                rend.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(DissolveAmountID, amount);
                rend.SetPropertyBlock(_propertyBlock);
            }
        }
    }
}