using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject attackAreaPrefab;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;

    [Header("Attack Timing")]
    public float warningDelay = 0.5f;
    public float damageDuration = 0.2f;

    [Header("Movement")]
    public float chaseRange = 6f;
    public float stopDistance = 1.5f;

    [Header("Animation")]
    public Animator animator;

    private Transform player;
    private bool canAttack = true;
    private NavMeshAgent agent;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (animator == null)
            animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Chase player if within chase range but not too close
        if (distance <= chaseRange && distance > stopDistance)
        {
            agent.SetDestination(player.position);
            animator?.SetBool("IsWalking", true);
        }
        else
        {
            agent.SetDestination(transform.position);
            animator?.SetBool("IsWalking", false);
        }

        // Attack if close enough
        if (canAttack && distance <= attackRange)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        // 🗡️ Play attack animation
        animator?.SetTrigger("Attack");

        // Step 1: Spawn the attack area
        Vector3 attackPosition = transform.position + transform.forward * 1.5f;
        GameObject attackArea = Instantiate(attackAreaPrefab, attackPosition, transform.rotation);
        attackArea.transform.localScale = new Vector3(1f, 1f, 1f);

        // Step 2: Show warning color
        Renderer rend = attackArea.GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = new Color(1f, 1f, 1f, 0.4f); // light gray

        yield return new WaitForSeconds(warningDelay);

        // Step 3: Change to red and apply damage
        if (rend != null)
            rend.material.color = new Color(1f, 0f, 0f, 0.5f);

        Collider[] hits = Physics.OverlapBox(
            attackArea.transform.position,
            attackArea.transform.localScale / 2f,
            attackArea.transform.rotation
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerStats playerStats = hit.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(attackDamage);
                    Debug.Log("Player took damage!");
                }
            }
        }

        yield return new WaitForSeconds(damageDuration);
        Destroy(attackArea);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void PlayTakeDamageAnimation()
    {
        animator?.SetTrigger("TakeDamage");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
