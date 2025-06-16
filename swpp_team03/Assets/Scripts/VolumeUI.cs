using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeUI : MonoBehaviour
{
	public BgmManager bgmManager;
	public EffectManager effectManager;

    public Slider bgmSlider;
    public Toggle bgmToggle;
    public Slider sfxSlider;
    public Toggle sfxToggle;

    // Start is called before the first frame update
    void Start()
    {
		if (bgmManager == null)
			bgmManager = FindObjectOfType<BgmManager>();

		bgmSlider.onValueChanged.AddListener((value) =>
		{
			bgmManager.SetBGMVolume(value);
			bgmToggle.isOn = value > 0f;
		});

		bgmToggle.onValueChanged.AddListener((isOn) =>
		{
			bgmManager.ToggleBGM(isOn);
			if (!isOn) bgmSlider.value = 0f;
			else if (bgmSlider.value == 0f) bgmSlider.value = 0.5f;
		});

		sfxSlider.onValueChanged.AddListener((value) =>
		{
			effectManager.SetSFXVolume(value);
			sfxToggle.isOn = value > 0f;
		});

		sfxToggle.onValueChanged.AddListener((isOn) =>
		{
			effectManager.ToggleSFX(isOn);
			if (!isOn)
				sfxSlider.value = 0f;
			else if (sfxSlider.value == 0f)
				sfxSlider.value = 0.5f;
		});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
