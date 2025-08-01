using UnityEngine;

using IGI.Enemy;

namespace IGI.Transition
{
    [CreateAssetMenu(fileName = "StateFinished", menuName = "SO/Transition/StateFinished")]
    public class StateFinished : TransitionCondition
    {
        public override bool TrueCondition(EnemyBrain brain)
        {
            return brain.stateMemory.IsStateFinished;
        }
    }
}