using UnityEngine;

namespace IGI.Transition
{
    [CreateAssetMenu(fileName = "PlayerOnCaught", menuName = "SO/Transition/PlayerOnCaught")]
    public class PlayerOnCaught : TransitionCondition
    {
        public override bool TrueCondition(Enemy.EnemyBrain brain)
        {
            if(brain.FieldOfView.TargetOnCaught != null)
            {
                brain.SetSuspiciousLocation(brain.FieldOfView.TargetOnCaught.position);
                return true;
            }
            return false;
        }
    }
}