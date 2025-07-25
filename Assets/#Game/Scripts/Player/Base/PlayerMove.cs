using UnityEngine;

namespace IGI.Player
{
    public class PlayerMove
    {
        private readonly CharacterController controller;
        private readonly PlayerInput input;
        private readonly Animator animator;
        private readonly Camera mainCamera;
        private readonly Transform playerTransform;

        private readonly float moveSpeed;
        private readonly float crouchSpeed;
        private readonly float rotationSpeed;
        private readonly float acceleration;

        private readonly int animCrouchID, animSpeedID, animMotionSpeedID;

        private float currentSpeed, targetRotation, rotationVelocity;

        public PlayerMove(
            CharacterController controller,
            PlayerInput input,
            Animator animator,
            Transform playerTransform,
            Camera mainCamera,

            float moveSpeed,
            float crouchSpeed,
            float rotationSpeed,
            float acceleration,
            int animCrouchID,
            int animSpeedID,
            int animMotionSpeedID)
        {
            this.controller = controller;
            this.input = input;
            this.animator = animator;
            this.playerTransform = playerTransform;
            this.mainCamera = mainCamera;
            this.moveSpeed = moveSpeed;
            this.crouchSpeed = crouchSpeed;
            this.rotationSpeed = rotationSpeed;
            this.acceleration = acceleration;
            this.animCrouchID = animCrouchID;
            this.animSpeedID = animSpeedID;
            this.animMotionSpeedID = animMotionSpeedID;
        }

        public void Tick()
        {
            float targetSpeed = input.crouch ? crouchSpeed : moveSpeed;
            if (input.move.sqrMagnitude < 0.001f) targetSpeed = 0;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * acceleration);
            }
            else
            {
                currentSpeed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

            if (input.move != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSpeed);
                playerTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
            controller.Move(targetDirection.normalized * (currentSpeed * Time.deltaTime));

            SetAnimation(inputMagnitude);
        }

        private void SetAnimation(float inputMagnitude)
        {
            animator.SetFloat(animSpeedID, currentSpeed);
            animator.SetFloat(animMotionSpeedID, inputMagnitude);
            animator.SetBool(animCrouchID, input.crouch);
        }
    }
}