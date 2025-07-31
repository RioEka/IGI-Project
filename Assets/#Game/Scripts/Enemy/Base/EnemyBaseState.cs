using IGI.StateMachine;

namespace IGI.Enemy
{
    public abstract class EnemyBaseState : BaseState<EnemyBrain.EnemyState>
    {
        protected EnemyBrain brain { get; private set; }
        protected FieldOfView FieldOfView { get; private set; }
        protected FieldOfView AttackRange { get; private set; }

        public float timeInCurrentState { get; protected set; }
        public bool IsStateFinished { get; protected set; }

        public virtual void Initialize(EnemyBrain brain)
        {
            this.brain = brain;
            FieldOfView = brain.FieldOfView;
            AttackRange = brain.AttackRange;
        }

        public override void EnterState()
        {
            IsStateFinished = false;
            timeInCurrentState = 0;
        }

        public override void UpdateState() => timeInCurrentState += UnityEngine.Time.deltaTime;

        public abstract void PauseState();
        public abstract void ResumeState();
    }
}