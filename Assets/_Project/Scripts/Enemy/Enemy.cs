using System;
using _Project.Scripts.Enemy.StateMachine;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(Animator))]
    public abstract class Enemy : MonoBehaviour
    {
        private Animator _animator;
        
        public event Action<Enemy> Die;

        public Health Health { get; private set; }
        public EnemyAnimatedStateMachine AnimatedStateMachine { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            Health = GetComponent<Health>();
            AnimatedStateMachine = new EnemyAnimatedStateMachine(_animator);
        }
        
        protected virtual void OnDie()
        {
            // ResetModifiers();
            // Health.IsSpawnedDamageText -= TextService.OnChangedFloatingText;
            // OnChangeSpeed -= UpdateCurrentSpeed;
            
            Die?.Invoke(this);
        }
    }
}