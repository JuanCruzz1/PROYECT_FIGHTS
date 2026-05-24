using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    [Header("Buttons")]
    [SerializeField] private Button[] buttons;

    private void Start()
    {
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                button.onClick.AddListener(PlayClickSound);
            }
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}