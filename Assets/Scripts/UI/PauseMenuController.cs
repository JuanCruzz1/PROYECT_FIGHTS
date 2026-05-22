using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuRoot = null;
    [SerializeField] private SceneLoader sceneLoader = null;
    [SerializeField] private KeyCode pauseKey = KeyCode.P;
    [SerializeField] private bool allowPause = true;

    private bool isPaused;

    private void Awake()
    {
        FindSceneLoaderIfNeeded();
    }

    private void Start()
    {
        SetPauseMenuVisible(false);
    }

    private void Update()
    {
        if (allowPause && CanTogglePause() && Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SetPauseMenuVisible(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameFlowState.Paused);
        }
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetPauseMenuVisible(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameFlowState.Fighting);
        }
    }

    public void ReturnToSelection()
    {
        Time.timeScale = 1f;
        isPaused = false;
        FindSceneLoaderIfNeeded();

        if (sceneLoader == null)
        {
            Debug.LogWarning($"{nameof(PauseMenuController)}: SceneLoader reference is missing.");
            return;
        }

        sceneLoader.LoadCharacterSelection();
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        isPaused = false;
        FindSceneLoaderIfNeeded();

        if (sceneLoader != null)
        {
            sceneLoader.ExitGame();
        }
        else
        {
            Application.Quit();
        }
    }

    private void SetPauseMenuVisible(bool visible)
    {
        if (pauseMenuRoot != null)
        {
            pauseMenuRoot.SetActive(visible);
        }
    }

    private bool CanTogglePause()
    {
        if (GameManager.Instance == null)
        {
            return true;
        }

        GameFlowState state = GameManager.Instance.CurrentState;
        return state == GameFlowState.Fighting || state == GameFlowState.Paused;
    }

    private void FindSceneLoaderIfNeeded()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindFirstObjectByType<SceneLoader>();
        }
    }
}
