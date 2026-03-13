using System;
using System.Collections.Generic;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation
{
    public class EnemyAnimatedStateMachine
    {
        private readonly Dictionary<Type, AnimatedState> _states = new();

        private AnimatedState _currentState;

        public EnemyAnimatedStateMachine(Animator animator)
        {
            AnimationNamesBase animationBase = new();

            AddState(new IdleAnimatedState(animator, animationBase));
            AddState(new AimAnimatedState(animator, animationBase));
            AddState(new MoveAnimatedState(animator, animationBase));
            AddState(new AttackAnimatedState(animator, animationBase));
            AddState(new ReloadingAnimatedState(animator, animationBase));
            AddState(new HitAnimatedState(animator, animationBase));
        }

        public void EnterIn<T>()
            where T : AnimatedState
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

        private void AddState(AnimatedState state)
        {
            var type = state.GetType();

            if (_states.ContainsKey(type) == false)
            {
                _states.TryAdd(type, state);
            }
        }
    }
}