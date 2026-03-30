using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Enemy.StateMachine.Animation;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public abstract class EnemyState
    {
        protected NavMeshAgent Agent;
        protected Enemy Enemy;

        protected EnemyStateMachine EnemyStateMachine => Enemy.EnemyStateMachine;
        protected EnemyAnimatedStateMachine AnimStateMachine => Enemy.AnimatedStateMachine;
        protected Player.Core.Player Player => Enemy.Player;
        protected EnemyData Data => Enemy.Data;

        public virtual void Initialize(Enemy enemy, NavMeshAgent agent)
        {
            Enemy = enemy;
            Agent = agent;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }     
    }
}
