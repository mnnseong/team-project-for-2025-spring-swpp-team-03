using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RouteManageInPlaying : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject routeManager;
    private int routeInt;
    private int leftCount;
    public Transform[] lightTransforms;
    public GameObject lightObject;
    public TextMeshProUGUI leftBaseText;
	private bool isGameCleared = false;
    public GameObject gameClear;
    public GameObject timeCountDown;
    private TimeCountdown timeCountdownScript;

    void Start()
    {
        timeCountdownScript = timeCountDown.GetComponent<TimeCountdown>();
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

        leftCount = routeInt.ToString().Length;
        leftBaseText.text = $"Left Base : {leftCount}";
        routeInt = routeInt / 10;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Next()
    {
        timeCountdownScript.AddTimeUsed();
        if (leftCount == 1)
        {
            timeCountdownScript.SetTimeUsed();
            Debug.Log("Game Clear!");
            gameClear.SetActive(true);
            isGameCleared = true;
            Time.timeScale = 0f;
        }
        else
        {
            int nextindex = routeInt % 10;
            routeInt = routeInt / 10;
            leftCount--;
            leftBaseText.text = $"Left Base : {leftCount}";
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
