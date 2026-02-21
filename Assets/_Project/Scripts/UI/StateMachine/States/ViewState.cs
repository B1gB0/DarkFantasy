namespace _Project.Scripts.UI.StateMachine.States
{
    public abstract class ViewState : UIState
    {
        private readonly View.View _view;
        
        protected ViewState(View.View view)
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