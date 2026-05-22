using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class FightManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private Transform playerSpawnPoint = null;
    [SerializeField] private Transform enemySpawnPoint = null;
    [SerializeField] private GameObject fallbackPlayerPrefab = null;
    [SerializeField] private GameObject enemyPrefab = null;

    [Header("Existing Scene Fighters")]
    [SerializeField] private GameObject scenePlayer = null;
    [SerializeField] private GameObject sceneEnemy = null;

    [Header("Health")]
    [SerializeField] private HealthSystem playerHealth = null;
    [SerializeField] private HealthSystem enemyHealth = null;
    [SerializeField] private int defaultPlayerHealth = 100;
    [SerializeField] private int defaultEnemyHealth = 100;

    [Header("UI")]
    [SerializeField] private Text countdownText = null;
    [SerializeField] private Text resultText = null;
    [SerializeField] private GameObject koObject = null;
    [SerializeField] private GameObject youLoseObject = null;
    [SerializeField] private ResultMenuController resultMenu = null;

    [Header("Flow")]
    [SerializeField] private bool startCombatOnStart = true;
    [SerializeField] private float resultMenuDelay = 1.5f;

    private GameObject player;
    private GameObject enemy;
    private bool fightEnded;
    private bool combatActive;

    public bool IsCombatActive => combatActive;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameFlowState.Fighting);
        }

        HideFightResult();
        SpawnOrUseSceneCharacters();
        InitializeHealth();

        if (startCombatOnStart)
        {
            StartCoroutine(StartCountdown());
        }
        else
        {
            SetCombatActive(true);
        }
    }

    private void OnDestroy()
    {
        UnsubscribeHealthEvents();
    }

    private IEnumerator StartCountdown()
    {
        SetCombatActive(false);

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            countdownText.text = "3";
            yield return new WaitForSeconds(1f);

            countdownText.text = "2";
            yield return new WaitForSeconds(1f);

            countdownText.text = "1";
            yield return new WaitForSeconds(1f);

            countdownText.gameObject.SetActive(false);
        }

        SetCombatActive(true);
    }

    private void SpawnOrUseSceneCharacters()
    {
        player = scenePlayer != null ? scenePlayer : SpawnPlayer();
        enemy = sceneEnemy != null ? sceneEnemy : SpawnEnemy();
    }

    private GameObject SpawnPlayer()
    {
        GameObject selectedPrefab = GameManager.Instance != null
            ? GameManager.Instance.SelectedCharacterPrefab
            : null;

        GameObject prefabToSpawn = selectedPrefab != null ? selectedPrefab : fallbackPlayerPrefab;

        return SpawnPrefab(prefabToSpawn, playerSpawnPoint);
    }

    private GameObject SpawnEnemy()
    {
        return SpawnPrefab(enemyPrefab, enemySpawnPoint);
    }

    private GameObject SpawnPrefab(GameObject prefab, Transform spawnPoint)
    {
        if (prefab == null)
        {
            return null;
        }

        Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        return Instantiate(prefab, position, rotation);
    }

    private void InitializeHealth()
    {
        if (playerHealth == null && player != null)
        {
            playerHealth = player.GetComponentInChildren<HealthSystem>();
        }

        if (enemyHealth == null && enemy != null)
        {
            enemyHealth = enemy.GetComponentInChildren<HealthSystem>();
        }

        if (playerHealth != null)
        {
            playerHealth.Initialize(defaultPlayerHealth);
            playerHealth.OnDeath += HandlePlayerDeath;
        }
        else
        {
            Debug.LogWarning($"{nameof(FightManager)}: Player HealthSystem is missing.");
        }

        if (enemyHealth != null)
        {
            enemyHealth.Initialize(defaultEnemyHealth);
            enemyHealth.OnDeath += HandleEnemyDeath;
        }
        else
        {
            Debug.LogWarning($"{nameof(FightManager)}: Enemy HealthSystem is missing.");
        }
    }

    private void UnsubscribeHealthEvents()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }

        if (enemyHealth != null)
        {
            enemyHealth.OnDeath -= HandleEnemyDeath;
        }
    }

    private void HandlePlayerDeath()
    {
        EndFight(false);
    }

    private void HandleEnemyDeath()
    {
        EndFight(true);
    }

    public void EndFight(bool playerWon)
    {
        if (fightEnded)
        {
            return;
        }

        fightEnded = true;
        Time.timeScale = 1f;
        SetCombatActive(false);

        if (playerWon)
        {
            ShowKO();
            SetGameState(GameFlowState.PlayerWin);
        }
        else
        {
            ShowYouLose();
            SetGameState(GameFlowState.PlayerLose);
        }

        StartCoroutine(ShowResultMenuAfterDelay());
    }

    public void SetCombatActive(bool active)
    {
        combatActive = active;
        SetFighterScriptsActive(player, active);
        SetFighterScriptsActive(enemy, active);
    }

    private void SetFighterScriptsActive(GameObject fighter, bool active)
    {
        if (fighter == null)
        {
            return;
        }

        PlayerInputHandler playerInput = fighter.GetComponentInChildren<PlayerInputHandler>();
        if (playerInput != null)
        {
            playerInput.enabled = active;
        }

        EnemyAIController enemyAI = fighter.GetComponentInChildren<EnemyAIController>();
        if (enemyAI != null)
        {
            enemyAI.enabled = active;
        }

        AttackController attackController = fighter.GetComponentInChildren<AttackController>();
        if (attackController != null)
        {
            attackController.enabled = active;
        }
    }

    private IEnumerator ShowResultMenuAfterDelay()
    {
        if (resultMenuDelay > 0f)
        {
            yield return new WaitForSeconds(resultMenuDelay);
        }

        SetGameState(GameFlowState.ResultMenu);

        if (resultMenu != null)
        {
            resultMenu.Show();
        }
    }

    private void ShowKO()
    {
        if (resultText != null)
        {
            resultText.text = "KO";
            resultText.gameObject.SetActive(true);
        }

        if (koObject != null)
        {
            koObject.SetActive(true);
        }
    }

    private void ShowYouLose()
    {
        if (resultText != null)
        {
            resultText.text = "YOU LOSE";
            resultText.gameObject.SetActive(true);
        }

        if (youLoseObject != null)
        {
            youLoseObject.SetActive(true);
        }
    }

    private void HideFightResult()
    {
        if (resultText != null)
        {
            resultText.gameObject.SetActive(false);
        }

        if (koObject != null)
        {
            koObject.SetActive(false);
        }

        if (youLoseObject != null)
        {
            youLoseObject.SetActive(false);
        }

        if (resultMenu != null)
        {
            resultMenu.Hide();
        }
    }

    private void SetGameState(GameFlowState newState)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(newState);
        }
    }
}
