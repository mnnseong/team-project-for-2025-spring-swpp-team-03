using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Button pauseButton;
    public Button resumeButton;
	public StatusBar statusBar;
	public RouteManageInPlaying routemanage;

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuPanel.SetActive(false);

        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isPaused) {
                ResumeGame();
            }
            else {
                PauseGame();
            }
        }

    }

    public void PauseGame()
    {
		EffectManager.Instance.PlayButtonClick(new Vector3(0, 0, 0));
		if (statusBar != null && statusBar.IsGameOver()) return;
		if (routemanage != null && routemanage.IsGameCleared()) return;

        Time.timeScale = 0f;
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
		EffectManager.Instance.PlayButtonClick(new Vector3(0, 0, 0));
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

	public bool IsGamePaused()
	{
		return isPaused;
	}
}

