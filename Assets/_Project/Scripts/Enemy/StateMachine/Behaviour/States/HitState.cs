using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class HitState : EnemyState
    {
        private float hitDuration = 0.5f;

        private float _timer;

        public override void Enter()
        {
            // Пример случайного выбора одной из двух hit-анимаций
            // int randomHit = Random.Range(0, 2);
            // if (randomHit == 0)
            //     AnimStateMachine.EnterIn<Hit1AnimatedState>();
            // else
            //     AnimStateMachine.EnterIn<Hit2AnimatedState>();

            AnimStateMachine.EnterIn<HitAnimatedState>();

            _timer = hitDuration;
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