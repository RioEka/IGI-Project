using UnityEngine;

namespace IGI.Interaction
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private float holdDuration = 1f;
        [SerializeField] private Interactable requirement;

        public float HoldDuration => holdDuration;

        private void OnTriggerEnter(Collider other)
        {
            InteractionSystem.Instance.Interact(this);
        }

        private void OnTriggerExit(Collider other)
        {
            InteractionSystem.Instance.Uninteract();
        }

        public void OnInteract()
        {
            if(requirement != null)
            {
                if (!Player.PlayerInventory.Contains(requirement)) return;
            }

            gameObject.SetActive(false);
            Player.PlayerInventory.Add(this);
        }
    }

}