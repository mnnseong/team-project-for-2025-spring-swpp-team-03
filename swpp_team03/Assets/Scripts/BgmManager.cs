using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager : MonoBehaviour
{
    public AudioClip defaultBGM;
    public AudioClip titleBGM;
    public AudioClip routeBGM;
    public AudioClip gameplayBGM;

    private AudioSource audioSource;
    private static BgmManager instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;

            audioSource.clip = defaultBGM;
            audioSource.Play();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip targetBGM = null;

        switch (scene.name)
        {
            case "1.StartScene":
                targetBGM = titleBGM;
                break;
            case "3-2.ChooseRoute1":
                targetBGM = routeBGM;
                break;
            case "Scene1":
                targetBGM = gameplayBGM;
                break;
            default:
                targetBGM = defaultBGM;
                break;
        }

        if (targetBGM != null && audioSource.clip != targetBGM)
        {
            audioSource.Stop();
            audioSource.clip = targetBGM;
            audioSource.Play();
        }
    }

	public void SetBGMVolume(float volume)
	{
		audioSource.volume = volume;
	}

	public void ToggleBGM(bool isOn)
	{
		audioSource.mute = !isOn;
	}
}
