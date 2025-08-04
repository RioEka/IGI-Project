using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace IGI.Interaction
{
    public class InteractionSystem : MonoBehaviour
    {
        [SerializeField] private RectTransform interactUI;
        [SerializeField] private Image progressBar;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private InputActionReference inputAction;
        
        private Interactable interactable;
        private Player.PlayerInput input;

        private Vector3 screenPos;
        private float holdStartTime;
        private bool isHolding, wasHolding;
        private string description;

        public static InteractionSystem Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            input = FindFirstObjectByType<Player.PlayerInput>();

            string display = inputAction.action.GetBindingDisplayString();
            description = $"{display}";
            descriptionText.text = description;
            UpdateProgressBar(0);
            Uninteract();
        }

        private void Update()
        {
            if (interactable == null)
            {
                if (wasHolding) ResetHold();
                return;
            }

            isHolding = input.interact;
            UpdatePositionUI();

            if (isHolding && !wasHolding) holdStartTime = Time.time;

            if (isHolding)
            {
                float progress = (Time.time - holdStartTime) / interactable.HoldDuration;
                progress = Mathf.Clamp01(progress);
                UpdateProgressBar(progress);

                if (progress >= 1f)
                {
                    interactable.OnInteract();
                    Uninteract();
                }
            }
            else ResetHold();

            wasHolding = isHolding;
        }

        private void ResetHold()
        {
            isHolding = false;
            wasHolding = false;
            holdStartTime = 0f;
            UpdateProgressBar(0f);
        }

        private void UpdateProgressBar(float progress) => progressBar.fillAmount = progress;

        private void UpdatePositionUI()
        {
            screenPos = Camera.main.WorldToScreenPoint(interactable.transform.position + Vector3.up * 2f);
            interactUI.position = screenPos;
        }

        public void Interact(Interactable interactable)
        {
            this.interactable = interactable;
            interactUI.gameObject.SetActive(true);
        }    

        public void Uninteract()
        {
            interactable = null;
            interactUI.gameObject.SetActive(false);
            //Debug.Log("gg");
        }
    }
}