using UnityEngine;
using UnityEngine.UIElements;
using Core.Events;
using EventDispatcher = Core.Events.EventDispatcher;
using EventType = Core.Events.EventType;

public class PlayerProgession
{
    public int level;
    public int skillCap;

    [Header("Debug Only")]
    public int skillPoints;
    public float currentXP;
    public float levelupXP = 50;

    [Space]
    public int vigor = 1;
    public int endurance = 1;
    public int strength = 1;
    public int dexterity = 1;
    
    public enum SkillType
    {
        Vigor,
        Endurance,
        Strength,
        Dexterity
    }

    public void AddXP(float amount) {
        if (currentXP + amount >= levelupXP) {
            currentXP = (currentXP + amount) - levelupXP;
            levelupXP = 50 * Mathf.Pow(1.2f, level);
            
            level++;
            skillPoints++;
        }

        else {
            currentXP += amount;
        }
        
        EventDispatcher.Instance.FireEvent(EventType.LevelChangeEvent, GetXpRatio() );
    }

    public void AddSkillType(SkillType type)
    {
        if (skillPoints <= 0) return;
        switch (type) {
            case SkillType.Vigor:
                if (vigor < skillCap) {
                    vigor++;
                    skillPoints--;
                    break;
                }

                break;

            case SkillType.Endurance:
                if (endurance < skillCap) {
                    endurance++;
                    skillPoints--;
                    break;
                }

                break;

            case SkillType.Strength:
                if (strength < skillCap) {
                    strength++;
                    skillPoints--;
                    break;
                }

                break;

            case SkillType.Dexterity:
                if (dexterity < skillCap) {
                    dexterity++;
                    skillPoints--;
                    break;
                }

                break;

            default:
                Debug.Log($"Invalid Skill Type in {this}");
                break;
        }
    }

    private float GetXpRatio()
    {
        return currentXP / levelupXP;
    }

}