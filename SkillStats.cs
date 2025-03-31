using Ink.Parsed;
using UnityEngine;
using System.Collections.Generic;

public class SkillStats : MonoBehaviour
{
    public static SkillStats Instance;

    [Header("Health")]
    public float maxHealth;
    public float takenDamage;
    public float healthTarget;

    int healthLvl = 0, maxHealthLvl = 10;
    public List<float> newHealth;
    public List<float> damageCheckpoint;

    [Header("Stamina")]
    public float maxStamina;
    public float staminaUsed;

    [Header("Walking speed")]
    public float speed;
    public float distanceWalked;
    public float speedTarget;

    int speedLvl = 0, maxSpeedLvl = 10;
    public List<float> Speed;
    public List<float> speedGoals;

    [Header("Jump")]
    public float jumpForce = 150;
    public float jumpsJumped;

    [Header("Struggle mechanic")]
    public float struggle;
    public float stuggleAmount;


    private void Awake()
    {
        Instance = this;

        speedTarget = speedGoals[0];
        speed = Speed[0];

        maxHealth = newHealth[0];
        takenDamage = damageCheckpoint[0];
    }
    public void Health()
    {
        if (healthLvl != maxHealthLvl && takenDamage >= healthTarget)
        {
            healthLvl++;
            healthTarget = newHealth[healthLvl];
        }

        for (int i = 0; i < newHealth.Count; i++)
        {
            if (takenDamage >= newHealth[i])
            {
                maxHealth = newHealth[i];
            }
        }
    }

    public void Walking()
    {
        if(speedLvl != maxSpeedLvl && distanceWalked >= speedTarget)
        {
            speedLvl++;
            speedTarget = speedGoals[speedLvl];
        }

        for (int i = 0; i < speedGoals.Count; i++)
        {
            if (distanceWalked >= speedGoals[i] && Player.Instance.canChange)
            {
                speed = Speed[i];
            }
        }
    }
}
