using UnityEngine.InputSystem;

namespace IGI.Player
{
    public class PlayerInput : StarterAssets.StarterAssetsInputs
    {
        public bool crouch;
        public bool interact;

        public void OnCrouch(InputValue value) => Crouch(value.isPressed);
        public void OnInteract(InputValue value) => Interact(value.isPressed);

        private void Crouch(bool crouch) => this.crouch = crouch;
        private void Interact(bool interact) => this.interact = interact;
    }
}