using UnityEngine;

using IGI.Enemy;

namespace IGI.Transition
{
    [CreateAssetMenu(fileName = "WaitingTime", menuName = "SO/Transition/WaitingTime")]
    public class WaitingTime : TransitionCondition
    {
        [Min(-1f)]
        [SerializeField] private float duration;

        public override bool TrueCondition(EnemyBrain brain)
        {
            if(duration < 0) return false;
            if(brain.stateMemory.timeInCurrentState >= duration) return true;
            return false;
        }
    }
}