using UnityEngine;

namespace IGI.Manager
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayButton() => SceneLoader.Instance.LoadScene(1);
    }
}