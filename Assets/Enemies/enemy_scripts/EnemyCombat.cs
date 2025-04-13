using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;      // Maximum range at which the sword can damage the player
    public float attackCooldown = 2f;   // Time between consecutive attacks
    public int attackDamage = 10;       // Damage dealt by the sword

    [Header("Attack Timing")]
    public float warningDelay = 0.5f;   // Wind-up delay before the sword strike deals damage

    [Header("Movement Settings")]
    public float chaseRange = 6f;       // Range within which the enemy will chase the player
    public float stopDistance = 1.5f;   // Distance at which the enemy stops moving to attack
    public float rotationSpeed = 5f;    // Speed at which the enemy rotates toward the player

    [Header("Animation")]
    public Animator animator;

    private Transform player;
    private bool canAttack = true;
    private NavMeshAgent agent;

    // Flag indicating that the enemy attack is actively executing its damage window.
    private bool isAttackExecuting = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (animator == null)
            animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        // Disable automatic agent rotation so we can manually control facing.
        agent.updateRotation = false;
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Movement: Chase the player if within chaseRange but outside stopDistance.
        if (distance <= chaseRange && distance > stopDistance)
        {
            agent.SetDestination(player.position);
            animator?.SetBool("IsWalking", true);
        }
        // If the player is too far away, stop chasing.
        else if (distance > chaseRange)
        {
            agent.SetDestination(transform.position);
            animator?.SetBool("IsWalking", false);
        }
        // If the enemy is close enough, stop moving and try to attack.
        else if (distance <= stopDistance)
        {
            agent.SetDestination(transform.position);
            animator?.SetBool("IsWalking", false);

            if (canAttack)
            {
                StartCoroutine(PerformAttack());
            }
        }

        // Smoothly rotate toward the player so the enemy always faces its target.
        Vector3 direction = player.position - transform.position;
        direction.y = 0f; // Keep rotation horizontal.
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;
        isAttackExecuting = true;

        // Trigger the attack animation.
        animator?.SetTrigger("Attack");

        // Wait for the wind-up (warning) delay.
        yield return new WaitForSeconds(warningDelay);

        // Only apply damage if the enemy is still executing its attack.
        if (isAttackExecuting)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            // Apply damage only if the player is within attackRange and roughly in front (within 45°).
            if (distance <= attackRange && angleToPlayer < 45f)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(attackDamage);
                    Debug.Log("Player took damage from enemy sword!");
                }
            }
        }

        // End the active attack window.
        isAttackExecuting = false;

        // Wait for the cooldown period before the next attack.
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack and chase ranges in the Editor.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
