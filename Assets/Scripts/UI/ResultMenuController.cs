using UnityEngine;

public class ResultMenuController : MonoBehaviour
{
    [SerializeField] private GameObject resultMenuRoot = null;
    [SerializeField] private SceneLoader sceneLoader = null;

    private void Awake()
    {
        FindSceneLoaderIfNeeded();
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        if (resultMenuRoot != null)
        {
            resultMenuRoot.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (resultMenuRoot != null)
        {
            resultMenuRoot.SetActive(false);
        }
    }

    public void FinishGame()
    {
        Time.timeScale = 1f;
        FindSceneLoaderIfNeeded();

        if (sceneLoader == null)
        {
            Debug.LogWarning($"{nameof(ResultMenuController)}: SceneLoader reference is missing.");
            return;
        }

        sceneLoader.LoadCredits();
    }

    public void ReturnToSelection()
    {
        Time.timeScale = 1f;
        FindSceneLoaderIfNeeded();

        if (sceneLoader == null)
        {
            Debug.LogWarning($"{nameof(ResultMenuController)}: SceneLoader reference is missing.");
            return;
        }

        sceneLoader.LoadCharacterSelection();
    }

    private void FindSceneLoaderIfNeeded()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindFirstObjectByType<SceneLoader>();
        }
    }
}
