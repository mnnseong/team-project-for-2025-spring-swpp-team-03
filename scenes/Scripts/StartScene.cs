using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("2-1.Introduction");
    }
}
