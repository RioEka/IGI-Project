using UnityEngine;
using UnityEngine.Playables;

namespace IGI.Manager
{
    public class TimelineManager : MonoBehaviour
    {
        public static TimelineManager Instance { get; private set; }

        [SerializeField] private PlayableDirector[] playableDirector;
        [SerializeField] private Camera cameraCutscene;

        private int index = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (playableDirector[index].state == PlayState.Paused)
            {
                //gameObject.SetActive(false);
                playableDirector[index].gameObject.SetActive(false);
                cameraCutscene.enabled = false;
            }
        }

        public void PlayCutscene(int index)
        {
            this.index = index;
            playableDirector[index].gameObject.SetActive(true);
            cameraCutscene.enabled = true;
        }
    }
}