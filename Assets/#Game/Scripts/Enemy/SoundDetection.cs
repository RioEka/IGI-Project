using UnityEngine;

namespace IGI.Enemy
{
    public class SoundDetection : MonoBehaviour
    {
        [SerializeField] private float hearingRadius = 10f;

        public Vector3? LastHeardSound { get; private set; }

        public Collider[] target { get; private set; } = new Collider[1];

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

        public Vector3? EyeOnTarget(LayerMask layerMask)
        {
            float detectionRadius = 10f;
            Vector3 eyePosition = transform.position + Vector3.up * 1.5f;

            int hitCount = Physics.OverlapSphereNonAlloc(eyePosition, detectionRadius, target, layerMask);

            if (hitCount > 0)
            {
                return target[0].transform.position;
            }

            return null;
        }
    }
}