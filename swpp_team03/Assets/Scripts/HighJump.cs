using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighJump : MonoBehaviour
{
    public float jumpForce = 10f;
    public float fallMultiplier = 0.5f;
    public float energyCost = 20f;
    public float cooldown = 8f;

    private float nextAvailableTime = 0f;
    private Rigidbody rb;
    private StatusBar statusBar;
    private PlayerController playerController;

    private bool isJumping = false;
    private bool slowFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        statusBar = FindObjectOfType<StatusBar>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (slowFalling && rb.velocity.y < 0)
        {
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
        }

        if (isJumping && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            isJumping = false;
            slowFalling = false;
        }
    }

    float GetEnergy()
    {
        return typeof(StatusBar).GetField("currentEnergy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(statusBar) is float value ? value : 0f;
    }

    void ConsumeEnergy(float amount)
    {
        typeof(StatusBar).GetField("currentEnergy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(statusBar, Mathf.Max(0, GetEnergy() - amount));
        statusBar.SendMessage("UpdateUI");
    }

    public void ManualTrigger()
    {
        if (Time.time < nextAvailableTime || statusBar == null || GetEnergy() < energyCost) return;

        ConsumeEnergy(energyCost);
        nextAvailableTime = Time.time + cooldown;

        Vector3 velocity = playerController != null ? playerController.GetVelocity() : Vector3.zero;
		EffectManager.Instance.PlayJujakSkill(transform.position, velocity);

        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        isJumping = true;
        slowFalling = true;
    }

}

