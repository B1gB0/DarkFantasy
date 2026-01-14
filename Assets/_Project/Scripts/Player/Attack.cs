using UnityEngine;

namespace _Project.Scripts.Player
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private float power = 10;
        [SerializeField] private float _velocity;
        [SerializeField] private Animator _animatorPlayer;

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out Enemy.Enemy enemy))
            {
                transform.LookAt(other.transform);
                _animatorPlayer.SetBool("Attack", true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            _animatorPlayer.SetBool("Attack", false);
        }
    }
}