using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite spriteNormal;  // Interfaz1P
    public Sprite spriteHover;   // Interfaz1G

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null && spriteHover != null)
            image.sprite = spriteHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null && spriteNormal != null)
            image.sprite = spriteNormal;
    }
}