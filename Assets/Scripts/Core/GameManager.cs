using UnityEngine;

public enum GameFlowState
{
    Start,
    CharacterSelection,
    Loading,
    Fighting,
    Paused,
    PlayerWin,
    PlayerLose,
    ResultMenu,
    Credits
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CharacterData SelectedCharacter { get; private set; }
    public GameObject SelectedCharacterPrefab { get; private set; }
    public GameFlowState CurrentState { get; private set; } = GameFlowState.Start;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSelectedCharacter(CharacterData character, GameObject characterPrefab)
    {
        SelectedCharacter = character;
        SelectedCharacterPrefab = characterPrefab;
    }

    public void SetGameState(GameFlowState newState)
    {
        CurrentState = newState;
    }
}
