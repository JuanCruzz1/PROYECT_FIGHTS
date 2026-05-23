using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CharacterButtonVisual : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;

    [Header("Selection Effect")]
    [SerializeField] private bool useScaleEffect = true;
    [SerializeField] private float selectedScale = 1.05f;

    private Image image;
    private Vector3 originalScale;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalScale = transform.localScale;
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (image != null)
        {
            image.sprite = isSelected && selectedSprite != null
                ? selectedSprite
                : normalSprite;
        }

        transform.localScale = isSelected && useScaleEffect
            ? originalScale * selectedScale
            : originalScale;
    }
}