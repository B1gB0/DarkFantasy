using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerSitState : IPlayerState
    {
        private const int LayerIndex = 0;
        private const float FallbackEndThreshold = 0.95f;
        private const float InputDelay = 0.2f;
        
        private readonly Core.Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerAnimatedState _anim;

        private bool _isStandingUp;
        private float _inputTimer;

        public PlayerSitState(Core.Player player)
        {
            _player = player;
            _stateMachine = _player.StateMachine;
            _anim = _player.PlayerAnimatedState;
        }

        public StateId IdState => StateId.Sit;

        public void Enter()
        {
            _player.Rigidbody.velocity = Vector3.zero;
            
            _anim.OnSitDown();
            _isStandingUp = false;
            _inputTimer = InputDelay;
            
            _player.InputController.LockPlayerMovement(); 
        }

        public void Update()
        {
            if (_isStandingUp)
            {
                AnimatorStateInfo stateInfo = _player.Animator.GetCurrentAnimatorStateInfo(LayerIndex);
                
                if (stateInfo.IsName("StandUp") && stateInfo.normalizedTime >= FallbackEndThreshold)
                {
                    _stateMachine.SwitchState(StateId.Idle);
                }
                
                return;
            }
            
            if (_inputTimer > 0f)
            {
                _inputTimer -= Time.deltaTime;
                return;
            }
            
            bool wantToStandUp = _player.InputController.IsPickUpPressed;

            if (wantToStandUp)
            {
                _isStandingUp = true;
                _anim.OnStandUp();
            }
        }

        public void FixedUpdate() { }

        public void Exit()
        {
            _player.InputController.SetMovementLock(false);
        }
    }
}