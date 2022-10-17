using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgession : MonoBehaviour
{
    public int level;
    public float currentXP;
    public float levelupXP;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            Debug.Log("Current XP: " + currentXP + ". Level Up XP: " + levelupXP + ". Level: " + level);
            AddXP(3);
        }
    }

    public void AddXP(float amount) {
        if (currentXP + amount >= levelupXP) {
            currentXP = (currentXP + amount) - levelupXP;
            levelupXP += 10;
            LevelUp();
        }

        else {
            currentXP += amount;
        }
    }

    public void LevelUp() {
        level++;
    }
} 
