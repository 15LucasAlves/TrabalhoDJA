using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    [Header("Health UI")]
    public Image healthBarFill; // Drag your red health bar here
    public float smoothSpeed = 5f;

    private void Start()
    {
        CurrentHealth = maxHealth;

        if (healthBarFill != null)
            healthBarFill.fillAmount = 1f;
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    public void PlayerTakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);


        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // Add respawn logic or death animation here
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float target = (float)CurrentHealth / maxHealth;
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, target, Time.deltaTime * smoothSpeed);
        }
    }
}
