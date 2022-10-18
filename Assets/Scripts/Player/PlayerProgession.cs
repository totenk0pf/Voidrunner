using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgession : MonoBehaviour
{
    public int level;
    public int skillPoints;
    public float currentXP;
    public float levelupXP = 50;

    [Space]
    public int vigor = 1;
    public int endurance = 1;
    public int strength = 1;
    public int dexterity = 1;

    //God i love chisato

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
            LevelUp();
        }

        else {
            currentXP += amount;
        }
    }

    //Sex

    public void LevelUp() {
        level++;
        skillPoints++;
    }

    public void AddSkillType(SkillType type) {
        if (skillPoints > 0) {
            switch (type) {
                case SkillType.Vigor:
                    vigor++;

                    skillPoints--;
                    break;

                case SkillType.Endurance:
                    endurance++;


                    skillPoints--;
                    break;

                case SkillType.Strength:
                    strength++;


                    skillPoints--;
                    break;

                case SkillType.Dexterity:
                    dexterity++;


                    skillPoints--;
                    break;

                default:
                    Debug.Log("Invalid Skill Type in " + this);
                    break;
            }
        }
    }

}