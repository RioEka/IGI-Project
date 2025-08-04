using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Combat", menuName = "SO/Enemy State/Combat")]
    public class EnemyCombat : EnemyBaseState
    {
        [SerializeField] private float radiusAwarenessInfluence = 10f;
        [SerializeField] private float alertInterval = 0.5f;
        [SerializeField] private float proximityCheckDistance = 2f, moveSpeed = 4f;

        public override void EnterState(EnemyBrain brain)
        {
            base.EnterState(brain);
            brain.stateMemory.isMovingToLastKnown = false;
            brain.HasBeenAlerted = true;
            brain.stateMemory.alertTimer = 0f;
            brain.SetMoveSpeed(moveSpeed);

            TryChaseTarget(brain);
        }

        public override void UpdateState(EnemyBrain brain)
        {
            base.UpdateState(brain);

            // -- Limit alert broadcasts --
            brain.stateMemory.alertTimer -= Time.deltaTime;
            if (brain.stateMemory.alertTimer <= 0f)
            {
                EnemyAwarenessSystem.AlertNearbyEnemies(brain, brain.suspiciousLocation, radiusAwarenessInfluence);
                brain.stateMemory.alertTimer = alertInterval;
            }

            if (brain.AttackRange.TargetOnCaught != null)
            {
                HandleShooting(brain, brain.AttackRange.TargetOnCaught.transform);
                return;
            }

            var playerTransform = brain.FieldOfView.TargetNoDelete;
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(brain.transform.position, playerTransform.position);
                if (distance < proximityCheckDistance)
                {
                    brain.Controller.LookAt(playerTransform.position);
                    brain.SetSuspiciousLocation(playerTransform.position);
                    //Debug.LogWarning($"{brain.name}: Player sangat dekat meskipun tidak terlihat");
                    return;
                }
            }

            TryChaseTarget(brain);
        }

        private void HandleShooting(EnemyBrain brain, Transform target)
        {
            brain.stateMemory.isMovingToLastKnown = false;
            brain.Controller.CancelCurrentMove();

            Vector3 toTarget = target.position - brain.transform.position;
            if (Vector3.Angle(brain.transform.forward, toTarget) > 5f)
                brain.Controller.LookAt(target.position);

            brain.Controller.Shooting(true);
            brain.SetSuspiciousLocation(target.position);
            brain.stateMemory.IsStateFinished = false;
        }

        private void TryChaseTarget(EnemyBrain brain)
        {
            if (!brain.stateMemory.isMovingToLastKnown)
            {
                brain.stateMemory.isMovingToLastKnown = true;
                brain.Controller.Shooting(false);
                brain.Controller.MoveTowards(brain.suspiciousLocation).OnArrive(() =>
                {
                    brain.stateMemory.IsStateFinished = true;
                });
            }
        }

        public override void PauseState(EnemyBrain brain)
        {
            base.PauseState(brain);
        }

        public override void ResumeState(EnemyBrain brain)
        {
            base.ResumeState(brain);
            brain.stateMemory.isMovingToLastKnown = false;
            TryChaseTarget(brain);
        }

        public override void ExitState(EnemyBrain brain)
        {
            brain.stateMemory.isMovingToLastKnown = false;
            brain.Controller.Shooting(false);
        }
    }
}