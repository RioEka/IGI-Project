using UnityEngine;

using IGI.StateMachine;

namespace IGI.Enemy
{
    public class EnemyBrain : StateManager<EnemyBrain.EnemyState>
    {
        public enum EnemyState
        {
            Idle, Patrol, Alert, Investigate, Combat
        }

        [SerializeField] private FieldOfView fieldOfView, attackRange;
        [SerializeField] private AIState[] stateVariants;

        public EnemyController Controller => controller;
        public FieldOfView FieldOfView => fieldOfView;
        public FieldOfView AttackRange => attackRange;
        public SoundDetection SoundDetection => soundDetection;
        public AIState[] StateVariants => stateVariants;
        public AIState CurrentState => currentAIState;
        public Vector3 suspiciousLocation {  get; private set; }
        //public Vector3? CachedAlertLocation { get; private set; }

        public StateMemory stateMemory { get; private set; }

        public bool HasBeenAlerted { get; set; }

        private EnemyController controller;
        private SoundDetection soundDetection;
        private AITask? pendingTask;
        private AIState pendingState;
        private AIState currentAIState;

        private void Awake()
        {
            controller = GetComponent<EnemyController>();
            soundDetection = GetComponent<SoundDetection>();
            stateMemory = new();

            //foreach (var item in stateVariants)
            //{
            //    //item.baseState.Initialize(this);
            //    states.Add(item.baseState.State, item.baseState);
            //}

            currentAIState = stateVariants[0];
            currentState = currentAIState.baseState;
        }

        private void OnEnable()
        {
            EnemyAwarenessSystem.Register(this);
        }

        private void OnDisable()
        {
            EnemyAwarenessSystem.Unregister(this);
        }

        private void Start()
        {
            currentAIState.baseState.EnterState(this);
        }

        private void Update()
        {
            Debug.Log(gameObject.name + ": " + currentAIState.baseState.name);

            if (currentAIState == null)
            {
                Debug.Log($"[{gameObject.name}] Current AI State is Null. Nothing state to play");
                return;
            }
            currentAIState.baseState.UpdateState(this);
            currentAIState.TickTransitions(this);
        }

        public void TransitionToStateIndex(int index)
        {
            if (index < 0 || index >= stateVariants.Length)
            {
                Debug.LogError("Invalid state index");
                return;
            }

            if (currentAIState.Equals(stateVariants[index])) return;

            currentAIState.baseState.ExitState(this);

            currentAIState = stateVariants[index];
            currentState = currentAIState.baseState;

            currentAIState.baseState.EnterState(this);
        }

        private int? GetStateIndex(EnemyState state)
        {
            for (int i = 0; i < stateVariants.Length; i++)
            {
                if (stateVariants[i].baseState.State == state)
                    return i;
            }
            return null;
        }

        public void SetTask(AITask task, bool allowInterruption = true)
        {
            if (allowInterruption)
                ExecuteTask(task);
            else
                if(!controller.StillOnMove()) ExecuteTask(task);
        }

        private void ExecuteTask(AITask task)
        {
            pendingState = currentAIState;
            pendingState.baseState.PauseState(this);
            currentAIState = null;

            controller.CancelCurrentMove();
            controller.MoveTowards(task.destination).OnArrive(() =>
            {
                task.task?.Invoke();
            });
        }

        public void TaskCompleted()
        {
            currentAIState = pendingState;
            currentAIState.baseState.ResumeState(this);
            pendingState = null;
        }

        public void ResumePendingTask()
        {
            if (pendingTask.HasValue)
            {
                AITask task = pendingTask.Value;
                pendingTask = null;
                ExecuteTask(task);
            }
        }

        public void OnAlertedByAlly(Vector3 alertPosition)
        {
            if (currentAIState != null && currentAIState.baseState.State == EnemyState.Combat) return;

            SetSuspiciousLocation(alertPosition);
            HasBeenAlerted = true;

            var alertIndex = GetStateIndex(EnemyState.Alert);
            if (alertIndex.HasValue) TransitionToStateIndex(alertIndex.Value);
        }

        public void SetSuspiciousLocation(Vector3 location) => suspiciousLocation = location;
    }

    public struct AITask
    {
        public Vector3 destination { get; set; }
        public System.Action task { get; set; }
        public string description { get; set; }

        public AITask(Vector3 destination, System.Action task, string description = "")
        {
            this.destination = destination;
            this.task = task;
            this.description = description;
        }
    }

    [System.Serializable]
    public class TransitionState
    {
        public Transition.TransitionCondition transition;
        public int nextStateIndex;
    }

    [System.Serializable]
    public class AIState
    {
        public EnemyBaseState baseState;
        public TransitionState[] transitions;

        public void TickTransitions(EnemyBrain brain)
        {
            foreach (var t in transitions)
            {
                if (t.transition != null && t.transition.TrueCondition(brain))
                {
                    brain.TransitionToStateIndex(t.nextStateIndex);
                    return;
                }
            }
        }
    }

    public class StateMemory
    {
        public float timeInCurrentState { get; set; }
        public bool paused { get; set; }
        public bool IsStateFinished { get; set; }

        public int currentWaypointIndex { get; set; } = 0;
        public float waitTimer { get; set; } = 0f;
        public bool patrolIsWaiting { get; set; } = false;

        public Vector3 alertPosition { get; set; }
        public float idleTimer { get; set; }

        public bool isMovingToLastKnown { get; set; }

        public Vector3? targetIdlePosition { get; set; }

        public Vector3[] investigatePoints { get; set; }
        public int currentIndex { get; set; }
        public bool isWaiting { get; set; }
        public float scanTimer { get; set; }
        public Vector3[] lookDirections { get; set; }
        public int lookIndex { get; set; }
    }
}