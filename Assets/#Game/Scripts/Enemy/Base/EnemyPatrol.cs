using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Patrol", menuName = "SO/Enemy State/Patrol")]
    public class EnemyPatrol : EnemyBaseState
    {
        private Transform[] waypoints;

        private int currentWaypointIndex = 0;
        private bool isWaiting = false;
        private float waitTimer, waitDuration = 0f;

        public override void Initialize(EnemyBrain brain)
        {
            base.Initialize(brain);
            State = EnemyBrain.EnemyState.Patrol;
            waypoints = brain.Controller.Waypoints;
            waitDuration = brain.Controller.IdleDuration;
        }

        public override void EnterState()
        {
            base.EnterState();
            currentWaypointIndex = 0;
            waitTimer = 0f;
            MoveToNextWaypoint();
        }

        public override void ExitState()
        {
        }

        //public EnemyBrain.EnemyState GetNextState()
        //{
        //    //masih hardcode
        //    if (brain.FieldOfView.TargetOnCaught != null) return EnemyBrain.EnemyState.Alert;

        //    return State;
        //}

        public override void UpdateState()
        {
            base.UpdateState();

            if (isWaiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitDuration)
                {
                    waitTimer = 0f;
                    isWaiting = false;
                    MoveToNextWaypoint();
                }
            }      
        }

        private void MoveToNextWaypoint()
        {
            if (waypoints.Length == 0) return;

            brain.Controller.MoveTowards(waypoints[currentWaypointIndex].position)
                .OnArrive(() =>
                {
                    waitTimer = 0f;
                    isWaiting = true;

                    currentWaypointIndex++;
                    if (currentWaypointIndex >= waypoints.Length)
                    {
                        currentWaypointIndex = 0;
                        IsStateFinished = true;
                    }
                });
        }

        public override void PauseState()
        {
            
        }

        public override void ResumeState()
        {
            MoveToNextWaypoint();
        }
    }
}