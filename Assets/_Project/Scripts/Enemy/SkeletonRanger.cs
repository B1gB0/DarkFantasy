using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Weapon.Enemy;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(Longbow))]
    public class SkeletonRanger : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public GameObject FakeArrow { get; private set; }

        public Longbow Longbow { get; private set; }
        
        private void Awake()
        {
            Longbow = GetComponent<Longbow>();
        }

        public override void OnReactState(bool isEnteredToState)
        {
            FakeArrow.gameObject.SetActive(isEnteredToState);
        }

        protected override void OnPlayHitEffect()
        {
            AudioSoundsService.PlaySound(SoundsType.SkeletonHit).Forget();
            base.OnPlayHitEffect();
        }
    }
}