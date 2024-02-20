using System;
using System.Collections;
using UnityEngine;

// Controls the hitbox used to detect attacks
public class HitboxController : MonoBehaviour
{
    [SerializeField] Collider hitbox;
    Action attackCallback;              // Used to trigger the attack from the network behaviour on the PlayerController script

    // Starts the attack
    public void Attack(float attackDuration, Action Callback)
    {
        StartCoroutine(AttackTime(attackDuration));
        attackCallback = Callback;
    }

    // Let's the hitbox be active for the attack's duration
    IEnumerator AttackTime(float attackDuration)
    {
        hitbox.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        hitbox.enabled = false;
    }

    // Checks what was hit
    private void OnTriggerEnter(Collider other)
    {
        if (other != transform.parent)
        {
            if (other.CompareTag(Tags.PLAYER))
            {
                attackCallback();
            }
            if (other.CompareTag(Tags.HITBOX))
            {
                // this can be used to create a mechanic where the hits cancel each other
            }
        }
    }
}
