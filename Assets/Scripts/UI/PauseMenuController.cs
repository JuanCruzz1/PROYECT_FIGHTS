using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text musicButtonText;
    [SerializeField] private TMP_Text sfxButtonText;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource[] sfxSources;

    [Header("Scenes")]
    [SerializeField] private string CharacterSelectionScene = "CharacterSelectionScene";

    private bool isPaused;
    private bool musicOn = true;
    private bool sfxOn = true;

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
        UpdateAudioTexts();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeFight();
            }
            else
            {
                PauseFight();
            }
        }
    }

    public void PauseFight()
    {
        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ResumeFight()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void EndFight()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(CharacterSelectionScene);
    }

    public void QuitGame()
    {
        Debug.Log("Cerrando juego...");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ToggleMusic()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMute();
            musicOn = !MusicManager.Instance.IsMuted();
        }

        UpdateAudioTexts();
    }

    public void ToggleSFX()
    {
        sfxOn = !sfxOn;

        if (sfxSources != null)
        {
            foreach (AudioSource source in sfxSources)
            {
                if (source != null)
                {
                    source.mute = !sfxOn;
                }
            }
        }

        UpdateAudioTexts();
    }

    private void UpdateAudioTexts()
    {
        if (musicButtonText != null)
        {
            musicButtonText.text = musicOn ? "MÚSICA: ON" : "MÚSICA: OFF";
        }

        if (sfxButtonText != null)
        {
            sfxButtonText.text = sfxOn ? "SFX: ON" : "SFX: OFF";
        }
    }
}