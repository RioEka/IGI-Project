using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Combat", menuName = "SO/Enemy State/Combat")]
    public class EnemyCombat : EnemyBaseState
    {
        private bool isMovingToLastKnown;

        public override void Initialize(EnemyBrain brain)
        {
            base.Initialize(brain);
            State = EnemyBrain.EnemyState.Combat;
        }

        public override void EnterState()
        {
            base.EnterState();
            TryChaseTarget();
            isMovingToLastKnown = false;
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if (brain.AttackRange.TargetOnCaught != null)
            {
                IsStateFinished = false;
                var target = brain.AttackRange.TargetOnCaught.transform;
                isMovingToLastKnown = false;

                brain.Controller.CancelCurrentMove();
                brain.Controller.LookAt(target.position);
                brain.Controller.Shooting(true);

                brain.SetSuspiciousLocation(target.position);

                return;
            }

            TryChaseTarget();
        }

        private void TryChaseTarget()
        {
            if (isMovingToLastKnown) return;
            //Debug.Log("mantap");

            isMovingToLastKnown = true;
            brain.Controller.Shooting(false);
            brain.Controller.MoveTowards(brain.suspiciousLocation).OnArrive(() =>
            {
                IsStateFinished = true;
                //Debug.Log("Combat Finished");
            });
        }

        public override void PauseState() { }
        public override void ResumeState()
        {
            isMovingToLastKnown = false;
            TryChaseTarget();
        }

        public override void ExitState()
        {
            isMovingToLastKnown = false;
            //Debug.Log("exit combat");
        }
    }
}