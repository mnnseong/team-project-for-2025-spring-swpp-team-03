using UnityEngine;

public class DashForward : MonoBehaviour
{
    public float dashSpeed = 50f;         // 조절 가능
    public float dashDuration = 0.3f;
    public GameObject hitEffect;

    private float dashTimer;
    public bool isDashing = false;
    private Rigidbody rb;
    public float effectposition = 35f;
    public float skillCost = 5f;
    private AreaDestroy areaDestroyScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        areaDestroyScript = GetComponent<AreaDestroy>();
        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody가 필요합니다!");
        }
    }

    void Update()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                Debug.Log("StopDash");
                StopDash();
            }
        }
    }

    public void StartDash()
    {
        if (areaDestroyScript.GetEnergy() < skillCost) return;
        if (rb == null) return;

        isDashing = true;
        dashTimer = dashDuration;

        areaDestroyScript.ConsumeEnergy(skillCost);
        Vector3 dashVelocity = transform.forward * dashSpeed;
		EffectManager.Instance.PlayBaekhoSkill(transform.position + transform.forward * effectposition, dashVelocity);

        // 👉 순간적으로 힘을 줘서 밀어버리기
        rb.velocity = dashVelocity;
        Debug.Log("StartDash");
    }

    void StopDash()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        isDashing = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isDashing) return;

        string tag = collision.gameObject.tag;

        if (tag == "Enemy" || tag == "Destructible")
        {
            Destroy(collision.gameObject);
        }

        if (hitEffect != null)
        {
            Instantiate(hitEffect, collision.contacts[0].point, Quaternion.identity);
        }

        StopDash();
    }
}
