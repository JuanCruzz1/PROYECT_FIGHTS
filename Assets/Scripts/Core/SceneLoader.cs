using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string startSceneName = string.Empty;
    [SerializeField] private string characterSelectionSceneName = string.Empty;
    [SerializeField] private string loadingSceneName = string.Empty;
    [SerializeField] private string fightSceneName = string.Empty;
    [SerializeField] private string creditsSceneName = string.Empty;

    public void LoadStart()
    {
        LoadConfiguredScene(startSceneName, GameFlowState.Start);
    }

    public void LoadCharacterSelection()
    {
        LoadConfiguredScene(characterSelectionSceneName, GameFlowState.CharacterSelection);
    }

    public void LoadLoading()
    {
        LoadConfiguredScene(loadingSceneName, GameFlowState.Loading);
    }

    public void LoadFight()
    {
        LoadConfiguredScene(fightSceneName, GameFlowState.Fighting);
    }

    public void LoadCredits()
    {
        LoadConfiguredScene(creditsSceneName, GameFlowState.Credits);
    }

    public void LoadLoadingOrFight(bool useLoadingScene)
    {
        if (useLoadingScene && !string.IsNullOrWhiteSpace(loadingSceneName))
        {
            LoadLoading();
            return;
        }

        LoadFight();
    }

    public void LoadScene(string sceneName)
    {
        LoadConfiguredScene(sceneName, GameFlowState.Loading);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void LoadConfiguredScene(string sceneName, GameFlowState newState)
    {
        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning($"{nameof(SceneLoader)}: Scene name is not assigned.");
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(newState);
        }

        SceneManager.LoadScene(sceneName);
    }
}
