using UnityEngine;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader = null;
    [SerializeField] private KeyCode startKey = KeyCode.X;
    [SerializeField] private bool listenForKeyboardInput = true;

    private void Awake()
    {
        FindSceneLoaderIfNeeded();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameFlowState.Start);
        }
    }

    private void Update()
    {
        if (listenForKeyboardInput && Input.GetKeyDown(startKey))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        FindSceneLoaderIfNeeded();

        if (sceneLoader == null)
        {
            Debug.LogWarning($"{nameof(StartScreenController)}: SceneLoader reference is missing.");
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
