using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 3f; // Maior alcance de ataque
    public float attackCooldown = 3f; // Tempo maior entre ataques
    public int attackDamage = 25; // Dano maior para o chefe


    [Header("Attack Timing")]
    public float warningDelay = 1.5f;

    [Header("Movement Settings")]
    public float chaseRange = 15f; // Maior alcance de perseguição
    public float stopDistance = 2.5f;
    public float rotationSpeed = 3f;

    [Header("Animation")]
    public Animator animator;

    private Transform player;
    private bool canAttack = true;
    private NavMeshAgent agent;
    private bool isAttackExecuting = false;

    private Vector3 lastKnownPlayerPosition;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (animator == null)
            animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Se o jogador estiver fora do alcance de ataque, persiga-o
        if (distance > attackRange && distance <= chaseRange)
        {
            agent.SetDestination(player.position);
            animator?.SetBool("IsWalking", true);
        }
        // Pare de se mover e ataque se o jogador estiver no alcance
        else if (distance <= attackRange)
        {
            agent.SetDestination(transform.position);
            animator?.SetBool("IsWalking", false);

            if (canAttack)
                StartCoroutine(PerformAttack());
        }
        // Se o jogador estiver muito longe, pare de se mover
        else if (distance > chaseRange)
        {
            agent.SetDestination(transform.position);
            animator?.SetBool("IsWalking", false);
        }

        // Rotacione em direção ao jogador, a menos que esteja atacando
        if (!isAttackExecuting)
        {
            RotateTowards(player.position);
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator PerformAttack()
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator não está atribuído ao BossCombat.");
            yield break;
        }

        canAttack = false;
        isAttackExecuting = true;

        lastKnownPlayerPosition = player.position;

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(warningDelay);

        RotateTowards(lastKnownPlayerPosition);

        float distance = Vector3.Distance(transform.position, lastKnownPlayerPosition);
        if (distance <= attackRange)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.PlayerTakeDamage(attackDamage);
            }
        }

        isAttackExecuting = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
