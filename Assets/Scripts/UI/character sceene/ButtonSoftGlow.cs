using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSoftGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image img;
    private Vector3 originalScale;

    void Awake()
    {
        img = GetComponent<Image>();
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse encima del botón");

        if (img != null)
            img.color = new Color(1.6f, 1.6f, 1.6f, 1f);

        transform.localScale = originalScale * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (img != null)
            img.color = Color.white;

        transform.localScale = originalScale;
    }
}