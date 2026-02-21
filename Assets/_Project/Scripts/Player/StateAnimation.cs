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

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movement = GetComponent<Movement>();
            _attack = GetComponent<Attack>();
        }

        private void OnEnable()
        {
            _movement.IsMovePerformed += ActiveRun;
            _attack.OnAttaked += ActiveAttack;
        }

        private void OnDisable()
        {
            _movement.IsMovePerformed -= ActiveRun;
            _attack.OnAttaked -= ActiveAttack;
        }

        private void ActiveRun(float speed)
        {
            _animator.SetFloat(Run, speed);
        }

        private void ActiveAttack(bool onAttack)
        {
            _animator.SetBool(Attack,onAttack);
        }
    }
}
