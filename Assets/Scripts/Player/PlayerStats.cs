using UnityEngine;

// Contains the character's stats and attributes used to balance in game
[System.Serializable]
public class PlayerStats
{
    public float acceleration;
    public float maxSpeed;
    public float jumpSpeed;
    public int jumpCount = 2;
    public float knockback = 2;
    public float attackDuration;
    public float attackCooldown;
}
