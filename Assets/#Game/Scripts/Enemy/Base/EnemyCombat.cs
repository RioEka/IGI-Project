using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Combat", menuName = "SO/Enemy State/Combat")]
    public class EnemyCombat : EnemyBaseState
    {
        [SerializeField] private float radiusAwarenessInfluence = 10f;

        public override void EnterState(EnemyBrain brain)
        {
            base.EnterState(brain);
            brain.stateMemory.isMovingToLastKnown = false;
            TryChaseTarget(brain);
            //Debug.LogError(brain.name + ": Enter Shooting");
            brain.HasBeenAlerted = true;
        }

        public override void UpdateState(EnemyBrain brain)
        {
            base.UpdateState(brain);

            EnemyAwarenessSystem.AlertNearbyEnemies(brain, brain.suspiciousLocation, radiusAwarenessInfluence);
            brain.Controller.LookAt(brain.suspiciousLocation);

            if (brain.AttackRange.TargetOnCaught != null)
            {
                brain.stateMemory.isMovingToLastKnown = false;
                var target = brain.AttackRange.TargetOnCaught.transform;

                brain.Controller.CancelCurrentMove();
                brain.Controller.LookAt(target.position);
                brain.Controller.Shooting(true);

                brain.SetSuspiciousLocation(target.position);
                brain.stateMemory.IsStateFinished = false;

                return;
            }

            TryChaseTarget(brain);
        }

        private void TryChaseTarget(EnemyBrain brain)
        {
            if (brain.stateMemory.isMovingToLastKnown) return;

            brain.stateMemory.isMovingToLastKnown = true;
            brain.Controller.Shooting(false);
            brain.Controller.LookAt(brain.suspiciousLocation);
            brain.Controller.MoveTowards(brain.suspiciousLocation).OnArrive(() =>
            {
                brain.stateMemory.IsStateFinished = true;
            });
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