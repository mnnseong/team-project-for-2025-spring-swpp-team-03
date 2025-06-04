using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StartSceneUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Button startButton;
    public float animationDuration = 1f;
    public float buttonDelay = 1f;

    private CanvasGroup titleCanvas;
    private RectTransform titleTransform;

    void Start()
    {
        // Setup references
        titleCanvas = titleText.GetComponent<CanvasGroup>();
        titleTransform = titleText.GetComponent<RectTransform>();

        // Reset states
        titleCanvas.alpha = 0f;
        titleTransform.localScale = new Vector3(0.8f, 0.8f, 1f);
        startButton.gameObject.SetActive(false);

        // Begin animation
        StartCoroutine(AnimateTitleThenShowButton());
    }

    IEnumerator AnimateTitleThenShowButton()
    {
        float t = 0f;
        while (t < animationDuration)
        {
            t += Time.deltaTime;
            float progress = t / animationDuration;
            titleCanvas.alpha = Mathf.Lerp(0f, 1f, progress);
            titleTransform.localScale = Vector3.Lerp(new Vector3(0.8f, 0.8f, 1f), Vector3.one, progress);
            yield return null;
        }

        titleCanvas.alpha = 1f;
        titleTransform.localScale = Vector3.one;

        // Wait extra delay before showing start button
        yield return new WaitForSeconds(buttonDelay);
        startButton.gameObject.SetActive(true);
    }
}
