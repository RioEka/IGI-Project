using UnityEngine;

namespace IGI.Transition
{
    [CreateAssetMenu(fileName = "PlayerInsideAttackRange", menuName = "SO/Transition/PlayerInsideAttackRange")]
    public class PlayerInsideAttackRange : TransitionCondition
    {
        public override bool TrueCondition(Enemy.EnemyBrain brain)
        {
            if (brain.AttackRange.TargetOnCaught != null)
            {
                brain.SetSuspiciousLocation(brain.FieldOfView.TargetOnCaught.position);
                return true;
            }
            return false;
        }
    }
}