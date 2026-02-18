using System;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class Skeleton : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }

        // private void Awake()
        // {
        //     EnemyPatrolComponent = GetComponent<EnemyPatrolComponent>();
        // }
    }
}