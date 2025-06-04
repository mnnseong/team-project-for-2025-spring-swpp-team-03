using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleHoverGrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleFactor = 1.5f; 
    public float scaleSpeed = 10f;   

    private bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Smoothly scale towards target size
        Vector3 targetScale = isHovered ? originalScale * scaleFactor : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
