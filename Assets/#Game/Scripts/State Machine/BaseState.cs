using System;

namespace IGI.StateMachine
{
    public abstract class BaseState<Estate> : UnityEngine.ScriptableObject where Estate : Enum
    {
        public Estate State { get; protected set; }

        public abstract void EnterState(Enemy.EnemyBrain brain);
        public abstract void UpdateState(Enemy.EnemyBrain brain);
        public abstract void ExitState(Enemy.EnemyBrain brain);
        public virtual Estate GetNextState() { return State; }
    }
}