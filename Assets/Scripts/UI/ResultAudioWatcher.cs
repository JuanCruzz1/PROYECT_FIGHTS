using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultAudioWatcher : MonoBehaviour
{
    [Header("Result Objects")]
    [SerializeField] private GameObject youWinObject;
    [SerializeField] private GameObject youLoseObject;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip youWinAudio;
    [SerializeField] private AudioClip youLoseAudio;

    private bool readyToWatch = false;
    private bool hasPlayedAudio = false;

    private void Update()
    {
        if (!readyToWatch)
        {
            bool winOff = youWinObject == null || !youWinObject.activeInHierarchy;
            bool loseOff = youLoseObject == null || !youLoseObject.activeInHierarchy;

            if (winOff && loseOff)
            {
                readyToWatch = true;
            }

            return;
        }

        if (hasPlayedAudio) return;

        if (youWinObject != null && youWinObject.activeInHierarchy)
        {
            PlayAudio(youWinAudio);
        }
        else if (youLoseObject != null && youLoseObject.activeInHierarchy)
        {
            PlayAudio(youLoseAudio);
        }
    }

    private void PlayAudio(AudioClip clip)
    {
        hasPlayedAudio = true;

        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void RestartFight()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}