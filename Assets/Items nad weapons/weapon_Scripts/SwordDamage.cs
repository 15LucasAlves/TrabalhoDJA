using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmountFastAttack = 10;
    public int damageAmountHeavyAttack = 20;
    public string enemyTag = "Enemy";




    private bool isAttacking = false;
    private bool isHeavy = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        if (other.CompareTag(enemyTag))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                int damage = isHeavy ? damageAmountHeavyAttack : damageAmountFastAttack;
               
                enemy.EnemyTakeDamage(damage);
            }
        }
    }

    public void StartAttack(bool heavy = false)
    {
        isAttacking = true;
        isHeavy = heavy;

      
    }

    public void EndAttack()
    {
        isAttacking = false;
        isHeavy = false;

      
    }

    
}
