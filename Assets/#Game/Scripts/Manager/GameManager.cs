using UnityEngine;

namespace IGI.Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;

        public static Transform PlayerTransform { get; private set; }

        private void Awake()
        {
            PlayerTransform = playerTransform;
        }
    }
}