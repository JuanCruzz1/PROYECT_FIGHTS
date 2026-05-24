using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    public AudioClip musicaInicio;
    public AudioClip musicaJuego;
    public AudioClip musicaPelea;

    private AudioSource audioSource;
    private bool isMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CambiarMusicaPorEscena(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CambiarMusicaPorEscena(scene.name);
    }

    private void CambiarMusicaPorEscena(string sceneName)
    {
        if (audioSource == null) return;

        AudioClip nuevaMusica;

        if (sceneName == "Start_Scene")
        {
            nuevaMusica = musicaInicio;
        }
        else if (sceneName == "FightScene")
        {
            nuevaMusica = musicaPelea;
        }
        else
        {
            nuevaMusica = musicaJuego;
        }

        if (audioSource.clip == nuevaMusica) return;

        audioSource.clip = nuevaMusica;
        audioSource.loop = true;
        audioSource.mute = isMuted;
        audioSource.Play();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }
    }

    public void SetMute(bool mute)
    {
        isMuted = mute;

        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }
    }

    public bool IsMuted()
    {
        return isMuted;
    }
}