using UnityEngine;

namespace _Project.Scripts.Player
{
    public class StateAnimation : MonoBehaviour
    {
        private const string NameParameterRun = "Run";
        private const string NameParameterAttack = "Attack";

        private static readonly int Run = Animator.StringToHash(NameParameterRun);
        private static readonly int Attack = Animator.StringToHash(NameParameterAttack);

        [SerializeField] private Animator _animator;
        [SerializeField] private Movement _movement;
        [SerializeField] private Attack _attack;

        // Время сглаживания параметра Run (в секундах)
        [SerializeField] private float _runDampTime = 0.08f;

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();

            if (_movement == null)
                _movement = GetComponent<Movement>();

            if (_attack == null)
                _attack = GetComponent<Attack>();
        }

        private void OnEnable()
        {
            if (_movement != null)
                _movement.IsMovePerformed += ActiveRun;

            if (_attack != null)
                _attack.OnAttaсked += ActiveAttack;
        }

        private void OnDisable()
        {
            if (_movement != null)
                _movement.IsMovePerformed -= ActiveRun;

            if (_attack != null)
                _attack.OnAttaсked -= ActiveAttack;
        }

        private void ActiveRun(float speed)
        {
            // Сглаживаем изменение параметра, чтобы избежать кратковременных провалов
            _animator.SetFloat(Run, speed, _runDampTime, Time.deltaTime);
        }

        private void ActiveAttack(bool onAttack)
        {
            _animator.SetBool(Attack, onAttack);
        }
    }
}

