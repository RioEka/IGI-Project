using UnityEngine;

namespace IGI.Enemy
{
    [CreateAssetMenu(fileName = "Investigate", menuName = "SO/Enemy State/Investigate")]
    public class EnemyInvestigate : EnemyBaseState
    {
        [SerializeField] private float radiusInvestigate = 5f;

        private Vector3[] investigatePoints;
        private int currentIndex;
        private bool isWaiting;

        private float scanTimer;
        private Vector3[] lookDirections;
        private int lookIndex;

        public override void Initialize(EnemyBrain brain)
        {
            base.Initialize(brain);
            State = EnemyBrain.EnemyState.Investigate;
        }

        public override void EnterState()
        {
            base.EnterState();
            currentIndex = 0;
            GenerateInvestigatePoints();
            MoveToNextPoint();
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if (isWaiting)
            {
                scanTimer += Time.deltaTime;

                brain.Controller.LookAt(
                    brain.transform.position + lookDirections[lookIndex] * 2f
                );

                if (scanTimer >= 1.2f)
                {
                    scanTimer = 0f;
                    lookIndex++;

                    if (lookIndex >= lookDirections.Length)
                    {
                        isWaiting = false;
                        MoveToNextPoint();
                    }
                }
            }
        }

        private void GenerateInvestigatePoints()
        {
            investigatePoints = new Vector3[3];
            Vector3 center = brain.suspiciousLocation;

            for (int i = 0; i < investigatePoints.Length; i++)
            {
                Vector2 offset = Random.insideUnitCircle * radiusInvestigate;
                Vector3 point = center + new Vector3(offset.x, 0, offset.y);
                investigatePoints[i] = point;
            }
        }

        private void MoveToNextPoint()
        {
            if (currentIndex >= investigatePoints.Length)
            {
                IsStateFinished = true;
                currentIndex = 0;
            }

            Vector3 next = investigatePoints[currentIndex++];
            brain.Controller.MoveTowards(next).OnArrive(() =>
            {
                BeginLookAround();
            });
        }

        private void BeginLookAround()
        {
            isWaiting = true;
            scanTimer = 0f;
            lookIndex = 0;

            lookDirections = new Vector3[3];
            float[] angles = { -45f, 0f, 45f };

            for (int i = 0; i < angles.Length; i++)
            {
                Vector3 dir = Quaternion.Euler(0, angles[i], 0) * brain.transform.forward;
                lookDirections[i] = dir.normalized;
            }
        }

        public override void PauseState() { }
        public override void ResumeState() { MoveToNextPoint(); }

        public override void ExitState()
        {
        }
    }
}