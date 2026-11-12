namespace _Project.Scripts.Player.State
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