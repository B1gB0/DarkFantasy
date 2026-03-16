namespace _Project.Scripts.UI.StateMachine.States
{
    public class LeaderboardPanelState : ViewState
    {
        private readonly View.View _view;

        public LeaderboardPanelState(View.View view) : base(view)
        {
            _view = view;
        }
        
        public override void Enter()
        {
            _view.Show();
        }

        public override void Exit()
        {
            _view.Hide();
        }
    }
}