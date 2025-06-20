using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillUIController : MonoBehaviour
{
    [Header("UI 버튼들")]
    public Button button_h;
    public Button button_j;
    public Button button_k;
    public Button button_l;
    
    [Header("쿨다운 컴포넌트들")]
    public SkillCooldown cooldown_h;
    public SkillCooldown cooldown_j;
    public SkillCooldown cooldown_k;
    public SkillCooldown cooldown_l;
    
    // Factory Pattern으로 생성된 스킬 명령들
    private Dictionary<SkillType, ISkillCommand> skillCommands;
    private Dictionary<SkillType, SkillCooldown> skillCooldowns;

    void Start()
    {
        InitializeSkillSystem();
        SetupButtonListeners();
    }
    
    void InitializeSkillSystem()
    {
        // Factory Pattern을 사용한 스킬 명령 초기화
        skillCommands = new Dictionary<SkillType, ISkillCommand>();
        foreach (SkillType skillType in System.Enum.GetValues(typeof(SkillType)))
        {
            var command = SkillFactory.CreateSkillCommand(skillType);
            if (command != null)
            {
                skillCommands[skillType] = command;
            }
        }
        
        // 쿨다운 컴포넌트 매핑
        skillCooldowns = new Dictionary<SkillType, SkillCooldown>
        {
            { SkillType.Dragon, cooldown_h },
            { SkillType.Tiger, cooldown_j },
            { SkillType.Phoenix, cooldown_k },
            { SkillType.Turtle, cooldown_l }
        };
    }
    
    void SetupButtonListeners()
    {
        button_h?.onClick.AddListener(() => UseSkill("H"));
        button_j?.onClick.AddListener(() => UseSkill("J"));
        button_k?.onClick.AddListener(() => UseSkill("K"));
        button_l?.onClick.AddListener(() => UseSkill("L"));
    }

    void Update()
    {
        HandleKeyboardInput();
    }
    
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.H)) UseSkill("H");
        if (Input.GetKeyDown(KeyCode.J)) UseSkill("J");
        if (Input.GetKeyDown(KeyCode.K)) UseSkill("K");
        if (Input.GetKeyDown(KeyCode.L)) UseSkill("L");
    }

    void UseSkill(string key)
    {
        Debug.Log($"💥 Skill {key} activated!");

        SkillType skillType = SkillFactory.GetSkillTypeFromKey(key);
        
        // 쿨다운 체크
        if (!CanUseSkill(skillType))
        {
            Debug.Log($"{key} : 쿨다운중");
            return;
        }
        
        // 스킬 실행
        if (ExecuteSkill(skillType))
        {
            // 쿨다운 시작
            StartCooldown(skillType);
            
            // Observer Pattern - 스킬 사용 이벤트 발생
            TriggerSkillUsedEvent(skillType, key);
        }
    }
    
    bool CanUseSkill(SkillType skillType)
    {
        return skillCooldowns.ContainsKey(skillType) && 
               skillCooldowns[skillType] != null && 
               !skillCooldowns[skillType].IsOnCooldown();
    }
    
    bool ExecuteSkill(SkillType skillType)
    {
        if (!skillCommands.ContainsKey(skillType))
        {
            Debug.LogWarning($"💥 Invalid skill type: {skillType}!");
            return false;
        }
        
        ISkillCommand command = skillCommands[skillType];
        if (command == null)
        {
            Debug.LogWarning($"🚫 Skill command is null for {skillType}!");
            return false;
        }
        
        if (command.CanExecute())
        {
            command.Execute();
            return true;
        }
        else
        {
            Debug.LogWarning($"🚫 {command.GetSkillName()}을(를) 사용할 수 없습니다!");
            return false;
        }
    }
    
    void StartCooldown(SkillType skillType)
    {
        if (!skillCooldowns.ContainsKey(skillType) || skillCooldowns[skillType] == null)
            return;
            
        SkillInfo skillInfo = SkillFactory.GetSkillInfo(skillType);
        if (skillInfo != null)
        {
            skillCooldowns[skillType].cooldownDuration = skillInfo.cooldownDuration;
            skillCooldowns[skillType].TriggerCooldown();
        }
    }
    
    void TriggerSkillUsedEvent(SkillType skillType, string key)
    {
        if (skillCommands.ContainsKey(skillType))
        {
            GameEventSystem.Instance.TriggerEvent(GameEventType.SkillUsed, new { 
                skillName = skillCommands[skillType].GetSkillName(), 
                skillKey = key,
                skillType = skillType,
                timestamp = Time.time 
            });
        }
    }

    // 기존 메서드들은 하위 호환성을 위해 유지 (deprecated)
    [System.Obsolete("Use UseSkill method instead")]
    void skill_H()
    {
        // 청룡
        if (cooldown_h.IsOnCooldown())
        {
            Debug.Log("쿨다운중");
            return;
        }

        Debug.Log("근처 장애물 파괴!");
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            AreaDestroy areaDestroy = player.GetComponent<AreaDestroy>();
            if (areaDestroy != null)
            {
                areaDestroy.ManualTrigger();
                cooldown_h.cooldownDuration = 10;
                cooldown_h.TriggerCooldown();
            }
            else
            {
                Debug.LogWarning("🚫 AreaDestroy 스크립트가 없음!");
            }
        }
    }

    [System.Obsolete("Use UseSkill method instead")]
    void skill_J()
    {
        // 백호
        if (cooldown_j.IsOnCooldown())
        {
            Debug.Log("쿨다운중");
            return;
        }

        Debug.Log("🐯 백호 스킬 발동: 돌진!");

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            DashForward dash = player.GetComponent<DashForward>();
            if (dash != null)
            {
                dash.StartDash();
                cooldown_j.cooldownDuration = 12;
                cooldown_j.TriggerCooldown();
            }
            else
            {
                Debug.LogWarning("🚫 DashForward 컴포넌트가 없음!");
            }
        }
    }

    [System.Obsolete("Use UseSkill method instead")]
    void skill_K()
    {
        // 주작
        if (cooldown_k.IsOnCooldown())
        {
            Debug.Log("쿨다운중");
            return;
        }

        Debug.Log("점프!");
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            HighJump jumpSkill = player.GetComponent<HighJump>();
            if (jumpSkill != null)
            {
                jumpSkill.ManualTrigger();
                cooldown_k.cooldownDuration = 8;
                cooldown_k.TriggerCooldown();
            }
            else
            {
                Debug.LogWarning("🚫 HighJump 스크립트가 없음!");
            }
        }
    }

    [System.Obsolete("Use UseSkill method instead")]
    void skill_L()
    {
        // 현무
        if (cooldown_l.IsOnCooldown())
        {
            Debug.Log("쿨다운중");
            return;
        }
        Debug.Log("현무모드");

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            HyunmuMode hyunmu = player.GetComponent<HyunmuMode>();
            if (hyunmu != null)
            {
                hyunmu.ManualTrigger();
                cooldown_l.cooldownDuration = 20;
                cooldown_l.TriggerCooldown();
            }
            else
            {
                Debug.LogWarning("🚫 HyunmuMode 스크립트가 없음!");
            }
        }
    }
}
