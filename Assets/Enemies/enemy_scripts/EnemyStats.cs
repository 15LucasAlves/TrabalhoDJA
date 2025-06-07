using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    [Header("Detection")]
    public float followRange = 15f;
    public float stopDistance = 2f;

    [Header("UI")]
    public GameObject healthBarPrefab;

    [Header("Animation")]
    public Animator animator;

    [Header("Effects")]
    public GameObject hitEffectPrefab;

    private Slider healthSlider;
    private Transform healthBar;
    private Vector3 healthBarOffset = new Vector3(0, 2.2f, 0);

    private Transform player;

    private Vector3 hitEffectOffset = new Vector3(0, 1.0f, 0); // Offset for hit effect

    private bool isDead = false;  // Track if enemy is already dead

    private void Start()
    {
        CurrentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthBar = hb.transform;
            healthSlider = hb.GetComponentInChildren<Slider>();
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = CurrentHealth;
            }
        }
    }

    private void Update()
    {
        if (healthBar != null)
        {
            healthBar.position = transform.position + healthBarOffset;
            healthBar.forward = Camera.main.transform.forward;
        }
    }

    // Called to apply damage to the enemy
    public void EnemyTakeDamage(int amount)
    {
        if (isDead) return; // Do not process damage if dead

        if (animator != null)
        {
            animator.SetBool("IsTakingDamage", true);
        }

        // Instantiate the hit effect slightly above the enemy
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position + hitEffectOffset, Quaternion.identity);
            Destroy(effect, 0.5f);
        }

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth;
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Reset the damage animation flag shortly after
            StartCoroutine(ResetTakeDamageBool());
        }
    }

    private IEnumerator ResetTakeDamageBool()
    {
        yield return new WaitForSeconds(0.01f); // Adjust timing as needed
        if (animator != null)
        {
            animator.SetBool("IsTakingDamage", false);
        }
    }

    // Called when the enemy dies
    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        if (animator != null)
        {
            animator.SetBool("IsDead", true);  // Trigger death animation
        }

        // Destroy enemy game object after the death animation plays
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(5f);  // Adjust to match your death animation length

        Destroy(gameObject);
    }
}
