using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StoryDisplay : MonoBehaviour
{
    public TextMeshProUGUI storyText;   
    public Button nextButton;           
    public string[] lines;
    public float delay = 2f;

    private void Start()
    {
        storyText.text = "";
        nextButton.gameObject.SetActive(false);  
        StartCoroutine(ShowStory());
    }

    IEnumerator ShowStory()
    {
        foreach (string line in lines)
        {
            storyText.text += line + "\n";
            yield return new WaitForSeconds(delay);
        }
        nextButton.gameObject.SetActive(true);  
    }

    public void GoToInstructions()
    {
        SceneManager.LoadScene("2-2.GamePlayExplanation");
    }
}
