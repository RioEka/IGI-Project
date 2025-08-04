using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using DG.Tweening;

namespace IGI.Manager
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private Image fadeMaterial;
        [SerializeField] private float fadeDuration = 1f;

        private AsyncOperation asyncLoad;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(int sceneIndex)
        {
            // Load async
            asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
            asyncLoad.allowSceneActivation = false;
            StartCoroutine(FadeAndLoadScene(sceneIndex));
        }

        private System.Collections.IEnumerator FadeAndLoadScene(int sceneIndex)
        {
            // Fade out (alpha 0 ? 1)
            yield return fadeMaterial.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine).WaitForCompletion();

            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }

            // Fade in (alpha 1 ? 0)
            yield return fadeMaterial.DOFade(0f, fadeDuration).SetEase(Ease.InOutSine).WaitForCompletion();
        }
    }
}