namespace _Project.Scripts.Player
{
    public interface IPlayerState
    {
        public void Enter();
        public void Update();
        public void FixedUpdate();
        public void Exit();
    }
}