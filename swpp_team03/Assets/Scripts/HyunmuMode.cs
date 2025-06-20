using System.Collections;
using UnityEngine;
using System.Reflection;

public class HyunmuMode : MonoBehaviour
{
    public float energyCost = 30f;
    public float cooldown = 20f;
    public float duration = 4f;
    private float nextAvailableTime = 0f;

    private StatusBar statusBar;
    private PlayerController playerController;
    public bool isInvincible = false;

    void Start()
    {
        statusBar = FindObjectOfType<StatusBar>();
        playerController = GetComponent<PlayerController>();
    }

    public void ManualTrigger()
    {
        // 쿨타임 및 에너지 체크
        if (Time.time < nextAvailableTime || statusBar == null || GetEnergy() < energyCost) return;

        // 에너지 소비 및 쿨타임 적용
        ConsumeEnergy(energyCost);
        nextAvailableTime = Time.time + cooldown;

        // 내구도 회복
        statusBar.AddHealth(20f);
        // 무적 상태 시작
        StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        EffectManager.Instance.PlayHyunmuSkillFollow(gameObject, duration);
        //EffectManager.Instance.PlayHyunmuSkill(transform.position);
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    float GetEnergy()
    {
        FieldInfo fi = typeof(StatusBar).GetField("currentEnergy", BindingFlags.Instance | BindingFlags.NonPublic);
        return fi.GetValue(statusBar) is float value ? value : 0f;
    }

    void ConsumeEnergy(float amount)
    {
        FieldInfo fi = typeof(StatusBar).GetField("currentEnergy", BindingFlags.Instance | BindingFlags.NonPublic);
        float current = GetEnergy();
        fi.SetValue(statusBar, Mathf.Max(0, current - amount));
        statusBar.SendMessage("UpdateUI");
    }

    // 외부에서 무적 상태를 확인할 수 있도록
    public bool IsInvincible() => isInvincible;
}
