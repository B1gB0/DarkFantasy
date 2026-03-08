using UnityEngine;

namespace _Project.Scripts.Player
{
    public class StateAnimation : MonoBehaviour
    {
        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int Attack = Animator.StringToHash("Attack");

        [SerializeField] private Animator _animator;
        [SerializeField] private Movement _movement;
        [SerializeField] private Attack _attack;

        [SerializeField] private float _runDampTime = 0.08f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _movement = GetComponent<Movement>();
            
            _attack = GetComponent<Attack>();
        }

        private void OnEnable()
        {
            _movement.IsMovePerformed += OnMove;
            _attack.OnAttaсked += OnAttack;
        }

        private void OnDisable()
        {
            _movement.IsMovePerformed -= OnMove;
            _attack.OnAttaсked -= OnAttack;
        }

        private void OnMove(float speed)
        {
            _animator.SetFloat(Run, speed, _runDampTime, Time.deltaTime);
        }

        private void OnAttack(bool isAttacking)
        {
            Debug.Log(isAttacking);
            _animator.SetBool(Attack, isAttacking);
        }
    }
}