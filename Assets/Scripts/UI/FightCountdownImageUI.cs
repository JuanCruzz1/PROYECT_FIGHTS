using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FightCountdownImageUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private Image countdownImage;

    [Header("Sprites")]
    [SerializeField] private Sprite sprite3;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite spriteFight;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audio3;
    [SerializeField] private AudioClip audio2;
    [SerializeField] private AudioClip audio1;
    [SerializeField] private AudioClip audioFight;

    [Header("Timing")]
    [SerializeField] private float numberDuration = 1f;
    [SerializeField] private float fightDuration = 0.8f;

    public IEnumerator PlayCountdown()
    {
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        yield return ShowSpriteWithAudio(sprite3, audio3, numberDuration);
        yield return ShowSpriteWithAudio(sprite2, audio2, numberDuration);
        yield return ShowSpriteWithAudio(sprite1, audio1, numberDuration);
        yield return ShowSpriteWithAudio(spriteFight, audioFight, fightDuration);

        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }
    }

    private IEnumerator ShowSpriteWithAudio(Sprite sprite, AudioClip clip, float duration)
    {
        if (countdownImage != null)
        {
            countdownImage.sprite = sprite;
            countdownImage.enabled = sprite != null;
        }

        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }

        yield return new WaitForSeconds(duration);
    }
}