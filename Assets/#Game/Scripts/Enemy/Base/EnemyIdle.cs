using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Idle", menuName = "SO/Enemy State/Idle")]
    public class EnemyIdle : EnemyBaseState
    {
        public override void EnterState(EnemyBrain brain)
        {
            base.EnterState(brain);
            Vector3 targetPosition = brain.stateMemory.targetIdlePosition.HasValue ? brain.stateMemory.targetIdlePosition.Value : brain.transform.position;

            if(!brain.stateMemory.targetIdlePosition.HasValue) brain.stateMemory.targetIdlePosition = targetPosition;

            if ((brain.transform.position - targetPosition).sqrMagnitude <= 0.1f)
            {
                brain.stateMemory.IsStateFinished = true;
                return;
            }

            brain.Controller.MoveTowards(targetPosition).OnArrive(() =>
            {
                brain.stateMemory.IsStateFinished = true;
            });
        }

        public override void UpdateState(EnemyBrain brain)
        {
            base.UpdateState(brain);
        }

        public override void PauseState(EnemyBrain brain)
        {
            base.PauseState(brain);
            brain.Controller.CancelCurrentMove();
        }

        public override void ResumeState(EnemyBrain brain)
        {
            base.ResumeState(brain);
            Vector3 targetPosition = brain.stateMemory.targetIdlePosition.Value;

            brain.Controller.MoveTowards(targetPosition).OnArrive(() =>
            {
                brain.stateMemory.IsStateFinished = true;
            });
        }

        public override void ExitState(EnemyBrain brain)
        {
            brain.Controller.CancelCurrentMove();
        }
    }
}