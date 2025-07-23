using UnityEngine;

namespace IGI.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private PlayerInput input;
        [SerializeField] private Animator animator;

        [SerializeField, Range(.1f, 5)] private float moveSpeed = 3f, crouchSpeed = 2f;
        [SerializeField, Range(0.0f, 0.3f)] private float rotationSpeed = .12f;
        [SerializeField] private float acceleration = 10f;

        private Camera mainCamera;
        private PlayerMove playerMove;

        private int animCrouchID, animSpeedID, animMotionSpeedID;

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
            playerMove.Tick();
        }

        private void AssignAnimationID()
        {
            animCrouchID = Animator.StringToHash("Crouch");
            animSpeedID = Animator.StringToHash("Speed");
            animMotionSpeedID = Animator.StringToHash("MotionSpeed");
        }
    }
}