using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    public AudioMixer audioMixer;
    public string sfxVolumeParameter = "SFXVolume";

    [Header("사운드 클립")]
    public AudioClip cheongryongSkillSFX;
    public AudioClip jujakSkillSFX;
    public AudioClip baekhoSkillSFX;
    public AudioClip hyunmuSkillSFX;
    public AudioClip marcoHitSFX;
    public AudioClip obstacleBreakSFX;
    public AudioClip alienBreakSFX;
    public AudioClip baseArrivalSFX;
    public AudioClip energyRecoverSFX;
    public AudioClip gameOverSFX;
    public AudioClip buttonClickSFX;

    [Header("이펙트 프리팹")]
    public GameObject cheongryongSkillEffect;
    public GameObject jujakSkillEffect;
    public GameObject baekhoSkillEffect;
    public GameObject hyunmuSkillEffect;
    public GameObject marcoHitEffect;
    public GameObject obstacleBreakEffect;
    public GameObject alienBreakEffect;
    public GameObject baseArrivalEffect;
    public GameObject marcoDeathEffect;

    private AudioMixerGroup sfxGroup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        sfxGroup = audioMixer.FindMatchingGroups("SFX")[0];
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            GameObject temp = new GameObject("TempSFX");
            AudioSource source = temp.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxGroup;
            source.clip = clip;
            source.Play();
            Destroy(temp, clip.length);
        }
    }

    public void PlayEffect(GameObject prefab, Vector3 position)
    {
        if (prefab != null)
        {
            GameObject effect = Instantiate(prefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    public void PlayCheongryongSkill(Vector3 pos)
    {
        PlaySound(cheongryongSkillSFX, pos);
        PlayEffect(cheongryongSkillEffect, pos);
    }

    public void PlayJujakSkill(Vector3 pos)
    {
        PlaySound(jujakSkillSFX, pos);
        PlayEffect(jujakSkillEffect, pos);
    }

    public void PlayBaekhoSkill(Vector3 pos)
    {
        PlaySound(baekhoSkillSFX, pos);
        PlayEffect(baekhoSkillEffect, pos);
    }

    public void PlayHyunmuSkill(Vector3 pos)
    {
        PlaySound(hyunmuSkillSFX, pos);
        PlayEffect(hyunmuSkillEffect, pos);
    }

    public void PlayMarcoHit(Vector3 pos)
    {
        PlaySound(marcoHitSFX, pos);
        PlayEffect(marcoHitEffect, pos);
    }

    public void PlayGameOver(Vector3 pos)
    {
        PlaySound(gameOverSFX, pos);
        PlayEffect(marcoDeathEffect, pos);
    }

    public void PlayObstacleBreak(Vector3 pos)
    {
        PlaySound(obstacleBreakSFX, pos);
        PlayEffect(obstacleBreakEffect, pos);
    }

    public void PlayAlienBreak(Vector3 pos)
    {
        PlaySound(alienBreakSFX, pos);
        PlayEffect(alienBreakEffect, pos);
    }

    public void PlayBaseArrival(Vector3 pos)
    {
        PlaySound(baseArrivalSFX, pos);
        PlayEffect(baseArrivalEffect, pos);
    }

    public void PlayEnergyRecover(Vector3 pos)
    {
        PlaySound(energyRecoverSFX, pos);
    }

    public void PlayButtonClick(Vector3 pos)
    {
        PlaySound(buttonClickSFX, pos);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dB = sliderValue <= 0.0001f ? -80f : Mathf.Log10(sliderValue) * 20f;
        audioMixer.SetFloat(sfxVolumeParameter, dB);
    }

    public void ToggleSFX(bool on)
    {
        SetSFXVolume(on ? 1f : 0f);
    }

}
