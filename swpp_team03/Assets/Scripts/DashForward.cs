using UnityEngine;

public class DashForward : MonoBehaviour
{
    public float dashSpeed = 50f;         // 조절 가능
    public float dashDuration = 0.3f;
    public GameObject hitEffect;

    private float dashTimer;
    public bool isDashing = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        if (rb == null) return;

        isDashing = true;
        dashTimer = dashDuration;
		
		EffectManager.Instance.PlayBaekhoSkill(transform.position + transform.forward * 35f);

        // 👉 순간적으로 힘을 줘서 밀어버리기
        rb.velocity = transform.forward * dashSpeed;
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
