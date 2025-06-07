using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using StarterAssets;

public class BossStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 10;
    public int CurrentHealth { get; private set; }

    [Header("UI")]
    public GameObject healthBarPrefab;
    public Canvas optionCanvas;
    public Canvas redResultCanvas;
    public Canvas blackResultCanvas;
    public float showCanvasDelay = 1.5f;
    public float resultDisplayTime = 2f;

    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public float launchForce = 100f;
    public Vector3 ballSpawnOffset = new Vector3(0, 1f, 0);

    [Header("Boss Settings")]
    public GameObject bossPrefab;

    [Header("Health Bar")]
    public Image healthFillImage;  // Deve ser atribuído manualmente no Inspector

    private Transform healthBar;  // Referência ao objeto da barra de vida instanciado
    private bool isDead = false;
    private Vector3 deathPosition;
    private Coroutine showUICoroutine;

    private bool playerChoseRed;
    private GameObject spawnedBall;
    private string lastCollidedLayer;

    private PlayerAttackSystem playerAttackSystem;
    private ThirdPersonController playerController;
    private GameObject player;

    private void Start()
    {
        CurrentHealth = maxHealth;
        InitializeHealthBar();

        Debug.Log("Boss starting health: " + CurrentHealth);

        if (optionCanvas != null) optionCanvas.gameObject.SetActive(false);
        if (redResultCanvas != null) redResultCanvas.gameObject.SetActive(false);
        if (blackResultCanvas != null) blackResultCanvas.gameObject.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerAttackSystem = player.GetComponent<PlayerAttackSystem>();
            playerController = player.GetComponent<ThirdPersonController>();
        }
    }

    private void InitializeHealthBar()
    {
        if (healthBarPrefab != null)
        {
            Canvas mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas != null)
            {
                if (healthBar != null)
                {
                    Destroy(healthBar.gameObject);
                }

                GameObject hb = Instantiate(healthBarPrefab, mainCanvas.transform);
                healthBar = hb.transform;

                // ATENÇÃO: após instanciar, você precisa atribuir a healthFillImage manualmente no Inspector
                // Por exemplo, arraste o Image Fill da nova barra para este campo no Inspector!

                UpdateHealthBarVisual();
            }
        }
    }

    private void UpdateHealthBarVisual()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (float)CurrentHealth / maxHealth;
        }
    }

    public void BossTakeDamage(int amount)
    {
        if (isDead) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        Debug.Log("Boss current health: " + CurrentHealth);

        UpdateHealthBarVisual();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        deathPosition = transform.position;

        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        SetBossVisible(false);
        playerAttackSystem?.DisableAttacks();

        if (showUICoroutine != null) StopCoroutine(showUICoroutine);
        showUICoroutine = StartCoroutine(ShowOptionUI());
    }

    private void SetBossVisible(bool visible)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = visible;
        }

        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = visible;
        }
    }

    private IEnumerator ShowOptionUI()
    {
        yield return new WaitForSeconds(showCanvasDelay);

        if (optionCanvas != null)
        {
            optionCanvas.gameObject.SetActive(true);
            SetCursorState(true);

            if (playerController != null)
            {
                playerController.LockCameraPosition = true;
            }

            Button[] buttons = optionCanvas.GetComponentsInChildren<Button>(true);
            if (buttons.Length >= 2)
            {
                buttons[0].onClick.RemoveAllListeners();
                buttons[0].onClick.AddListener(() => SelectOption(true));

                buttons[1].onClick.RemoveAllListeners();
                buttons[1].onClick.AddListener(() => SelectOption(false));
            }
        }
    }

    private void SelectOption(bool choseRed)
    {
        playerChoseRed = choseRed;
        Debug.Log("Botão pressionado: " + (choseRed ? "Vermelho" : "Preto"));

        if (optionCanvas != null)
        {
            optionCanvas.gameObject.SetActive(false);
        }

        RestorePlayerControl();
        LaunchBall(choseRed);
    }

    private void RestorePlayerControl()
    {
        SetCursorState(false);
        if (playerController != null)
        {
            playerController.LockCameraPosition = false;
        }

        playerAttackSystem?.EnableAttacks();
    }

    private void LaunchBall(bool choseRed)
    {
        if (ballPrefab == null) return;

        Vector3 spawnPosition = deathPosition + ballSpawnOffset;
        spawnedBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

        BallCollisionTracker tracker = spawnedBall.AddComponent<BallCollisionTracker>();
        tracker.Initialize(this);

        Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            float direction = choseRed ? 1f : -1f;

            Vector3 force = new Vector3(direction * launchForce, 0, 0);
            rb.AddForce(force, ForceMode.Impulse);

            Vector3 spinAxis = Vector3.up;
            float spinAmount = 900f;
            rb.AddTorque(spinAxis * spinAmount, ForceMode.Impulse);
        }

        StartCoroutine(CheckBallDecisionAfterDelay());
    }

    public void OnBallCollision(GameObject collidedObject)
    {
        string layerName = LayerMask.LayerToName(collidedObject.layer);
        if (layerName == "Red" || layerName == "Black")
        {
            lastCollidedLayer = layerName;
            Debug.Log("Colisão com: " + layerName);
        }
    }

    private IEnumerator CheckBallDecisionAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        if (spawnedBall == null) yield break;

        if (!string.IsNullOrEmpty(lastCollidedLayer))
        {
            if (lastCollidedLayer == "Red" && redResultCanvas != null)
            {
                redResultCanvas.gameObject.SetActive(true);
                yield return new WaitForSeconds(resultDisplayTime);
                redResultCanvas.gameObject.SetActive(false);
            }
            else if (lastCollidedLayer == "Black" && blackResultCanvas != null)
            {
                blackResultCanvas.gameObject.SetActive(true);
                yield return new WaitForSeconds(resultDisplayTime);
                blackResultCanvas.gameObject.SetActive(false);
            }
        }

        bool errou = !string.IsNullOrEmpty(lastCollidedLayer) &&
                      ((playerChoseRed && lastCollidedLayer == "Red") ||
                       (!playerChoseRed && lastCollidedLayer == "Black"));

        if (errou)
        {
            RespawnBoss();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
        }
    }

    private void RespawnBoss()
    {
        CurrentHealth = maxHealth;
        isDead = false;
        transform.position = deathPosition;

        InitializeHealthBar();  // RECRIA A BARRA, ATENÇÃO: reatribuir healthFillImage manualmente no Inspector!

        SetBossVisible(true);
        playerAttackSystem?.EnableAttacks();

        if (spawnedBall != null)
        {
            Destroy(spawnedBall);
        }
    }

    private void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnDestroy()
    {
        if (optionCanvas != null)
        {
            Button[] buttons = optionCanvas.GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        if (showUICoroutine != null)
        {
            StopCoroutine(showUICoroutine);
        }

        RestorePlayerControl();
    }
}

public class BallCollisionTracker : MonoBehaviour
{
    private BossStats bossStats;

    public void Initialize(BossStats stats)
    {
        bossStats = stats;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bossStats?.OnBallCollision(collision.gameObject);
    }
}
