using System;

using UnityEngine;
using UnityEngine.AI;

namespace IGI.Enemy
{
    public class EnemyMove
    {
        public Action<EnemyMove> onComplete { get; set; } = delegate { };
        private readonly NavMeshAgent agent;
        private Action onArriveCallback;

        public EnemyMove(NavMeshAgent agent, Vector3 destination)
        {
            this.agent = agent;
            agent.SetDestination(destination);
        }

        public EnemyMove OnArrive(Action callback)
        {
            onArriveCallback = callback;
            return this;
        }

        public void ArrivalCheck()
        {
            if (!agent.pathPending &&
                agent.remainingDistance <= agent.stoppingDistance &&
                (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
            {
                onComplete(this);
                onArriveCallback?.Invoke();
            }
        }
    }
}
