using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Alert", menuName = "SO/Enemy State/Alert")]
    public class EnemyAlert : EnemyBaseState
    {
        [SerializeField] private float proximityCheckDistance = 2f;

        public override void EnterState(EnemyBrain brain)
        {
            base.EnterState(brain);

            brain.Controller.CancelCurrentMove();
            brain.stateMemory.alertPosition = brain.suspiciousLocation;
            brain.stateMemory.idleTimer = 0f;

            //Debug.Log(brain.name + "EnterAlert");

            brain.Controller.LookAt(brain.stateMemory.alertPosition);

            if (brain.HasBeenAlerted)
            {
                MoveToAlertLocation(brain);
            }
        }

        public override void UpdateState(EnemyBrain brain)
        {
            base.UpdateState(brain);

            Transform player = brain.FieldOfView.TargetNoDelete;
            if (player != null)
            {
                float sqrDistance = (player.position - brain.transform.position).sqrMagnitude;

                if (sqrDistance < proximityCheckDistance * proximityCheckDistance)
                {
                    brain.Controller.LookAt(player.position);
                    brain.SetSuspiciousLocation(player.position);
                    // Debug.LogWarning($"{brain.name}: Player sangat dekat dalam state ALERT!");
                    //return;
                }
            }

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