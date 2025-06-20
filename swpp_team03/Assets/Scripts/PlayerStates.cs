using UnityEngine;

// 일반 상태
public class NormalState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        Debug.Log("🟢 Normal State 진입");
        // 일반 상태 시작 시 설정
        if (player != null)
        {
            // 특별한 색상이나 이펙트가 있다면 초기화
        }
    }

    public void Update(PlayerController player)
    {
        // 일반 상태에서의 로직 (기존 PlayerController 로직)
        // 모든 기본 조작 가능
    }

    public void Exit(PlayerController player)
    {
        Debug.Log("🟢 Normal State 종료");
    }

    public string GetStateName()
    {
        return "Normal";
    }
}

// 대시 상태
public class DashingState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        Debug.Log("🔥 Dashing State 진입");
        // 대시 시작 시 설정
        if (player != null)
        {
            // 대시 이펙트나 무적 상태 설정 가능
        }
    }

    public void Update(PlayerController player)
    {
        // 대시 중에는 일반 이동 제한
        // 대시 중 특별한 물리 효과나 제약 사항 적용 가능
    }

    public void Exit(PlayerController player)
    {
        Debug.Log("🔥 Dashing State 종료");
        // 대시 종료 시 정리 작업
    }

    public string GetStateName()
    {
        return "Dashing";
    }
}

// 무적 상태 (현무 스킬 등)
public class InvincibleState : IPlayerState
{
    private float invincibilityDuration = 0f;
    
    public void Enter(PlayerController player)
    {
        Debug.Log("✨ Invincible State 진입");
        invincibilityDuration = 0f;
        
        // 무적 상태 시각적 효과
        if (player != null)
        {
            // 무적 이펙트나 색상 변경 등
            var renderer = player.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 투명도 조절이나 색상 변경으로 무적 상태 표시
            }
        }
    }

    public void Update(PlayerController player)
    {
        invincibilityDuration += Time.deltaTime;
        
        // 무적 상태 로직
        // 일정 시간 후 자동으로 Normal 상태로 복귀하는 로직도 가능
    }

    public void Exit(PlayerController player)
    {
        Debug.Log("✨ Invincible State 종료");
        
        // 무적 상태 시각적 효과 제거
        if (player != null)
        {
            var renderer = player.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 원래 색상으로 복귀
            }
        }
    }

    public string GetStateName()
    {
        return "Invincible";
    }
    
    public float GetInvincibilityDuration()
    {
        return invincibilityDuration;
    }
}

// 면역 상태 (피격 후 일시적 무적)
public class ImmuneState : IPlayerState
{
    private float immuneDuration = 0f;
    private float blinkTimer = 0f;
    private const float blinkInterval = 0.1f;
    private bool isVisible = true;
    
    public void Enter(PlayerController player)
    {
        Debug.Log("🛡️ Immune State 진입");
        immuneDuration = 0f;
        blinkTimer = 0f;
        isVisible = true;
        
        // 면역 상태 시각적 효과 시작
        if (player != null)
        {
            StartBlinkEffect(player);
        }
    }

    public void Update(PlayerController player)
    {
        immuneDuration += Time.deltaTime;
        blinkTimer += Time.deltaTime;
        
        // 깜빡임 효과
        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            ToggleVisibility(player);
        }
        
        // 면역 상태에서는 데미지를 받지 않음
        // 일반적인 이동은 가능
    }

    public void Exit(PlayerController player)
    {
        Debug.Log("🛡️ Immune State 종료");
        
        // 깜빡임 효과 종료 및 완전히 보이게 설정
        if (player != null)
        {
            SetVisibility(player, true);
        }
    }

    public string GetStateName()
    {
        return "Immune";
    }
    
    public float GetImmuneDuration()
    {
        return immuneDuration;
    }
    
    private void StartBlinkEffect(PlayerController player)
    {
        // 깜빡임 효과 초기화
        isVisible = true;
    }
    
    private void ToggleVisibility(PlayerController player)
    {
        isVisible = !isVisible;
        SetVisibility(player, isVisible);
    }
    
    private void SetVisibility(PlayerController player, bool visible)
    {
        if (player == null) return;
        
        var renderer = player.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color color = renderer.material.color;
            color.a = visible ? 1f : 0.3f; // 완전 투명하지 않고 살짝 보이게
            renderer.material.color = color;
        }
    }
} 