using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 10; // Vida maior para o chefe
    public int CurrentHealth { get; private set; }

    [Header("Detection")]
    public float followRange = 20f; // Maior alcance de detecção
    public float stopDistance = 3f;

    [Header("UI")]
    public GameObject healthBarPrefab;

    [Header("Animation")]
    public Animator animator;

    private Slider healthSlider;
    private Transform healthBar;
    private Vector3 healthBarOffset = new Vector3(0, 4f, 0); // Ajuste para o tamanho do chefe

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

    // Método chamado para aplicar dano ao chefe
    public void BossTakeDamage(int amount)
    {
        if (animator != null)
        {
            animator.SetBool("IsTakingDamage", true);
        }

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        // Imprime a vida atual no console
        Debug.Log($"Boss Current Health: {CurrentHealth}");

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
            // Resetar o bool após um curto intervalo
            StartCoroutine(ResetTakeDamageBool());
        }
    }

    private IEnumerator ResetTakeDamageBool()
    {
        yield return new WaitForSeconds(0.1f); // Ajuste o tempo conforme necessário
        if (animator != null)
        {
            animator.SetBool("IsTakingDamage", false);
        }
    }

    // Método chamado quando o chefe morre
    private void Die()
    {
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        Destroy(gameObject, 1.5f);
        // Inicia a transição de cena com fade
        SceneManager.LoadScene("Gambling1");
    }

}
