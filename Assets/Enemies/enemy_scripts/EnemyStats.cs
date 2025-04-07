using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    [Header("Detection")]
    public float followRange = 15f;
    public float stopDistance = 2f;

    private Transform player;
    private NavMeshAgent agent;

    private void Start()
    {
        CurrentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            Debug.LogWarning("NavMeshAgent was not found. Adding one to the enemy.");
        }
    }

    private void Update()
    {
        if (player != null && CurrentHealth > 0)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= followRange && distance > stopDistance)
            {
                FollowPlayer();
            }
            else
            {
                agent.SetDestination(transform.position); // Stop moving
            }
        }
    }

    private void FollowPlayer()
    {
        if (agent != null)
        {
            agent.destination = player.position;
        }
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        GetComponent<EnemyCombat>()?.PlayTakeDamageAnimation();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        agent.isStopped = true;
        Destroy(gameObject, 1.5f);
    }

    private void OnGUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2);
        GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y, 100, 20), $"HP: {CurrentHealth}");
    }
}
