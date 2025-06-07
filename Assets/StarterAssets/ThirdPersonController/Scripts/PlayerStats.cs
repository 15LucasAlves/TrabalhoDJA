using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    [Header("Health UI")]
    public Image healthBarFill; // Drag your red health bar here
    public float smoothSpeed = 5f;

    [Header("Death Settings")]
    public Animator playerAnimator;    // Assign your player Animator here
    public GameObject deathCanvas;     // Assign the canvas to show on death (set inactive initially)
    public float deathAnimationDuration = 3f;  // Length of death animation in seconds
    public string sceneToLoad;         // Name of scene to load after death sequence


    [Header("Movement")]
    public MonoBehaviour playerMovementScript;

    private bool isDead = false;

    private void Start()
    {
        CurrentHealth = maxHealth;

        if (healthBarFill != null)
            healthBarFill.fillAmount = 1f;

        if (deathCanvas != null)
            deathCanvas.SetActive(false);  // Make sure death canvas is hidden initially
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    public void PlayerTakeDamage(int amount)
    {
        if (isDead) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        // Trigger the hit animation
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Hit");
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died!");

        // Disable player movement
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");  // Assumes you have a "Die" trigger in Animator
        }

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Wait for death animation to finish
        yield return new WaitForSeconds(deathAnimationDuration);

        // Show death canvas UI
        if (deathCanvas != null)
            deathCanvas.SetActive(true);

        // Wait some seconds to let player see the UI before changing scene
        yield return new WaitForSeconds(2f);

        // Change scene
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene to load is not set.");
        }
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
