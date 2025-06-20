using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDestroy : MonoBehaviour
{
    public float radius = 5f;
    public float energyCost = 20f;
    public float cooldown = 10f;
    private float nextAvailableTime = 0f;

    private StatusBar statusBar;
    private PlayerController playerController;

    void Start()
    {
        statusBar = FindObjectOfType<StatusBar>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
    }

    float GetEnergy()
    {
        return typeof(StatusBar).GetField("currentEnergy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(statusBar) is float value ? value : 0f;
    }

    void ConsumeEnergy(float amount)
    {
        typeof(StatusBar).GetField("currentEnergy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(statusBar, Mathf.Max(0, GetEnergy() - amount));
        statusBar.SendMessage("UpdateUI");  // UI 업데이트
    }

    public void ManualTrigger()
    {
        if (Time.time < nextAvailableTime || statusBar == null || GetEnergy() < energyCost) return;

        ConsumeEnergy(energyCost);
        nextAvailableTime = Time.time + cooldown;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        Vector3 velocity = playerController != null ? playerController.GetVelocity() : Vector3.zero;
		EffectManager.Instance.PlayCheongryongSkill(transform.position + transform.forward * 10f, velocity);

        foreach (Collider c in colliders)
        {
            if (c.CompareTag("Destructible") || c.CompareTag("Enemy"))
            {
		        Vector3 pos = c.transform.position;

				if (c.CompareTag("Destructible"))
					EffectManager.Instance.PlayObstacleBreak(pos);
				else if (c.CompareTag("Enemy"))
					EffectManager.Instance.PlayAlienBreak(pos);

                Debug.Log(c);
                Destroy(c.gameObject);
            }
        }
    }

}
