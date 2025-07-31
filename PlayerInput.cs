using UnityEngine.InputSystem;

namespace IGI.Player
{
    public class PlayerInput : StarterAssets.StarterAssetsInputs
    {
        public bool crouch;

        public void OnCrouch(InputValue value) => Crouch(value.isPressed);

        private void Crouch(bool crouch) => this.crouch = crouch;
    }
}