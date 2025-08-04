using UnityEngine;

namespace IGI.Interaction
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private float holdDuration = 1f;
        [SerializeField] private Interactable[] requirements;
        [SerializeField] private int cutsceneIndex = 0;
        [SerializeField] private GameObject[] objectsToSpawn;

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
            if(requirements.Length > 0)
            {
                foreach (var item in requirements)
                {
                    if (!Player.PlayerInventory.Contains(item)) return;
                }
            }

            foreach (var item in objectsToSpawn)
            {
                item.SetActive(true);
            }

            gameObject.SetActive(false);
            Player.PlayerInventory.Add(this);

            if(cutsceneIndex > 0) Manager.TimelineManager.Instance.PlayCutscene(cutsceneIndex - 1);
        }
    }

}