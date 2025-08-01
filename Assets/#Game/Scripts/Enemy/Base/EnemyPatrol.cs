using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Patrol", menuName = "SO/Enemy State/Patrol")]
    public class EnemyPatrol : EnemyBaseState
    {
        [SerializeField] private float waitDuration = 3f;

        public override void EnterState(EnemyBrain brain)
        {
            base.EnterState(brain);
            brain.stateMemory.currentWaypointIndex = 0;
            brain.stateMemory.waitTimer = 0f;
            MoveToNextWaypoint(brain);
        }

        public override void ExitState(EnemyBrain brain)
        {
        }

        //public EnemyBrain.EnemyState GetNextState()
        //{
        //    //masih hardcode
        //    if (brain.FieldOfView.TargetOnCaught != null) return EnemyBrain.EnemyState.Alert;

        //    return State;
        //}

        public override void UpdateState(EnemyBrain brain)
        {
            base.UpdateState(brain);

            if (brain.stateMemory.patrolIsWaiting)
            {
                brain.stateMemory.waitTimer += Time.deltaTime;
                if (brain.stateMemory.waitTimer >= waitDuration)
                {
                    brain.stateMemory.waitTimer = 0f;
                    brain.stateMemory.patrolIsWaiting = false;
                    MoveToNextWaypoint(brain);
                }
            }      
        }

        private void MoveToNextWaypoint(EnemyBrain brain)
        {
            if (brain.Controller.Waypoints.Length == 0) return;

            brain.Controller.MoveTowards(brain.Controller.Waypoints[brain.stateMemory.currentWaypointIndex].position)
                .OnArrive(() =>
                {
                    brain.stateMemory.waitTimer = 0f;
                    brain.stateMemory.patrolIsWaiting = true;

                    brain.stateMemory.currentWaypointIndex++;
                    if (brain.stateMemory.currentWaypointIndex >= brain.Controller.Waypoints.Length)
                    {
                        brain.stateMemory.currentWaypointIndex = 0;
                        brain.stateMemory.IsStateFinished = true;
                    }
                });
        }

        public override void PauseState(EnemyBrain brain)
        {
            base .PauseState(brain);
        }

        public override void ResumeState(EnemyBrain brain)
        {
            base.ResumeState(brain);
            MoveToNextWaypoint(brain);
        }
    }
}