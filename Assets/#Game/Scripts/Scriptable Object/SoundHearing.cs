using UnityEngine;

using IGI.Enemy;

namespace IGI.Transition
{
    [CreateAssetMenu(fileName = "SoundHearing", menuName = "SO/Transition/SoundHearing")]
    public class SoundHearing : TransitionCondition
    {
        public override bool TrueCondition(EnemyBrain brain)
        {
            Vector3? heard = brain.SoundDetection.ConsumeSound();

            if (heard.HasValue)
            {
                // ini terbaca true
                Debug.Log("Gg");
                brain.SetSuspiciousLocation(heard.Value);
                return true;
            }

            return false;
        }
    }
}