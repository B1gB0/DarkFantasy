using _Project.Scripts.Enemy.StateMachine.Behaviour.States;

namespace _Project.Scripts.Player
{
    public interface IPlayerState
    {
        public StateId IdState { get;}
        
        public void Enter();
        public void Update();
        public void FixedUpdate();
        public void Exit();
    }
}