using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "FightScene";
    [SerializeField] private float delaySeconds = 6f;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameFlowState.Loading);
        }

        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogWarning($"{nameof(LoadingScreenController)}: nextSceneName is not assigned.");
            yield break;
        }

        yield return new WaitForSecondsRealtime(Mathf.Max(0f, delaySeconds));
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}
