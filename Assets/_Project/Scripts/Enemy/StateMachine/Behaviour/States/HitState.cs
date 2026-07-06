using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class HitState : EnemyState
    {
        private const float HitDuration = 0.5f;

        private float _timer;

        public override void Enter()
        {
            AnimStateMachine.EnterIn<HitEnemyAnimatedState>();

            _timer = HitDuration;
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= MinValue)
            {
                Enemy.ChangeFollowEnemyState(true);
                EnemyStateMachine.SwitchState<FollowState>();
            }
        }
    }
}