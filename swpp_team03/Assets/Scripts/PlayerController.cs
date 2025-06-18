using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveForce = 20f;
    public float maxSpeed = 10f;
    public float rotationSpeed = 60f;
    public float uprightStability = 2f;
    public float groundCheckDistance = 1.0f;
    public float centerOfGravityY = -1.5f;
    public float gravityStrength = 1.0f;
    public GameObject gameClear;
    private Rigidbody rb;
    private bool isGrounded;
    public bool isImmune = false;
    public int enemyDamage = 10;
    public float immuneTime = 3.0f;
    private GameObject gameManager;
    private StatusBar statusBarScript;
    private RouteManageInPlaying routeManageInPlayingScript;
    private DashForward dashForwardScript;
    private HyunmuMode hyunmuModeScript;
    private bool lightTrigger = true;

    // State Pattern 추가
    private IPlayerState currentState;
    private NormalState normalState;
    private DashingState dashingState;
    private InvincibleState invincibleState;
    private ImmuneState immuneState;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.centerOfMass = new Vector3(0, centerOfGravityY, 0); // 무게 중심을 아래로
        Vector3 com = rb.centerOfMass;
        com.y = centerOfGravityY;
        rb.centerOfMass = com;

        Physics.gravity = new Vector3(0, -9.81f * gravityStrength, 0);
        gameManager = GameObject.Find("GameManager");
        statusBarScript = gameManager.GetComponent<StatusBar>();
        routeManageInPlayingScript = gameManager.GetComponent<RouteManageInPlaying>();
        dashForwardScript = GetComponent<DashForward>();
        hyunmuModeScript = GetComponent<HyunmuMode>();
        // State Pattern 초기화
        InitializeStates();
    }

    void InitializeStates()
    {
        normalState = new NormalState();
        dashingState = new DashingState();
        invincibleState = new InvincibleState();
        immuneState = new ImmuneState();

        // 초기 상태는 Normal
        ChangeState(normalState);
    }

    public void ChangeState(IPlayerState newState)
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }

        currentState = newState;
        currentState.Enter(this);
    }

    public string GetCurrentStateName()
    {
        return currentState?.GetStateName() ?? "None";

    }

    // Update is called once per frame
    void Update()
    {
        // State Pattern 업데이트
        currentState?.Update(this);

        // 기존 Update 로직 유지
        float turn = Input.GetAxis("Horizontal");
        if (turn != 0)
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * turn * rotationSpeed * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f);

        float move = Input.GetAxis("Vertical");

        if (isGrounded && move != 0)
        {
            Vector3 force = transform.forward * move * moveForce;
            if (rb.velocity.magnitude < maxSpeed)
                rb.AddForce(force);
        }

        if (!isGrounded)
		{
			UprightCorrection();
		}
    }

    void UprightCorrection()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * rb.rotation;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, uprightStability * Time.fixedDeltaTime));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            bool immune = isImmune || dashForwardScript.isDashing || hyunmuModeScript.isInvincible;
            if (!immune)
            {
				EffectManager.Instance.PlayMarcoHit(transform.position);
                statusBarScript.TakeDamage(enemyDamage);
                StartCoroutine(ImmuneCoroutine());
            }
        }
    }

    private IEnumerator ImmuneCoroutine()
    {
        isImmune = true;
        // State를 Immune으로 변경
        IPlayerState previousState = currentState;
        ChangeState(immuneState);

        yield return new WaitForSeconds(immuneTime);

        isImmune = false;
        // 이전 상태로 복귀 (일반적으로 Normal)
        ChangeState(normalState);
    }

    void OnTriggerEnter(Collider other)
    {
        // TODO : Item logic
        if (other.gameObject.CompareTag("Light") && lightTrigger)
        {
			EffectManager.Instance.PlayBaseArrival(transform.position);
            gameClear.SetActive(true);
            StartCoroutine(LightTriggerCoroutine());
            Time.timeScale = 0f;
        }
    }

    private IEnumerator LightTriggerCoroutine()
    {
        lightTrigger = false;
        yield return new WaitForSeconds(1f);
        lightTrigger = true;
    }

    // 외부에서 상태 변경을 위한 공개 메서드들
    public void SetDashingState()
    {
        ChangeState(dashingState);
    }

    public void SetInvincibleState()
    {
        ChangeState(invincibleState);
    }

    public void SetNormalState()
    {
        ChangeState(normalState);
    }
}
