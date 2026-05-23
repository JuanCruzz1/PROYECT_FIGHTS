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
    public FighterCharacter SelectedFighterCharacter { get; private set; } = FighterCharacter.FerchoPoker;
    public GameFlowState CurrentState { get; private set; } = GameFlowState.Start;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log($"{nameof(GameManager)}: Duplicate found on {name}. Keeping existing selection {Instance.SelectedFighterCharacter} and destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"{nameof(GameManager)}: Instance ready. Current selected fighter: {SelectedFighterCharacter}.");
    }

    public void SetSelectedCharacter(CharacterData character, GameObject characterPrefab)
    {
        SelectedCharacter = character;
        SelectedCharacterPrefab = characterPrefab != null ? characterPrefab : character != null ? character.characterPrefab : null;

        if (character != null)
        {
            SelectedFighterCharacter = character.fighterCharacter;
        }

        Debug.Log($"{nameof(GameManager)}: SetSelectedCharacter data={(character != null ? character.name : "None")}, prefab={(SelectedCharacterPrefab != null ? SelectedCharacterPrefab.name : "None")}, fighter={SelectedFighterCharacter}.");
    }

    public void SetSelectedFighterCharacter(FighterCharacter fighterCharacter)
    {
        SelectedFighterCharacter = fighterCharacter;
        Debug.Log($"{nameof(GameManager)}: SelectedFighterCharacter saved as {SelectedFighterCharacter}.");
    }

    public void SetGameState(GameFlowState newState)
    {
        CurrentState = newState;
    }
}
