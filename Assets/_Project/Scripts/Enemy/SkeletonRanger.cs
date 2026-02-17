using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class SkeletonRanger : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
    }
}