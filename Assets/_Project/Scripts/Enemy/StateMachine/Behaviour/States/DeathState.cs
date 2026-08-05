using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class DeathState : EnemyState
    {
        // Длительность состояния смерти (можно брать из анимации или настроить)
        private const float DeathDuration = 10f;
        private float _timer;
        private bool _deathProcessed;

        public override void Enter()
        {
            _timer = DeathDuration;
            _deathProcessed = false;
            
            if (Agent != null && Agent.isOnNavMesh)
            {
                Agent.isStopped = true;
                Agent.enabled = false;
            }
            
            foreach (var col in Enemy.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }

            // Проигрываем анимацию смерти (убедитесь, что состояние DeathEnemyAnimatedState существует)
            AnimStateMachine.EnterIn<DeathAnimatedState>();

            // Эффекты смерти (пример, используйте свои ParticleType)
            // ParticleEffectsService.PlayEffect(ParticleType.DeathEffect, Enemy.transform.position);
            // AudioSoundsService.PlaySound(SoundType.EnemyDeath); // предположим, такой метод есть
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;

            // Когда анимация/эффекты завершены – убираем объект
            if (_timer <= 0f && !_deathProcessed)
            {
                _deathProcessed = true;
                Enemy.gameObject.SetActive(false);  // или Destroy(Enemy.gameObject)
            }
        }

        public override void Exit()
        {
            // Обычно переход из смерти не происходит, но если понадобится (например, revive),
            // включите всё обратно
        }
    }
}