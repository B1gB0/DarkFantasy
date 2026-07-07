using System;
using System.Collections.Generic;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation
{
    public class EnemyAnimatedStateMachine
    {
        private readonly Dictionary<Type, EnemyAnimatedState> _states = new();

        private EnemyAnimatedState _currentState;

        public EnemyAnimatedStateMachine(Animator animator)
        {
            EnemyAnimationNamesBase enemyAnimationBase = new();

            AddState(new IdleEnemyAnimatedState(animator, enemyAnimationBase));
            AddState(new AimEnemyAnimatedState(animator, enemyAnimationBase));
            AddState(new MoveEnemyAnimatedState(animator, enemyAnimationBase));
            AddState(new AttackEnemyAnimatedState(animator, enemyAnimationBase));
            AddState(new ReloadingEnemyAnimatedState(animator, enemyAnimationBase));
            AddState(new HitEnemyAnimatedState(animator, enemyAnimationBase));
            AddState(new CoilEnemyAnimatedState(animator, enemyAnimationBase));
            AddState(new OmniEnemyAnimatedState(animator, enemyAnimationBase));
            AddState(new DeathAnimatedState(animator, enemyAnimationBase));
        }

        public void EnterIn<T>()
            where T : EnemyAnimatedState
        {
            var type = typeof(T);

            if (_currentState != null && _currentState.GetType() == type)
            {
                return;
            }

            if (!_states.TryGetValue(type, out var newState))
                return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        private void AddState(EnemyAnimatedState state)
        {
            var type = state.GetType();

            if (_states.ContainsKey(type) == false)
            {
                _states.TryAdd(type, state);
            }
        }
    }
}