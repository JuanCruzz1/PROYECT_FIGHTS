using UnityEngine;
using UnityEngine.SceneManagement;
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
        public GameObject infoPanel;
        public Sprite infoSprite;
    }

    [SerializeField] private CharacterOption[] characters = null;
    [SerializeField] private SceneLoader sceneLoader = null;
    [SerializeField] private Text characterNameText = null;
    [SerializeField] private Image characterIconImage = null;
    [SerializeField] private Image characterInfoImage = null;
    [SerializeField] private CharacterButtonVisual[] characterButtonVisuals = null;
    [SerializeField] private string loadingSceneName = "LoadingScene";
    [SerializeField] private KeyCode previousKey = KeyCode.A;
    [SerializeField] private KeyCode nextKey = KeyCode.D;
    [SerializeField] private KeyCode confirmKey = KeyCode.K;

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
        LogCurrentSelection("Previous");
    }

    public void SelectNext()
    {
        if (!HasCharacters())
        {
            return;
        }

        selectedIndex = (selectedIndex + 1) % characters.Length;
        UpdateSelectionView();
        LogCurrentSelection("Next");
    }

    public void ConfirmSelection()
    {
        if (!HasCharacters())
        {
            Debug.LogWarning($"{nameof(CharacterSelectionUI)}: No characters are assigned.");
            return;
        }

        CharacterOption selectedCharacter = characters[selectedIndex];
        Debug.Log($"{nameof(CharacterSelectionUI)}: Confirm pressed with {confirmKey}. Index={selectedIndex}, fighter={selectedCharacter.fighterCharacter}, display={GetCharacterDisplayName(selectedCharacter)}.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSelectedCharacter(
                selectedCharacter.characterData,
                selectedCharacter.characterPrefab
            );

            GameManager.Instance.SetSelectedFighterCharacter(selectedCharacter.fighterCharacter);
        }
        else
        {
            Debug.LogWarning($"{nameof(CharacterSelectionUI)}: GameManager.Instance is missing. Selection cannot persist.");
        }

        LoadLoadingScene();
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

            if (characterInfoImage != null)
            {
                characterInfoImage.enabled = false;
            }

            SetAllInfoPanelsInactive();
            UpdateButtonVisuals();

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

        if (characterInfoImage != null)
        {
            characterInfoImage.sprite = selectedCharacter.infoSprite;
            characterInfoImage.enabled = selectedCharacter.infoSprite != null;
        }

        UpdateInfoPanels();
        UpdateButtonVisuals();
    }

    private void LogCurrentSelection(string source)
    {
        if (!HasCharacters())
        {
            return;
        }

        CharacterOption selectedCharacter = characters[selectedIndex];
        Debug.Log($"{nameof(CharacterSelectionUI)}: {source} selection index={selectedIndex}, fighter={selectedCharacter.fighterCharacter}, display={GetCharacterDisplayName(selectedCharacter)}.");
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

    private void LoadLoadingScene()
    {
        if (string.IsNullOrWhiteSpace(loadingSceneName))
        {
            Debug.LogWarning($"{nameof(CharacterSelectionUI)}: loadingSceneName is not assigned.");
            return;
        }

        if (sceneLoader != null)
        {
            sceneLoader.LoadScene(loadingSceneName);
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameFlowState.Loading);
        }

        SceneManager.LoadScene(loadingSceneName);
    }

    private void UpdateInfoPanels()
    {
        if (!HasCharacters())
        {
            return;
        }

        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].infoPanel != null)
            {
                characters[i].infoPanel.SetActive(i == selectedIndex);
            }
        }
    }

    private void SetAllInfoPanelsInactive()
    {
        if (characters == null)
        {
            return;
        }

        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].infoPanel != null)
            {
                characters[i].infoPanel.SetActive(false);
            }
        }
    }

    private void UpdateButtonVisuals()
    {
        if (characterButtonVisuals == null)
        {
            return;
        }

        for (int i = 0; i < characterButtonVisuals.Length; i++)
        {
            if (characterButtonVisuals[i] != null)
            {
                characterButtonVisuals[i].SetSelected(HasCharacters() && i == selectedIndex);
            }
        }
    }
}
