using UnityEngine;

namespace IGI.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        private static PlayerInventory instance;

        private System.Collections.Generic.List<Interaction.Interactable> collection = new();

        private void Awake()
        {
            instance = this;    
        }

        public static void Add(Interaction.Interactable item) => instance.collection.Add(item);
        public static bool Contains(Interaction.Interactable item) => instance.collection.Contains(item);
    }
}