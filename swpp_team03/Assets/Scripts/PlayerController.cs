using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveForce = 20f;
    public float maxSpeed = 10f;
    public float rotationSpeed = 60f;
    public float uprightStability = 2f;
    public float groundCheckDistance = 1.0f;

    [Header("물리 설정")]
    public float centerOfGravityY = -1.5f;
    public float gravityStrength = 1.0f;

    [Header("전투 설정")]
    public int enemyDamage = 10;
    public float immuneTime = 3.0f;


    // 컴포넌트 참조들
    private Rigidbody rb;
    private GameObject gameManager;
    private StatusBar statusBarScript;
    private RouteManageInPlaying routeManageInPlayingScript;
    private DashForward dashForwardScript;
    private HyunmuMode hyunmuModeScript;

    // 상태 변수들
    private bool isGrounded;
    public bool isImmune = false;
    private bool lightTrigger = true;

    // State Pattern
    private IPlayerState currentState;
    private NormalState normalState;
    private DashingState dashingState;
    private InvincibleState invincibleState;
    private ImmuneState immuneState;

    void Start()
    {
        InitializeComponents();
        InitializePhysics();
        InitializeStates();
        InitializeGameReferences();
    }

    void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody component is missing!");
            return;
        }

        dashForwardScript = GetComponent<DashForward>();
        hyunmuModeScript = GetComponent<HyunmuMode>();
    }

    void InitializePhysics()
    {
        if (rb == null) return;

        // 무게 중심 설정
        Vector3 com = rb.centerOfMass;
        com.y = centerOfGravityY;
        rb.centerOfMass = com;

        // 중력 설정
        Physics.gravity = new Vector3(0, -9.81f * gravityStrength, 0);
    }

    void InitializeGameReferences()
    {
        gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
        {
            statusBarScript = gameManager.GetComponent<StatusBar>();
            routeManageInPlayingScript = gameManager.GetComponent<RouteManageInPlaying>();

            if (statusBarScript == null)
                Debug.LogWarning("⚠️ StatusBar script not found on GameManager!");
            if (routeManageInPlayingScript == null)
                Debug.LogWarning("⚠️ RouteManageInPlaying script not found on GameManager!");
        }
        else
        {
            Debug.LogWarning("⚠️ GameManager not found!");
        }
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
        if (newState == null)
        {
            Debug.LogWarning("⚠️ Trying to change to null state!");
            return;
        }

        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public string GetCurrentStateName()
    {
        return currentState?.GetStateName() ?? "None";
    }

    void Update()
    {
        // State Pattern 업데이트
        currentState?.Update(this);

        // 입력 처리
        HandleRotationInput();
    }

    void HandleRotationInput()
    {
        float turn = Input.GetAxis("Horizontal");
        if (turn != 0 && rb != null)
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * turn * rotationSpeed * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        UpdateGroundCheck();
        HandleMovementInput();

        if (!isGrounded)
        {
            UprightCorrection();
        }
    }

    void UpdateGroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f);
    }

    void HandleMovementInput()
    {
        float move = Input.GetAxis("Vertical");

        if (isGrounded && move != 0)
        {
            Vector3 force = transform.forward * move * moveForce;
            if (rb.velocity.magnitude < maxSpeed)
                rb.AddForce(force);
        }
    }

    void UprightCorrection()
    {
        if (rb == null) return;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * rb.rotation;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, uprightStability * Time.fixedDeltaTime));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision();
        }
    }

    void HandleEnemyCollision()
    {
        bool immune = isImmune ||
                     (dashForwardScript != null && dashForwardScript.isDashing) ||
                     (hyunmuModeScript != null && hyunmuModeScript.isInvincible);

        if (!immune)
        {
            EffectManager.Instance?.PlayMarcoHit(transform.position);
            statusBarScript?.TakeDamage(enemyDamage);
            StartCoroutine(ImmuneCoroutine());
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
        // Normal 상태로 복귀
        ChangeState(normalState);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Light") && lightTrigger)
        {
            HandleLightTrigger();
        }
    }

    void HandleLightTrigger()
    {
        EffectManager.Instance?.PlayBaseArrival(transform.position);
        routeManageInPlayingScript.Next();
        StartCoroutine(LightTriggerCoroutine());
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

    // 디버그 정보 제공
    public bool IsGrounded() => isGrounded;
    public bool IsImmune() => isImmune;
    public Vector3 GetVelocity() => rb != null ? rb.velocity : Vector3.zero;
}
