using UnityEngine;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader = null;

    private void Awake()
    {
        FindSceneLoaderIfNeeded();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameFlowState.Credits);
        }
    }

    public void Exit()
    {
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

    private void FindSceneLoaderIfNeeded()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindFirstObjectByType<SceneLoader>();
        }
    }
}
