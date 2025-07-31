using IGI.Transition;
using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Alert", menuName = "SO/Enemy State/Alert")]
    public class EnemyAlert : EnemyBaseState
    {
        private Vector3 alertPosition;
        private float idleTimer;

        public override void Initialize(EnemyBrain brain)
        {
            base.Initialize(brain);
            State = EnemyBrain.EnemyState.Alert;
        }

        public override void EnterState()
        {
            base.EnterState();

            brain.Controller.CancelCurrentMove();
            alertPosition = brain.suspiciousLocation;
            idleTimer = 0f;

            brain.Controller.LookAt(alertPosition);

            if (!brain.HasBeenAlerted)
            {
            }
            else
            {
                MoveToAlertLocation();
            }
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if (!brain.HasBeenAlerted)
            {
                if ((brain.suspiciousLocation - alertPosition).sqrMagnitude > 0.5f)
                {
                    alertPosition = brain.suspiciousLocation;
                }

                brain.Controller.LookAt(alertPosition);

                idleTimer += Time.deltaTime;
                if (idleTimer >= brain.Controller.IdleDuration)
                {
                    brain.HasBeenAlerted = true;
                    MoveToAlertLocation();
                }
            }
            else
            {
                if ((brain.suspiciousLocation - alertPosition).sqrMagnitude > 0.5f)
                {
                    alertPosition = brain.suspiciousLocation;
                    MoveToAlertLocation();
                }
            }

        }

        private void MoveToAlertLocation()
        {
            brain.Controller.MoveTowards(alertPosition).OnArrive(() =>
            {
                IsStateFinished = true;
            });
        }

        public override void PauseState() { }
        public override void ResumeState() { MoveToAlertLocation(); }

        public override void ExitState()
        {
            brain.HasBeenAlerted = false;
        }
    }
}