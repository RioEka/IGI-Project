using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Alert", menuName = "SO/Enemy State/Alert")]
    public class EnemyAlert : EnemyBaseState
    {
        public override void EnterState(EnemyBrain brain)
        {
            base.EnterState(brain);

            brain.Controller.CancelCurrentMove();
            brain.stateMemory.alertPosition = brain.suspiciousLocation;
            brain.stateMemory.idleTimer = 0f;

            Debug.Log(brain.name + "EnterAlert");

            brain.Controller.LookAt(brain.stateMemory.alertPosition);

            if (brain.HasBeenAlerted)
            {
                MoveToAlertLocation(brain);
            }
        }

        public override void UpdateState(EnemyBrain brain)
        {
            base.UpdateState(brain);
            var alertMem = brain.stateMemory;

            if (!brain.HasBeenAlerted)
            {
                //if ((brain.suspiciousLocation - alertMem.alertPosition).sqrMagnitude > 0.5f)
                //{
                //    alertMem.alertPosition = brain.suspiciousLocation;
                //}

                brain.Controller.LookAt(brain.suspiciousLocation);

                alertMem.idleTimer += Time.deltaTime;
                if (alertMem.idleTimer >= brain.Controller.IdleDuration)
                {
                    brain.HasBeenAlerted = true;
                    MoveToAlertLocation(brain);
                }
            }
            else
            {
                //if ((brain.suspiciousLocation - alertMem.alertPosition).sqrMagnitude > 0.5f)
                //{
                //    alertMem.alertPosition = brain.suspiciousLocation;
                //}
                    MoveToAlertLocation(brain);
            }
        }

        private void MoveToAlertLocation(EnemyBrain brain)
        {
            brain.Controller.MoveTowards(brain.suspiciousLocation).OnArrive(() =>
            {
                brain.stateMemory.IsStateFinished = true;
                brain.Controller.LookAt(brain.suspiciousLocation);
            });
        }

        public override void PauseState(EnemyBrain brain)
        {
            base.PauseState(brain);
        }

        public override void ResumeState(EnemyBrain brain)
        {
            base.ResumeState(brain);
            MoveToAlertLocation(brain);
        }

        public override void ExitState(EnemyBrain brain)
        {
            brain.HasBeenAlerted = false;
        }
    }
}