using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonImage : MonoBehaviour
{
    [Header("Button Image")]
    [SerializeField] private Image targetImage;

    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite mutedSprite;

    private bool isMuted = false;

    private void Start()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage != null && normalSprite != null)
        {
            targetImage.sprite = normalSprite;
        }
    }

    public void ToggleImage()
    {
        isMuted = !isMuted;

        if (targetImage == null) return;

        targetImage.sprite = isMuted ? mutedSprite : normalSprite;
    }
}