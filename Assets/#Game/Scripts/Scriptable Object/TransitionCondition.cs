using UnityEngine;

namespace IGI.Transition
{
    public abstract class TransitionCondition : ScriptableObject
    {
        public abstract bool TrueCondition(Enemy.EnemyBrain brain);
    }
}