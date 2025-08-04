using UnityEngine;

public enum GameState
{
    MainMenu,
    Cutscene1,
    GameplayStage1,
    Cutscene2,
    GameplayStage2,
    Cutscene3,
    Cutscene4,
    GameOver
}

public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance;
    public GameState CurrentState = GameState.MainMenu;

    [Header("UI")]
    public GameObject mainMenuUI;

    [Header("Triggers")]
    public GameObject triggerToCutscene2;
    public GameObject escapeAreaTrigger;
    public GameObject documentItem;
    public GameObject crowbarItem;
    public GameObject keyItem;
    public GameObject carBoxInteraction;

    [Header("Spawns")]
    public GameObject enemySpawner;
    public GameObject escapeArea;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetGameState(GameState.MainMenu);
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        UpdateGameState();
        Debug.Log("Game state changed to: " + CurrentState);
    }

    public void UpdateGameState()
    {
        switch (CurrentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0f;
                mainMenuUI?.SetActive(true);
                break;

            case GameState.Cutscene1:
                Time.timeScale = 1f;
                mainMenuUI?.SetActive(false);
                PlayCutscene("Cutscene1", () => SetGameState(GameState.GameplayStage1));
                break;

            case GameState.GameplayStage1:
                // Enable player movement, enemies inactive
                EnableTrigger(triggerToCutscene2, true);
                break;

            case GameState.Cutscene2:
                PlayCutscene("Cutscene2", () =>
                {
                    SetGameState(GameState.GameplayStage2);
                });
                break;

            case GameState.GameplayStage2:
                enemySpawner?.SetActive(true);
                escapeArea?.SetActive(true);
                EnableObject(documentItem, true);
                break;

            case GameState.Cutscene3:
                PlayCutscene("Cutscene3", () =>
                {
                    EnableObject(carBoxInteraction, true);
                    SetGameState(GameState.Cutscene4); // or wait for interaction
                });
                break;

            case GameState.Cutscene4:
                PlayCutscene("Cutscene4", () =>
                {
                    SetGameState(GameState.GameOver);
                });
                break;

            case GameState.GameOver:
                Debug.Log("Game Over!");
                break;
        }
    }

    private void PlayCutscene(string cutsceneName, System.Action onFinished)
    {
        // Gantilah ini dengan sistem Timeline, signal, atau event
        Debug.Log("Playing cutscene: " + cutsceneName);
        StartCoroutine(SimulateCutsceneCoroutine(3f, onFinished));
    }

    private System.Collections.IEnumerator SimulateCutsceneCoroutine(float duration, System.Action onFinished)
    {
        yield return new WaitForSeconds(duration);
        onFinished?.Invoke();
    }

    public void OnTriggerCutscene2()
    {
        if (CurrentState == GameState.GameplayStage1)
        {
            SetGameState(GameState.Cutscene2);
        }
    }

    public void OnDocumentCollected()
    {
        if (CurrentState == GameState.GameplayStage2)
        {
            EnableObject(crowbarItem, true);
            EnableObject(keyItem, true);
            EnableObject(carBoxInteraction, true);
        }
    }

    public void OnEscapeAreaReached()
    {
        if (CurrentState == GameState.GameplayStage2)
        {
            SetGameState(GameState.Cutscene3);
        }
    }

    public void OnCarBoxInteraction()
    {
        if (CurrentState == GameState.Cutscene4)
        {
            SetGameState(GameState.GameOver);
        }
    }

    private void EnableTrigger(GameObject obj, bool state)
    {
        if (obj != null) obj.SetActive(state);
    }

    private void EnableObject(GameObject obj, bool state)
    {
        if (obj != null) obj.SetActive(state);
    }

    // Dipanggil dari UI Button Play
    public void OnPlayButtonPressed()
    {
        SetGameState(GameState.Cutscene1);
    }
}
