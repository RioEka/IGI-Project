using UnityEngine;
using UnityEngine.AI;
using UnityEngine.LowLevel;

namespace IGI.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private Transform indicator;
        [SerializeField] private AudioClip[] footstepAudioClips;
        [SerializeField] private AudioClip shootAudioClip;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform sightPosition;
        [SerializeField] private LayerMask shootTargetLayer;
        [SerializeField] private Interaction.Interactable dropItem;

        [Range(0, 1)]
        [SerializeField] private float rotationSpeed = .12f;
        [Min(0)]
        [SerializeField] private float idleDuration = 3f;
        [Range(0, 1)]
        [SerializeField] private float footstepAudioVolume = .3f;

        public Transform[] Waypoints => waypoints;
        public SoundDetection SoundDetection => soundDetection;

        public float IdleDuration => idleDuration;
        public bool isShoot { get; private set; }

        private EnemyMove move;
        private Vector3 lookTarget;
        private SoundDetection soundDetection;
        private EnemyBrain brain;

        private int animSpeedID, animShootID, animAlertID;
        private float rotationVelocity;

        private void Awake()
        {
            AssignAnimationID();
            agent.updateRotation = false;
            brain = GetComponent<EnemyBrain>();
            soundDetection = GetComponent<SoundDetection>();
        }

        private void Update()
        {
            animator.SetFloat(animSpeedID, agent.velocity.magnitude);
            animator.SetBool(animShootID, isShoot);
            //animator.SetBool(animAlertID, brain.HasBeenAlerted);

            if (lookTarget != Vector3.zero) RotateTowardsTarget();
            if(agent.desiredVelocity != Vector3.zero) RotateTowards(agent.desiredVelocity);
            move?.ArrivalCheck();
        }

        public void SetMoveSpeed(float speed) => agent.speed = speed;
        public void LookAt(Vector3 lookTarget) => this.lookTarget = lookTarget;
        public void Shooting(bool shoot) => isShoot = shoot;

        private void RotateTowardsTarget()
        {
            Vector3 direction = (lookTarget - transform.position).normalized;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
                RotateTowards(direction);

            float angle = Vector3.Angle(transform.forward, direction);
            if (angle < 1) lookTarget = Vector3.zero;
        }

        public EnemyMove MoveTowards(Vector3 destination)
        {
            //Debug.Log(gameObject.name + ": Move Towards");

            if (move != null)
            {
                Debug.LogWarning($"[EnemyController: {name}] MoveTowards called while another move is active. Forcing overwrite.");
                agent.ResetPath();
            }

            move = new(agent, destination);
            move.onComplete = (self) => move = null;
            return move;
        }

        public void TakeDamage()
        {
            enabled = false;
            brain.enabled = false;
            brain.FieldOfView.ViewMeshRenderer.enabled = false;
            brain.AttackRange.ViewMeshRenderer.enabled = false;

            animator.SetTrigger("Dead");

            Interaction.Interactable interactable = Instantiate(dropItem, transform.position, Quaternion.identity);
            interactable.gameObject.SetActive(true);
        }

        public void CancelCurrentMove()
        {
            agent.ResetPath();
            move = null;
        }

        private void RotateTowards(Vector3 direction)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationVelocity,
                rotationSpeed
            );
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        public bool StillOnMove() => move != null;

        private void Shoot()
        {
            isShoot = false;
            Sound.SoundSystem.EmitSound(shootAudioClip, transform.position, footstepAudioVolume, 5, soundDetection);
            Vector3 origin = sightPosition.position;
            Vector3 direction = sightPosition.forward;

            RaycastHit hit;
            Vector3 endPoint = origin + direction * 50;

            if (Physics.Raycast(origin, direction, out hit, 50, shootTargetLayer))
            {
                endPoint = hit.point;
                Debug.Log(hit.collider.name);
                Player.PlayerController player = hit.collider.GetComponentInParent<Player.PlayerController>();
                if (player != null)
                {
                    Vector3 hitDir = (hit.transform.position - transform.position).normalized;
                    player.TakeDamage(hitDir, hit.rigidbody);
                    //hit.rigidbody.AddForce(hitDir * 100f, ForceMode.Impulse);
                }
            }
            else return;

            StartCoroutine(BulletTracerLerp(origin, endPoint)); //  pass origin juga
        }

        private System.Collections.IEnumerator BulletTracerLerp(Vector3 origin, Vector3 target)
        {
            float elapsedTime = 0f;
            lineRenderer.enabled = true;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * 8f;
                Vector3 currentPos = Vector3.Lerp(origin, target, elapsedTime);

                lineRenderer.SetPosition(0, origin); //  static start point
                lineRenderer.SetPosition(1, currentPos);
                yield return null;
            }

            lineRenderer.enabled = false;
        }

        private void AssignAnimationID()
        {
            animSpeedID = Animator.StringToHash("Speed");
            animShootID = Animator.StringToHash("Shoot");
            animAlertID = Animator.StringToHash("Alert");
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (footstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, footstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.position, footstepAudioVolume);
                }
            }
        }

        private void OnShooting(AnimationEvent animationEvent)
        {
            Shoot();
        }
    }
}