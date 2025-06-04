using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RouteSelectManager : MonoBehaviour
{
    public GameObject[] maps;                  
    public string[] routeDescriptions;         
    public TextMeshProUGUI routeDescription;   
    public Button startButton;                 

    private int selectedRouteIndex = -1;       

    public void OnHoverRoute(int index)
    {
        for (int i = 0; i < maps.Length; i++)
            maps[i].SetActive(i == index);

        routeDescription.text = routeDescriptions[index];

        selectedRouteIndex = index;

        startButton.gameObject.SetActive(true);

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => StartRoute(index));
    }

    void StartRoute(int index)
    {
        string sceneName = $"Track{index + 1}Scene";
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
