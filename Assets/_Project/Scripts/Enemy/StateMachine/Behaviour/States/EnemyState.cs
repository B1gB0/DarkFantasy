using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Services;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public abstract class EnemyState
    {
        protected const int MinValue = 0;

        protected NavMeshAgent Agent;
        protected Enemy Enemy;
        protected ParticleEffectsService ParticleEffectsService;
        protected AudioSoundsService AudioSoundsService;

        protected EnemyStateMachine EnemyStateMachine => Enemy.EnemyStateMachine;
        protected EnemyAnimatedStateMachine AnimStateMachine => Enemy.AnimatedStateMachine;
        protected Player.Core.Player Player => Enemy.Player;
        protected EnemyData Data => Enemy.Data;

        public virtual void Initialize(
            Enemy enemy,
            NavMeshAgent agent,
            ParticleEffectsService particleEffectsService,
            AudioSoundsService audioSoundsService)
        {
            Enemy = enemy;
            Agent = agent;
            ParticleEffectsService = particleEffectsService;
            AudioSoundsService = audioSoundsService;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
            if (Enemy.Health.TargetHealth <= MinValue)
                EnemyStateMachine.SwitchState<DeathState>();
        }
    }
}