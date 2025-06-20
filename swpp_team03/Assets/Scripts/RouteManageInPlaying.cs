using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RouteManageInPlaying : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject routeManager;
    private int routeInt;
    public Transform[] lightTransforms;
    public GameObject lightObject;
    public TextMeshProUGUI nextBaseText;
	private bool isGameCleared = false;
    public GameObject gameClear;

    void Start()
    {
        if (GameObject.Find("RouteManager_1"))
        {
            routeManager = GameObject.Find("RouteManager_1");
            routeInt = routeManager.GetComponent<RouteManager>().route;

            Debug.Log(routeInt);
        }
        else
        {
            routeInt = 32;
        }
        lightObject.transform.position = lightTransforms[routeInt % 10].position;

		MiniMapNext marker = FindObjectOfType<MiniMapNext>();
		marker.target = lightObject.transform;

        nextBaseText.text = $"Next Base : {routeInt%10}";
        routeInt = routeInt / 10;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Next()
    {
        if (routeInt == 0)
        {

            Debug.Log("Game Clear!");
            gameClear.SetActive(true);
			isGameCleared = true;
            Time.timeScale = 0f;
        }
        else
        {
            int nextindex = routeInt % 10;
            routeInt = routeInt / 10;
            nextBaseText.text = $"Next Base : {nextindex}";
            lightObject.transform.position = lightTransforms[nextindex].position;
			MiniMapNext marker = FindObjectOfType<MiniMapNext>();
			if (marker != null)
			{
				marker.target = lightObject.transform;
			}
        }
    }

	public bool IsGameCleared()
	{
		return isGameCleared;
	}
}
