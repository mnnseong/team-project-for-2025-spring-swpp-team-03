using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayTutorial : MonoBehaviour
{
    public Image tutorialImage;
    public TextMeshProUGUI tutorialText;

    public Sprite[] tutorialSprites;        // Add 5 images in Inspector
    [TextArea]
    public string[] tutorialDescriptions;   // Add 5 matching texts in Inspector

    public float slideDuration = 2f;

    private int currentIndex = 0;

    void Start()
    {
        if (tutorialSprites.Length != tutorialDescriptions.Length)
        {
            Debug.LogError("Sprites and Descriptions must match in length!");
            return;
        }

        StartCoroutine(PlayTutorial());
    }

    IEnumerator PlayTutorial()
    {
        while (currentIndex < tutorialSprites.Length)
        {
            tutorialImage.sprite = tutorialSprites[currentIndex];
            tutorialText.text = tutorialDescriptions[currentIndex];

            yield return new WaitForSeconds(slideDuration);
            currentIndex++;
        }

        // Optional: go to next scene or enable a button
        // SceneManager.LoadScene("NextScene");
    }
}
