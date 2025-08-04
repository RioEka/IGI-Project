using UnityEngine;

using DG.Tweening;

namespace IGI.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private PlayerInput input;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody[] ragdoll;
        [SerializeField] private AudioClip[] footstepAudioClips;

        [Header("Takedown")]
        private Vector3 relativeOffsetFromEnemy = new Vector3(0.07083f, 0f, -0.54896f);
        [SerializeField] private float moveDuration = 0.25f;
        [SerializeField] private float rotateDuration = 0.2f;

        [Header("")]
        [SerializeField, Range(0, 1)] private float footstepAudioVolume = .3f;
        [SerializeField, Range(.1f, 5)] private float moveSpeed = 3f, crouchSpeed = 2f, sprintSpeed = 4f;
        [SerializeField, Range(0.0f, 0.3f)] private float rotationSpeed = .12f;
        [SerializeField] private float acceleration = 10f;

        private Enemy.EnemyController enemy;
        private Camera mainCamera;
        private PlayerMove playerMove;

        private bool isAttack;
        private int animCrouchID, animSpeedID, animMotionSpeedID, animAttackID;

        private void Awake()
        {
            mainCamera = Camera.main;
            AssignAnimationID();

            playerMove = new PlayerMove(
                controller,
                input,
                animator,
                transform,
                mainCamera,
                moveSpeed,
                sprintSpeed,
                crouchSpeed,
                rotationSpeed,
                acceleration,
                animCrouchID,
                animSpeedID,
                animMotionSpeedID
            );
        }

        private void Update()
        {
            if(input.attack)
            {
                input.attack = false;
                if (enemy == null) return;
                TriggerTakedown(enemy);
                enemy = null;
            }

            if (isAttack) return;
            playerMove.Tick();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<Enemy.EnemyController>(out var enemy))
            {
                this.enemy = enemy;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Enemy.EnemyController>(out var enemy))
            {
                this.enemy = null;
            }
        }

        private void TriggerTakedown(Enemy.EnemyController enemy)
        {
            isAttack = true;
            Vector3 targetPosition = enemy.transform.position + enemy.transform.TransformDirection(relativeOffsetFromEnemy);
            Quaternion targetRotation = Quaternion.LookRotation(enemy.transform.position - transform.position);

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutSine));
            seq.Join(transform.DORotateQuaternion(targetRotation, rotateDuration));
            seq.OnComplete(() =>
            {
                PlayTakedownAnimation(enemy);
            });
        }

        private void PlayTakedownAnimation(Enemy.EnemyController enemy)
        {
            animator.SetTrigger(animAttackID);
            enemy.TakeDamage();
        }

        private void AssignAnimationID()
        {
            animCrouchID = Animator.StringToHash("Crouch");
            animSpeedID = Animator.StringToHash("Speed");
            animMotionSpeedID = Animator.StringToHash("MotionSpeed");
            animAttackID = Animator.StringToHash("Attack");
        }

        [NaughtyAttributes.Button]
        public void TakeDamage(Vector3 vector, Rigidbody rigidbody)
        {
            animator.enabled = false;
            foreach (var item in ragdoll)
            {
                item.isKinematic = false;
                if (item.Equals(rigidbody)) item.AddForce(vector * 50f, ForceMode.Impulse);
            }
            controller.enabled = false;
            //Manager.EventCallback.OnGameOver(Manager.GameResult.Lose);
            Manager.SceneLoader.Instance.LoadScene(0);
        }

        private void OnDoneAttack() => isAttack = false;

        private void OnFootstep(AnimationEvent animationEvent)
        {
            //if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (footstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, footstepAudioClips.Length);
                    Sound.SoundSystem.EmitSound(footstepAudioClips[index], transform.position, footstepAudioVolume);
                }
            }
        }
    }
}