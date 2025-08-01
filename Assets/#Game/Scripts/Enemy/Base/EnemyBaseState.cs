using IGI.StateMachine;

namespace IGI.Enemy
{
    public abstract class EnemyBaseState : BaseState<EnemyBrain.EnemyState>
    {
        public override void EnterState(EnemyBrain brain)
        {
            brain.stateMemory.IsStateFinished = false;
            brain.stateMemory.timeInCurrentState = 0;
            brain.stateMemory.paused = false;
        }

        public override void UpdateState(EnemyBrain brain)
        {
            if (brain.stateMemory.paused) return;

            brain.stateMemory.timeInCurrentState += UnityEngine.Time.deltaTime;
        }

        public virtual void PauseState(EnemyBrain brain) => brain.stateMemory.paused = true;
        public virtual void ResumeState(EnemyBrain brain) => brain.stateMemory.paused = false;
    }
}