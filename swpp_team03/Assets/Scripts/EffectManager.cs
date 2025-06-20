using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("오디오 설정")]
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

    [Header("오브젝트 풀 설정")]
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxPoolSize = 50;

    private AudioMixerGroup sfxGroup;

    // Object Pool for Effects
    private Dictionary<GameObject, Queue<GameObject>> effectPools;
    private Dictionary<GameObject, GameObject> poolParents;

    private void Awake()
    {
        // Singleton 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSystem();
            InitializeObjectPools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSystem()
    {
        if (audioMixer != null)
        {
            var groups = audioMixer.FindMatchingGroups("SFX");
            if (groups != null && groups.Length > 0)
            {
                sfxGroup = groups[0];
            }
            else
            {
                Debug.LogWarning("⚠️ SFX AudioMixerGroup not found!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ AudioMixer is not assigned!");
        }
    }

    private void InitializeObjectPools()
    {
        effectPools = new Dictionary<GameObject, Queue<GameObject>>();
        poolParents = new Dictionary<GameObject, GameObject>();

        // 이펙트 프리팹들의 풀 생성
        GameObject[] effectPrefabs = {
            cheongryongSkillEffect, jujakSkillEffect, baekhoSkillEffect, hyunmuSkillEffect,
            marcoHitEffect, obstacleBreakEffect, alienBreakEffect, baseArrivalEffect, marcoDeathEffect
        };

        foreach (var prefab in effectPrefabs)
        {
            if (prefab != null)
            {
                CreatePool(prefab);
            }
        }
    }

    private void CreatePool(GameObject prefab)
    {
        // 풀 부모 오브젝트 생성
        GameObject poolParent = new GameObject($"Pool_{prefab.name}");
        poolParent.transform.SetParent(transform);
        poolParents[prefab] = poolParent;

        // 풀 초기화
        Queue<GameObject> pool = new Queue<GameObject>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab, poolParent.transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        effectPools[prefab] = pool;
    }

    private GameObject GetPooledObject(GameObject prefab)
    {
        if (prefab == null || !effectPools.ContainsKey(prefab))
            return null;

        Queue<GameObject> pool = effectPools[prefab];

        // 풀에서 비활성화된 오브젝트 찾기
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // 사용 가능한 오브젝트가 없으면 새로 생성 (최대 크기 제한)
        if (pool.Count < maxPoolSize)
        {
            GameObject newObj = Instantiate(prefab, poolParents[prefab].transform);
            pool.Enqueue(newObj);
            return newObj;
        }

        // 최대 크기에 도달했으면 가장 오래된 오브젝트 재사용
        GameObject oldestObj = pool.Dequeue();
        pool.Enqueue(oldestObj);
        return oldestObj;
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip == null)
        {
            Debug.LogWarning("⚠️ AudioClip is null!");
            return;
        }

        GameObject temp = new GameObject("TempSFX");
        temp.transform.position = position;

        AudioSource source = temp.AddComponent<AudioSource>();
        if (sfxGroup != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
        }

        source.clip = clip;
        source.Play();

        // 클립 재생 완료 후 자동 삭제
        StartCoroutine(DestroyAfterClip(temp, clip.length));
    }

    private IEnumerator DestroyAfterClip(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    public void PlayEffect(GameObject prefab, Vector3 position, float duration = 2f)
    {
        if (prefab == null)
        {
            Debug.LogWarning("⚠️ Effect prefab is null!");
            return;
        }

        GameObject effect = GetPooledObject(prefab);
        if (effect != null)
        {
            effect.transform.position = position;
            effect.transform.rotation = Quaternion.identity;
            effect.SetActive(true);

            // 지정된 시간 후 비활성화
            StartCoroutine(DeactivateAfterDelay(effect, duration));
        }
        else
        {
            // 풀링 실패 시 기존 방식으로 폴백
            GameObject effect2 = Instantiate(prefab, position, Quaternion.identity);
            Destroy(effect2, duration);
        }
    }

    private IEnumerator DeactivateAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            obj.SetActive(false);
        }
    }

    // 스킬별 이펙트 및 사운드 재생 메서드들
    public void PlayCheongryongSkill(Vector3 pos, Vector3 characterVelocity)
    {
        Vector3 predictedPos = pos + characterVelocity * 0.2f;
        PlaySound(cheongryongSkillSFX, pos);
        PlayEffect(cheongryongSkillEffect, predictedPos);
    }

    public void PlayJujakSkill(Vector3 pos, Vector3 characterVelocity)
    {
        Vector3 predictedPos = pos + characterVelocity * 0.2f;
        PlaySound(jujakSkillSFX, pos);
        PlayEffect(jujakSkillEffect, predictedPos);
    }

    public void PlayBaekhoSkill(Vector3 pos, Vector3 characterVelocity)
    {
        Vector3 predictedPos = pos + characterVelocity * 0.2f;
        PlaySound(baekhoSkillSFX, pos);
        PlayEffect(baekhoSkillEffect, predictedPos);
    }

    // public void PlayHyunmuSkill(Vector3 pos)
    // {
    //     PlaySound(hyunmuSkillSFX, pos);
    //     PlayEffect(hyunmuSkillEffect, pos);
    // }

    public void PlayHyunmuSkillFollow(GameObject target, float duration)
    {
        if (hyunmuSkillEffect == null || target == null)
        {
            Debug.LogWarning("⚠️ Hyunmu effect or target is null!");
            return;
        }
        PlaySound(hyunmuSkillSFX, target.transform.position);
        GameObject effect = GetPooledObject(hyunmuSkillEffect);
        if (effect != null)
        {
            effect.transform.SetParent(target.transform);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.identity;
            effect.SetActive(true);
            StartCoroutine(DeactivateAndUnparentAfterDelay(effect, duration));
        }
        else
        {
            GameObject effect2 = Instantiate(hyunmuSkillEffect, target.transform.position, Quaternion.identity, target.transform);
            Destroy(effect2, duration);
        }
    }

    private IEnumerator DeactivateAndUnparentAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            obj.SetActive(false);
            obj.transform.SetParent(poolParents[hyunmuSkillEffect].transform);
        }
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
        if (audioMixer == null)
        {
            Debug.LogWarning("⚠️ AudioMixer is not assigned!");
            return;
        }

        float dB = sliderValue <= 0.0001f ? -80f : Mathf.Log10(sliderValue) * 20f;
        audioMixer.SetFloat(sfxVolumeParameter, dB);
    }

    public void ToggleSFX(bool on)
    {
        SetSFXVolume(on ? 1f : 0f);
    }

    // 디버그 정보 제공
    public void PrintPoolStatus()
    {
        foreach (var kvp in effectPools)
        {
            Debug.Log($"Pool {kvp.Key.name}: {kvp.Value.Count} objects");
        }
    }
}
