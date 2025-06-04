using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class MapImageAnimator : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    public float animationDuration = 0.5f;
    public Vector3 startScale = new Vector3(0.8f, 0.8f, 1f);
    public Vector3 endScale = new Vector3(1f, 1f, 1f);

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        ResetState();
    }

    public void AnimateIn()
    {
        StopAllCoroutines();
        ResetState();
        StartCoroutine(FadeAndZoom());
    }

    private void ResetState()
    {
        canvasGroup.alpha = 0f;
        rectTransform.localScale = startScale;
    }

    private IEnumerator FadeAndZoom()
    {
        float t = 0f;
        while (t < animationDuration)
        {
            t += Time.deltaTime;
            float progress = t / animationDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.localScale = endScale;
    }
}
