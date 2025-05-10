using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;

    [Header("Attack Timing")]
    public float warningDelay = 2f;

    [Header("Movement Settings")]
    public float chaseRange = 6f;
    public float stopDistance = 1.5f;
    public float rotationSpeed = 5f;

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

    // If the player is outside of attack range, chase them
    if (distance > attackRange && distance <= chaseRange)
    {
        agent.SetDestination(player.position);
        animator?.SetBool("IsWalking", true);
    }
    // Stop moving and idle if the player is in attack range
    else if (distance <= attackRange)
    {
        agent.SetDestination(transform.position);
        animator?.SetBool("IsWalking", false);

        // Begin attack if able
        if (canAttack)
            StartCoroutine(PerformAttack());
    }
    // Player is too far away — stop moving
    else if (distance > chaseRange)
    {
        agent.SetDestination(transform.position);
        animator?.SetBool("IsWalking", false);
    }

    // Rotate toward the player unless attacking
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
            Debug.LogWarning("Animator não está atribuído ao EnemyCombat.");
            yield break;
        }

        canAttack = false;
        isAttackExecuting = true;

        // Salvar a última posição conhecida do jogador
        lastKnownPlayerPosition = player.position;

        // Garantir que o trigger de ataque seja resetado antes de ativar
        GetComponent<Animator>().ResetTrigger("Attack");
        GetComponent<Animator>().SetTrigger("Attack");

        // Esperar antes de atacar (telegraph)
        yield return new WaitForSeconds(warningDelay);

        // Rotacionar para a posição salva antes de atacar
        RotateTowards(lastKnownPlayerPosition);

        float distance = Vector3.Distance(transform.position, lastKnownPlayerPosition);
        Vector3 directionToLastKnown = (lastKnownPlayerPosition - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToLastKnown);

        if (distance <= attackRange && angle < 45f)
        {
            // Verificar se o jogador ainda está na área de dano
            float currentDistance = Vector3.Distance(transform.position, player.position);
            Vector3 currentDirection = (player.position - transform.position).normalized;
            float currentAngle = Vector3.Angle(transform.forward, currentDirection);

            if (currentDistance <= attackRange && currentAngle < 45f)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.PlayerTakeDamage(attackDamage);
                }
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
