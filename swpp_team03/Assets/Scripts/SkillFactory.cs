using System.Collections.Generic;
using UnityEngine;

// 스킬 타입 enum
public enum SkillType
{
    Dragon,    // 청룡 - H키
    Tiger,     // 백호 - J키
    Phoenix,   // 주작 - K키
    Turtle     // 현무 - L키
}

// 스킬 정보 데이터 클래스
[System.Serializable]
public class SkillInfo
{
    public string name;
    public float cooldownDuration;
    public string description;
    
    public SkillInfo(string skillName, float cooldown, string desc = "")
    {
        name = skillName;
        cooldownDuration = cooldown;
        description = desc;
    }
}

// Factory Pattern - 스킬 생성 팩토리
public static class SkillFactory
{
    private static readonly Dictionary<SkillType, SkillInfo> skillInfos = new Dictionary<SkillType, SkillInfo>
    {
        { SkillType.Dragon, new SkillInfo("청룡 스킬", 10f, "근처 장애물 파괴") },
        { SkillType.Tiger, new SkillInfo("백호 스킬", 12f, "돌진 공격") },
        { SkillType.Phoenix, new SkillInfo("주작 스킬", 8f, "높이 점프") },
        { SkillType.Turtle, new SkillInfo("현무 스킬", 15f, "무적 모드") }
    };
    
    public static ISkillCommand CreateSkillCommand(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Dragon:
                return new DragonSkillCommand();
            case SkillType.Tiger:
                return new TigerSkillCommand();
            case SkillType.Phoenix:
                return new PhoenixSkillCommand();
            case SkillType.Turtle:
                return new TurtleSkillCommand();
            default:
                Debug.LogWarning($"Unknown skill type: {skillType}");
                return null;
        }
    }
    
    public static SkillInfo GetSkillInfo(SkillType skillType)
    {
        return skillInfos.ContainsKey(skillType) ? skillInfos[skillType] : null;
    }
    
    public static SkillType GetSkillTypeFromKey(string key)
    {
        switch (key.ToLower())
        {
            case "h": return SkillType.Dragon;
            case "j": return SkillType.Tiger;
            case "k": return SkillType.Phoenix;
            case "l": return SkillType.Turtle;
            default:
                Debug.LogWarning($"Unknown skill key: {key}");
                return SkillType.Dragon; // 기본값
        }
    }
} 