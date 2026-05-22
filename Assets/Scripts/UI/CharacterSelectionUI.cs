using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [System.Serializable]
    public class CharacterOption
    {
        public string displayName;
        public FighterCharacter fighterCharacter = FighterCharacter.FerchoPoker;
        public CharacterData characterData;
        public GameObject characterPrefab;
        public Sprite icon;
    }

    [SerializeField] private CharacterOption[] characters = null;
    [SerializeField] private SceneLoader sceneLoader = null;
    [SerializeField] private Text characterNameText = null;
    [SerializeField] private Image characterIconImage = null;
    [SerializeField] private bool continueThroughLoadingScene = false;
    [SerializeField] private KeyCode previousKey = KeyCode.A;
    [SerializeField] private KeyCode nextKey = KeyCode.D;
    [SerializeField] private KeyCode confirmKey = KeyCode.X;

    private int selectedIndex;

    private void Awake()
    {
        FindSceneLoaderIfNeeded();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameFlowState.CharacterSelection);
        }

        UpdateSelectionView();
    }

    private void Update()
    {
        if (Input.GetKeyDown(previousKey))
        {
            SelectPrevious();
        }

        if (Input.GetKeyDown(nextKey))
        {
            SelectNext();
        }

        if (Input.GetKeyDown(confirmKey))
        {
            ConfirmSelection();
        }
    }

    public void SelectPrevious()
    {
        if (!HasCharacters())
        {
            return;
        }

        selectedIndex--;
        if (selectedIndex < 0)
        {
            selectedIndex = characters.Length - 1;
        }

        UpdateSelectionView();
    }

    public void SelectNext()
    {
        if (!HasCharacters())
        {
            return;
        }

        selectedIndex = (selectedIndex + 1) % characters.Length;
        UpdateSelectionView();
    }

    public void ConfirmSelection()
    {
        if (!HasCharacters())
        {
            Debug.LogWarning($"{nameof(CharacterSelectionUI)}: No characters are assigned.");
            return;
        }

        CharacterOption selectedCharacter = characters[selectedIndex];

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSelectedCharacter(
                selectedCharacter.characterData,
                selectedCharacter.characterPrefab
            );

            if (selectedCharacter.characterData == null)
            {
                GameManager.Instance.SetSelectedFighterCharacter(selectedCharacter.fighterCharacter);
            }
        }

        FindSceneLoaderIfNeeded();

        if (sceneLoader == null)
        {
            Debug.LogWarning($"{nameof(CharacterSelectionUI)}: SceneLoader reference is missing.");
            return;
        }

        sceneLoader.LoadLoadingOrFight(continueThroughLoadingScene);
    }

    private void UpdateSelectionView()
    {
        if (!HasCharacters())
        {
            if (characterNameText != null)
            {
                characterNameText.text = string.Empty;
            }

            if (characterIconImage != null)
            {
                characterIconImage.enabled = false;
            }

            return;
        }

        CharacterOption selectedCharacter = characters[selectedIndex];

        if (characterNameText != null)
        {
            characterNameText.text = GetCharacterDisplayName(selectedCharacter);
        }

        if (characterIconImage != null)
        {
            characterIconImage.sprite = selectedCharacter.icon;
            characterIconImage.enabled = selectedCharacter.icon != null;
        }
    }

    private bool HasCharacters()
    {
        return characters != null && characters.Length > 0;
    }

    private string GetCharacterDisplayName(CharacterOption character)
    {
        if (!string.IsNullOrWhiteSpace(character.displayName))
        {
            return character.displayName;
        }

        if (character.characterPrefab != null)
        {
            return character.characterPrefab.name;
        }

        if (character.characterData != null)
        {
            return character.characterData.name;
        }

        return "Character";
    }

    private void FindSceneLoaderIfNeeded()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindFirstObjectByType<SceneLoader>();
        }
    }
}
