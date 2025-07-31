using System;

namespace IGI.StateMachine
{
    public abstract class BaseState<Estate> : UnityEngine.ScriptableObject where Estate : Enum
    {
        public Estate State { get; protected set; }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public virtual Estate GetNextState() { return State; }
    }
}