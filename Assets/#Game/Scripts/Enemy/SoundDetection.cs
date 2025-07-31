using UnityEngine;

namespace IGI.Enemy
{
    public class SoundDetection : MonoBehaviour
    {
        [SerializeField] private float hearingRadius = 10f;

        public Vector3? LastHeardSound { get; private set; }

        private void OnEnable()
        {
            Sound.SoundSystem.RegisterListener(this);
        }

        private void OnDisable()
        {
            Sound.SoundSystem.UnregisterListener(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, hearingRadius);
        }

        public void HearSound(Vector3 soundPosition, float intensity)
        {
            float distance = Vector3.Distance(transform.position, soundPosition);
            if (distance <= hearingRadius * intensity)
            {
                LastHeardSound = soundPosition;
            }
        }

        public Vector3? ConsumeSound()
        {
            Vector3? sound = LastHeardSound;
            LastHeardSound = null;
            return sound;
        }

        public void ClearSound()
        {
            LastHeardSound = null;
        }
    }
}