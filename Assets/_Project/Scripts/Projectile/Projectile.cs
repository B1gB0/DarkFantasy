using System.Collections;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Projectile
{
    public abstract class Projectile : MonoBehaviour
    {
        private const float DefaultDirectionY = 0f;

        protected float Damage;

        protected Vector3 Direction;
        protected Transform Transform;

        protected ParticleEffectsService ParticleEffectsService;
        protected AudioSoundsService AudioSoundsService;
        
        [SerializeField] private LayerMask[] _layers;
        
        private float _projectileSpeed;

        [field: SerializeField] public float LifeTime { get; private set; } = 4f;

        private void Awake()
        {
            Transform = transform;
        }

        protected virtual void OnEnable()
        {
            StartCoroutine(LifeRoutine());
        }

        protected virtual void FixedUpdate()
        {
            Transform.position += Direction * (_projectileSpeed * Time.fixedDeltaTime);
        }

        protected virtual void OnTriggerEnter(Collider collision)
        {
            CheckBordersLayers(collision);

            if (collision.gameObject.TryGetComponent(out Enemy.Enemy enemy))
            {
                enemy.Health.TakeDamage(Damage);
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnDisable()
        {
            StopCoroutine(LifeRoutine());
        }

        public virtual void SetDirection(Vector3 targetPosition)
        {
            Direction = (targetPosition - Transform.position).normalized;
            Transform.forward = Direction;
            Direction.y = DefaultDirectionY;
            Direction = Direction.normalized;
        }

        public void SetCharacteristics(float damage, float bulletSpeed)
        {
            Damage = damage;
            _projectileSpeed = bulletSpeed;
        }

        public void GetServices(ParticleEffectsService particleEffectsService, AudioSoundsService audioSoundsService)
        {
            ParticleEffectsService = particleEffectsService;
            AudioSoundsService = audioSoundsService;
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