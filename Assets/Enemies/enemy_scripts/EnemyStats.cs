using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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


    //this is referenced in the SwordDamage.cs script
    public void EnemyTakeDamage(int amount)
    {
        //here animation is just for this one, need to change in the future 
        animator.SetTrigger("TakeDamage");
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
    }

    //in the future add animations and stuff like that
    private void Die()
    {

        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        Destroy(gameObject, 1.5f);
    }
}
