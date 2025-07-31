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

            foreach (var item in stateVariants)
            {
                item.baseState.Initialize(this);
                states.Add(item.baseState.State, item.baseState);
            }

            currentAIState = stateVariants[0];
            currentState = currentAIState.baseState;
        }

        private void Start()
        {
            currentAIState.baseState.EnterState();
        }

        private void Update()
        {
            Debug.Log(currentAIState.baseState.name);

            currentAIState.baseState.UpdateState();
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

            currentAIState.baseState.ExitState();

            currentAIState = stateVariants[index];
            currentState = currentAIState.baseState;

            currentAIState.baseState.EnterState();
        }

        public void Alert(Vector3 position)
        {

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
            pendingState.baseState.PauseState();
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
            currentAIState.baseState.ResumeState();
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
}