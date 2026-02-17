using System;
using _Project.Scripts.Enemy.StateMachine;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Health _health;

        private Animator _animator;

        public EnemyAnimatedStateMachine AnimatedStateMachine { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            AnimatedStateMachine = new EnemyAnimatedStateMachine(_animator);
        }
    }
}