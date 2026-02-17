using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class SkeletonRanger : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
        
        [field: SerializeField] public EnemyPatrolComponent EnemyPatrolComponent { get; private set; }
        
        // private void Start()
        // {
        //     EnemyPatrolComponent = GetComponent<EnemyPatrolComponent>();
        // }
    }
}