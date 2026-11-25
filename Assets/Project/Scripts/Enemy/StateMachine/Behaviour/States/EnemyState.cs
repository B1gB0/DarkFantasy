using Project.Scripts.DataBase.Data;
using Project.Scripts.Enemy.StateMachine.Animation;
using Project.Scripts.Services;
using UnityEngine.AI;

namespace Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public abstract class EnemyState
    {
        protected const int MinValue = 0;

        private NavMeshAgent _agent;
        private Enemy _enemy;
        private ParticleEffectsService _particleEffectsService;
        private AudioSoundsService _audioSoundsService;

        protected NavMeshAgent Agent => _agent;
        protected Enemy Enemy => _enemy;
        protected ParticleEffectsService ParticleEffectsService => _particleEffectsService;
        protected AudioSoundsService AudioSoundsService => _audioSoundsService;
        protected EnemyStateMachine EnemyStateMachine => _enemy.EnemyStateMachine;
        protected EnemyAnimatedStateMachine AnimStateMachine => _enemy.AnimatedStateMachine;
        protected Player.Core.Player Player => _enemy.Player;
        protected EnemyData Data => _enemy.Data;

        public virtual void Initialize(
            Enemy enemy,
            NavMeshAgent agent,
            ParticleEffectsService particleEffectsService,
            AudioSoundsService audioSoundsService)
        {
            _enemy = enemy;
            _agent = agent;
            _particleEffectsService = particleEffectsService;
            _audioSoundsService = audioSoundsService;
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
        }
    }
}