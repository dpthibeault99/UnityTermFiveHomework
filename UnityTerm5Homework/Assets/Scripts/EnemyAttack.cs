using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform attackTransform;
    public float attackRadius;
    public float attackDamage = 10.0f;

    public void Attack()
    {
        Collider[] attackHits = Physics.OverlapSphere(attackTransform.position, attackRadius);

        foreach(var attackHit in attackHits)
        {
            if (attackHit.gameObject.CompareTag("Player"))
            {
                PlayerHealth health = attackHit.GetComponentInParent<PlayerHealth>();
                if (health != null) health.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (attackTransform == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackTransform.position, attackRadius);
    }
}
