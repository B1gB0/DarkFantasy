using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;

namespace _Project.Scripts.Player
{
    public class PlayerAttackState : IPlayerState
    {
        private const int MinCombo = 0;
        private const int FirstStrike = 1;
        private const int MaxCombo = 3;

        private readonly Core.Player _player;
        private readonly PlayerAnimatedState _anim;
        private readonly PlayerStateMachine _stateMachine;

        private bool _canQueueNextAttack;
        private bool _nextAttackQueued;
        private bool _attackBuffered;        // ← буфер нажатия
        private int _comboCounter;

        public PlayerAttackState(Core.Player player)
        {
            _player = player;
            _anim = player.PlayerAnimatedState;
            _stateMachine = player.StateMachine;
        }

        public StateId IdState => StateId.Attack;

        public void Enter()
        {
            _player.InputController.OnAttackButtonPressed += OnAttackButtonPressed;
            
            if (_comboCounter == MinCombo)
                StartCombo();
        }

        public void Update() { }
        public void FixedUpdate() { }

        public void Exit()
        {
            _player.InputController.OnAttackButtonPressed -= OnAttackButtonPressed;
            ResetCombo();
        }

        private void StartCombo()
        {
            _comboCounter = FirstStrike;
            _canQueueNextAttack = false;
            _nextAttackQueued = false;
            _attackBuffered = false;          // сбрасываем буфер
            _anim.OnAttack(true);
            _anim.OnComboChanged(_comboCounter);
        }

        // Вызывается из анимации, когда можно прервать текущий удар следующим
        public void AllowCombo()
        {
            _canQueueNextAttack = true;

            // Если нажатие ждало в буфере — сразу обрабатываем
            if (_attackBuffered)
            {
                _attackBuffered = false;
                TryContinueCombo();
            }
        }

        public void QueueNextCombo()
        {
            if (_comboCounter > MaxCombo)
            {
                _canQueueNextAttack = false;
                return;
            }

            _comboCounter++;
            _canQueueNextAttack = false;
        }

        public void StartDamageWindow() => _player.Sword.ActivateCollider();
        public void EndDamageWindow()   => _player.Sword.DeactivateCollider();

        public void EndAttack()
        {
            if (_nextAttackQueued)
            {
                _nextAttackQueued = false;
                _canQueueNextAttack = false;
                _attackBuffered = false;
                _anim.OnAttack(true);
                return;
            }

            ResetCombo();

            if (_player.InputController.IsRollInputPerformed)
                _stateMachine.SwitchState(StateId.Roll);
            else if (_player.InputController.IsMoveInputPerformed)
                _stateMachine.SwitchState(StateId.Move);
            else
                _stateMachine.SwitchState(StateId.Idle);
        }

        private void ResetCombo()
        {
            _comboCounter = MinCombo;
            _canQueueNextAttack = false;
            _attackBuffered = false;
            _nextAttackQueued = false;
            _anim.OnAttack(false);
            _anim.OnComboChanged(_comboCounter);
        }

        private void OnAttackButtonPressed()
        {
            // Если окно открыто и комбо ещё не закончено – выполняем немедленно
            if (_canQueueNextAttack && _comboCounter < MaxCombo)
            {
                TryContinueCombo();
            }
            else
            {
                // Иначе кладём нажатие в буфер (даже если окно закрыто или комбо достигло максимума)
                _attackBuffered = true;
            }
        }

        private void TryContinueCombo()
        {
            if (!_canQueueNextAttack || _comboCounter >= MaxCombo)
                return;

            _comboCounter++;
            _canQueueNextAttack = false;
            _nextAttackQueued = true;
            _anim.OnComboChanged(_comboCounter);
        }
    }
}