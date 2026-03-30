using _Project.Scripts.Player.Combat;
using UnityEngine;

namespace _Project.Scripts.Player.Animation
{
    public class StateAnimation : MonoBehaviour
    {
        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int ComboStep = Animator.StringToHash("ComboStep");

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
            _movement.IsMovePerformed += OnMove;
            _attack.OnAttacked += OnAttack;
            _attack.OnComboChanged += OnComboChanged;
        }

        private void OnDisable()
        {
            _movement.IsMovePerformed -= OnMove;
            _attack.OnAttacked -= OnAttack;
            _attack.OnComboChanged -= OnComboChanged;
        }

        private void OnMove(float speed)
        {
            _animator.SetFloat(Run, speed);
        }

        private void OnAttack(bool isAttacking)
        {
            Debug.Log(isAttacking);
            _animator.SetBool(Attack, isAttacking);
        }

        private void OnComboChanged(int step)
        {
            _animator.SetInteger(ComboStep, step);
        }
    }
}