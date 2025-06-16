using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ClickSound : MonoBehaviour
{
    public AudioClip clickSound;

    // Start is called before the first frame update
    void Start()
    {
		GetComponent<Button>().onClick.AddListener(PlayClickSound);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayClickSound()
    {
	    GameObject temp = new GameObject("TempSFX");
	    AudioSource source = temp.AddComponent<AudioSource>();
		source.clip = clickSound;
	    source.Play();
		DontDestroyOnLoad(temp);
	    Destroy(temp, clickSound.length);
    }
}
