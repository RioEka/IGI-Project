using UnityEngine.InputSystem;

namespace IGI.Player
{
    public class PlayerInput : StarterAssets.StarterAssetsInputs
    {
        public bool crouch;
        public bool interact;
        public bool attack;

        public void OnCrouch(InputValue value) => Crouch(value.isPressed);
        public void OnInteract(InputValue value) => Interact(value.isPressed);
        public void OnAttack(InputValue value) => Attack(value.isPressed);

        private void Crouch(bool crouch) => this.crouch = crouch;
        private void Interact(bool interact) => this.interact = interact;
        private void Attack(bool attack) => this.attack = attack;
    }
}