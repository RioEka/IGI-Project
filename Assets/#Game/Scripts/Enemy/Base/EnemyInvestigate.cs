using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Investigate", menuName = "SO/Enemy State/Investigate")]
    public class EnemyInvestigate : EnemyBaseState
    {
        [SerializeField] private float radiusInvestigate = 5f;

        public override void EnterState(EnemyBrain brain)
        {
            base.EnterState(brain);

            var mem = brain.stateMemory;
            mem.currentIndex = 0;
            mem.isWaiting = false;

            GenerateInvestigatePoints(brain);
            MoveToNextPoint(brain);
        }

        public override void UpdateState(EnemyBrain brain)
        {
            base.UpdateState(brain);

            var mem = brain.stateMemory;

            if (mem.isWaiting)
            {
                mem.scanTimer += Time.deltaTime;

                brain.Controller.LookAt(
                    brain.transform.position + mem.lookDirections[mem.lookIndex] * 2f
                );

                if (mem.scanTimer >= 1.2f)
                {
                    mem.scanTimer = 0f;
                    mem.lookIndex++;

                    if (mem.lookIndex >= mem.lookDirections.Length)
                    {
                        mem.isWaiting = false;
                        MoveToNextPoint(brain);
                    }
                }
            }
        }

        private void GenerateInvestigatePoints(EnemyBrain brain)
        {
            var mem = brain.stateMemory;
            mem.investigatePoints = new Vector3[3];
            Vector3 center = brain.suspiciousLocation;

            for (int i = 0; i < mem.investigatePoints.Length; i++)
            {
                Vector2 offset = Random.insideUnitCircle * radiusInvestigate;
                Vector3 point = center + new Vector3(offset.x, 0, offset.y);
                mem.investigatePoints[i] = point;
            }
        }

        private void MoveToNextPoint(EnemyBrain brain)
        {
            var mem = brain.stateMemory;

            if (mem.currentIndex >= mem.investigatePoints.Length)
            {
                brain.stateMemory.IsStateFinished = true;
                mem.currentIndex = 0;
                return;
            }

            Vector3 next = mem.investigatePoints[mem.currentIndex++];
            brain.Controller.MoveTowards(next).OnArrive(() =>
            {
                BeginLookAround(brain);
            });
        }

        private void BeginLookAround(EnemyBrain brain)
        {
            var mem = brain.stateMemory;
            mem.isWaiting = true;
            mem.scanTimer = 0f;
            mem.lookIndex = 0;

            mem.lookDirections = new Vector3[3];
            float[] angles = { -45f, 0f, 45f };

            for (int i = 0; i < angles.Length; i++)
            {
                Vector3 dir = Quaternion.Euler(0, angles[i], 0) * brain.transform.forward;
                mem.lookDirections[i] = dir.normalized;
            }
        }

        public override void PauseState(EnemyBrain brain)
        {
            base.PauseState(brain);
            brain.Controller.CancelCurrentMove();
        }

        public override void ResumeState(EnemyBrain brain)
        {
            base.ResumeState(brain);
            MoveToNextPoint(brain);
        }

        public override void ExitState(EnemyBrain brain)
        {
            brain.Controller.CancelCurrentMove();
        }
    }
}