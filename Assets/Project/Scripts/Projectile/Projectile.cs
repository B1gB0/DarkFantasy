using System.Collections;
using Project.Scripts.Services;
using UnityEngine;

namespace Project.Scripts.Projectile
{
    public abstract class Projectile : MonoBehaviour
    {
        private const float DefaultDirectionY = 0f;

        [SerializeField] private LayerMask[] _layers;

        private float _damage;

        private Vector3 _direction;
        private Transform _transform;

        private ParticleEffectsService _particleEffectsService;
        private AudioSoundsService _audioSoundsService;

        private float _projectileSpeed;

        [field: SerializeField] public float LifeTime { get; private set; } = 4f;

        protected float Damage => _damage;
        protected Vector3 Direction => _direction;
        protected Transform Transform => _transform;
        protected ParticleEffectsService ParticleEffectsService => _particleEffectsService;
        protected AudioSoundsService AudioSoundsService => _audioSoundsService;

        private void Awake()
        {
            _transform = transform;
        }

        protected virtual void OnEnable()
        {
            StartCoroutine(LifeRoutine());
        }

        protected virtual void FixedUpdate()
        {
            _transform.position += _direction * (_projectileSpeed * Time.fixedDeltaTime);
        }

        protected virtual void OnTriggerEnter(Collider collision)
        {
            CheckBordersLayers(collision);

            if (collision.gameObject.TryGetComponent(out Enemy.Enemy enemy))
            {
                enemy.Health.TakeDamage(_damage);
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnDisable()
        {
            StopCoroutine(LifeRoutine());
        }

        public virtual void SetDirection(Vector3 targetPosition)
        {
            _direction = (targetPosition - _transform.position).normalized;
            _transform.forward = _direction;
            _direction.y = DefaultDirectionY;
            _direction = _direction.normalized;
        }

        public void SetCharacteristics(float damage, float bulletSpeed)
        {
            _damage = damage;
            _projectileSpeed = bulletSpeed;
        }

        public void GetServices(ParticleEffectsService particleEffectsService, AudioSoundsService audioSoundsService)
        {
            _particleEffectsService = particleEffectsService;
            _audioSoundsService = audioSoundsService;
        }

        protected void CheckBordersLayers(Collider collision)
        {
            foreach (var layerMask in _layers)
            {
                if ((layerMask.value & (1 << collision.gameObject.layer)) != 0)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }
        }

        private IEnumerator LifeRoutine()
        {
            yield return new WaitForSeconds(LifeTime);

            gameObject.SetActive(false);
        }
    }
}