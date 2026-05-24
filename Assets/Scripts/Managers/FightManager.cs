using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class FightManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private Transform playerSpawnPoint = null;
    [SerializeField] private Transform enemySpawnPoint = null;
    [SerializeField] private GameObject ferchoPokerPrefab = null;
    [SerializeField] private GameObject cruzRusherPrefab = null;
    [SerializeField] private GameObject kingBlingPrefab = null;
    [SerializeField] private GameObject fallbackPlayerPrefab = null;
    [SerializeField] private GameObject enemyPrefab = null;

    [Header("Existing Scene Fighters")]
    [SerializeField] private GameObject scenePlayer = null;
    [SerializeField] private GameObject sceneEnemy = null;

    [Header("Health")]
    [SerializeField] private HealthSystem playerHealth = null;
    [SerializeField] private HealthSystem enemyHealth = null;
    [SerializeField] private CharacterData fallbackPlayerCharacterData = null;
    [SerializeField] private CharacterData enemyCharacterData = null;
    [SerializeField] private int defaultPlayerHealth = 100;
    [SerializeField] private int defaultEnemyHealth = 100;

    [Header("UI")]
    [SerializeField] private Text countdownText = null;
    [SerializeField] private Text resultText = null;
    [SerializeField] private GameObject koObject = null;
    [SerializeField] private GameObject youLoseObject = null;
    [SerializeField] private ResultMenuController resultMenu = null;
    [SerializeField] private FightCountdownImageUI countdownImageUI = null;

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
        Debug.Log($"{nameof(FightManager)}: Start. GameManager={(GameManager.Instance != null ? "found" : "missing")}, selectedFighter={(GameManager.Instance != null ? GameManager.Instance.SelectedFighterCharacter.ToString() : "None")}.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameFlowState.Fighting);
        }

        HideFightResult();
        SpawnOrUseSceneCharacters();
        ConfigureFighters();
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

        if (countdownImageUI != null)
        {
            yield return StartCoroutine(countdownImageUI.PlayCountdown());
        }

        SetCombatActive(true);
    }

    private void SpawnOrUseSceneCharacters()
    {
        if (scenePlayer != null && GameManager.Instance == null)
        {
            Debug.Log($"{nameof(FightManager)}: Using scenePlayer {scenePlayer.name} because GameManager is missing.");
            player = scenePlayer;
        }
        else
        {
            if (scenePlayer != null)
            {
                Debug.Log($"{nameof(FightManager)}: Ignoring scenePlayer {scenePlayer.name}; spawning selected player from GameManager.");
            }

            player = SpawnPlayer();
        }

        enemy = sceneEnemy != null ? sceneEnemy : SpawnEnemy();
    }

    private GameObject SpawnPlayer()
    {
        FighterCharacter selectedCharacter = GameManager.Instance != null
            ? GameManager.Instance.SelectedFighterCharacter
            : FighterCharacter.FerchoPoker;

        Debug.Log($"{nameof(FightManager)}: SpawnPlayer selected fighter={selectedCharacter}.");

        GameObject prefabToSpawn = GetPlayerPrefabFor(selectedCharacter);

        if (prefabToSpawn == null && GameManager.Instance != null)
        {
            prefabToSpawn = GameManager.Instance.SelectedCharacterPrefab;
        }

        if (prefabToSpawn == null)
        {
            prefabToSpawn = fallbackPlayerPrefab;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"{nameof(FightManager)}: Missing player prefab for {selectedCharacter}. Assign Fercho/Cruz/King prefabs or fallbackPlayerPrefab.");
        }
        else
        {
            Debug.Log($"{nameof(FightManager)}: Instantiating player prefab {prefabToSpawn.name} for {selectedCharacter}.");
        }

        return SpawnPrefab(prefabToSpawn, playerSpawnPoint);
    }

    private GameObject SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"{nameof(FightManager)}: enemyPrefab is missing. Assign Enemy_Test or a temporary enemy prefab.");
        }

        return SpawnPrefab(enemyPrefab, enemySpawnPoint);
    }

    private GameObject SpawnPrefab(GameObject prefab, Transform spawnPoint)
    {
        if (prefab == null)
        {
            return null;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning($"{nameof(FightManager)}: Spawn point is missing for prefab {prefab.name}. Spawning at world origin.");
        }

        Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        return Instantiate(prefab, position, rotation);
    }

    private GameObject GetPlayerPrefabFor(FighterCharacter fighterCharacter)
    {
        switch (fighterCharacter)
        {
            case FighterCharacter.CruzRusher:
                if (cruzRusherPrefab == null)
                {
                    Debug.LogWarning($"{nameof(FightManager)}: CruzRusher prefab is missing. Falling back to FerchoPoker prefab.");
                }

                Debug.Log($"{nameof(FightManager)}: Prefab lookup CruzRusher -> {(cruzRusherPrefab != null ? cruzRusherPrefab.name : "None")}.");
                return cruzRusherPrefab != null ? cruzRusherPrefab : ferchoPokerPrefab;
            case FighterCharacter.KingBling:
                if (kingBlingPrefab == null)
                {
                    Debug.LogWarning($"{nameof(FightManager)}: KingBling prefab is missing. Falling back to FerchoPoker prefab.");
                }

                Debug.Log($"{nameof(FightManager)}: Prefab lookup KingBling -> {(kingBlingPrefab != null ? kingBlingPrefab.name : "None")}.");
                return kingBlingPrefab != null ? kingBlingPrefab : ferchoPokerPrefab;
            case FighterCharacter.FerchoPoker:
            default:
                if (ferchoPokerPrefab == null)
                {
                    Debug.LogWarning($"{nameof(FightManager)}: FerchoPoker prefab is missing.");
                }

                Debug.Log($"{nameof(FightManager)}: Prefab lookup FerchoPoker -> {(ferchoPokerPrefab != null ? ferchoPokerPrefab.name : "None")}.");
                return ferchoPokerPrefab;
        }
    }

    private void InitializeHealth()
    {
        CharacterData selectedCharacter = GetSelectedPlayerCharacterData();

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
            int health = selectedCharacter != null ? selectedCharacter.maxHealth : defaultPlayerHealth;
            playerHealth.Initialize(health);
            playerHealth.OnDeath += HandlePlayerDeath;
        }
        else
        {
            Debug.LogWarning($"{nameof(FightManager)}: Player HealthSystem is missing.");
        }

        if (enemyHealth != null)
        {
            int health = enemyCharacterData != null ? enemyCharacterData.maxHealth : defaultEnemyHealth;
            enemyHealth.Initialize(health);
            enemyHealth.OnDeath += HandleEnemyDeath;
        }
        else
        {
            Debug.LogWarning($"{nameof(FightManager)}: Enemy HealthSystem is missing.");
        }
    }

    private void ConfigureFighters()
    {
        ConfigureFighter(player, FighterTeam.Player, GetSelectedPlayerCharacterData());
        ConfigureFighter(enemy, FighterTeam.Enemy, enemyCharacterData);
    }

    private void ConfigureFighter(GameObject fighter, FighterTeam team, CharacterData characterData)
    {
        if (fighter == null)
        {
            return;
        }

        DamageReceiver[] receivers = fighter.GetComponentsInChildren<DamageReceiver>(true);
        for (int i = 0; i < receivers.Length; i++)
        {
            receivers[i].SetTeam(team);
        }

        HitboxController[] hitboxes = fighter.GetComponentsInChildren<HitboxController>(true);
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxes[i].SetOwnerRoot(fighter.transform);
            hitboxes[i].SetOwnerTeam(team);
        }

        AttackController attackController = fighter.GetComponentInChildren<AttackController>(true);
        if (attackController != null)
        {
            attackController.Configure(team, characterData);
        }

        SpecialPowerController specialPowerController = fighter.GetComponentInChildren<SpecialPowerController>(true);
        if (specialPowerController != null)
        {
            specialPowerController.Configure(team, characterData);

            if (team == FighterTeam.Player && characterData == null && GameManager.Instance != null)
            {
                specialPowerController.SetFighterCharacter(GameManager.Instance.SelectedFighterCharacter);
            }
        }

        PlayerController playerController = fighter.GetComponentInChildren<PlayerController>(true);
        if (playerController != null && characterData != null)
        {
            playerController.ConfigureMovement(characterData.moveSpeed, characterData.jumpVelocity);
        }
    }

    private CharacterData GetSelectedPlayerCharacterData()
    {
        if (GameManager.Instance != null && GameManager.Instance.SelectedCharacter != null)
        {
            return GameManager.Instance.SelectedCharacter;
        }

        return fallbackPlayerCharacterData;
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
