using UnityEngine;
using System.Collections;

public class SwordDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmountFastAttack = 10;
    public int damageAmountHeavyAttack = 20;
    public string enemyTag = "Enemy";
    public string bossTag = "Boss"; // Adicionado para diferenciar o chefe

    private bool isAttacking = false;
    private bool isHeavy = false;

    private Collider swordCollider; // Referência ao Collider da espada

    private void Awake()
    {
        // Obtém o Collider da espada
        swordCollider = GetComponent<Collider>();
        if (swordCollider != null)
        {
            swordCollider.enabled = false; // Desativa o Collider inicialmente
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        // Verifica se o objeto atingido é um inimigo comum
        if (other.CompareTag(enemyTag))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                int damage = isHeavy ? damageAmountHeavyAttack : damageAmountFastAttack;
                enemy.EnemyTakeDamage(damage);
            }
        }

        // Verifica se o objeto atingido é o chefe
        if (other.CompareTag(bossTag))
        {
            BossStats boss = other.GetComponent<BossStats>();
            if (boss != null)
            {
                int damage = isHeavy ? damageAmountHeavyAttack : damageAmountFastAttack;
                boss.BossTakeDamage(damage); // Aplica dano ao chefe
            }
        }
    }

    public void StartAttack(bool heavy = false, float delay = 0.5f)
    {
        isHeavy = heavy;
        StartCoroutine(ActivateHitboxWithDelay(delay));
    }

    private IEnumerator ActivateHitboxWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Aguarda o tempo do atraso

        isAttacking = true;

        // Ativa o Collider da espada
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        isHeavy = false;

        // Desativa o Collider da espada
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
    }

    // Método público para aplicar dano diretamente
    public void ApplyDamage(GameObject target)
    {
        if (target.CompareTag(enemyTag))
        {
            EnemyStats enemy = target.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                int damage = isHeavy ? damageAmountHeavyAttack : damageAmountFastAttack;
                enemy.EnemyTakeDamage(damage);
            }
        }
        else if (target.CompareTag(bossTag))
        {
            BossStats boss = target.GetComponent<BossStats>();
            if (boss != null)
            {
                int damage = isHeavy ? damageAmountHeavyAttack : damageAmountFastAttack;
                boss.BossTakeDamage(damage);
            }
        }
    }
}
