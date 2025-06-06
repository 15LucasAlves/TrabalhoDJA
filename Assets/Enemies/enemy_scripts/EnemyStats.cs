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

    private Slider healthSlider;
    private Transform healthBar;
    private Vector3 healthBarOffset = new Vector3(0, 2.2f, 0);

    private Transform player;

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

    // M�todo chamado para aplicar dano ao inimigo
    public void EnemyTakeDamage(int amount)
    {
        if (animator != null)
        {
            animator.SetBool("IsTakingDamage", true);
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
            // Resetar o bool ap�s um curto intervalo
            StartCoroutine(ResetTakeDamageBool());
        }
    }

    private IEnumerator ResetTakeDamageBool()
    {
        yield return new WaitForSeconds(0.01f); // Ajuste o tempo conforme necess�rio
        if (animator != null)
        {
            animator.SetBool("IsTakingDamage", false);
        }
    }

    // M�todo chamado quando o inimigo morre
    private void Die()
    {
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        Destroy(gameObject, 1.5f);
    }
}
