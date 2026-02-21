using System;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Enemy.StateMachine;
using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Enemy.StateMachine.Behaviour;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(Animator))]
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        
        public event Action<Enemy> Die;
        public event Action OnFollowPlayer;

        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public EnemyStateMachine EnemyStateMachine { get; private set; }
        
        public EnemyData Data { get; private set; }
        public Player.Player Player { get; private set; }
        public EnemyAnimatedStateMachine AnimatedStateMachine { get; private set; }
        public EnemyType Type { get; private set; }

        private void Start()
        {
            AnimatedStateMachine = new EnemyAnimatedStateMachine(_animator);
        }

        public void GetData(Player.Player player, EnemyData enemyData)
        {
            Player = player;
            Data = enemyData;
            Type = Data.Type;
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