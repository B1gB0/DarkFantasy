using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class Priest : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
    }
}