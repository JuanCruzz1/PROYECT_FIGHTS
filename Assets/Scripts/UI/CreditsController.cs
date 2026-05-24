using System.Collections;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader = null;
    [SerializeField] private float exitDelay = 0.40f;

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
        StartCoroutine(ExitAfterDelay());
    }

    private IEnumerator ExitAfterDelay()
    {
        yield return new WaitForSecondsRealtime(exitDelay);

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