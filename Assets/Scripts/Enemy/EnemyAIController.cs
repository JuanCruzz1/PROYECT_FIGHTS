using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    private enum AIState
    {
        Idle,
        Approach,
        KeepDistance,
        Attack,
        Defend,
        Retreat,
        EvadeJump
    }

    [Header("References")]
    [SerializeField] private Transform targetPlayer = null;
    [SerializeField] private Rigidbody2D rb = null;
    [SerializeField] private AttackController attackController = null;
    [SerializeField] private EnemyShieldController shieldController = null;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float attackRange = 1.3f;
    [SerializeField] private float stopDistance = 1.15f;
    [SerializeField] private float personalSpace = 0.75f;
    [SerializeField] private float retreatSpeed = 2f;
    [SerializeField] private float preferredDistance = 1.8f;
    [SerializeField] private float closeDistance = 0.8f;
    [SerializeField] private float maxChaseDistance = 5f;

    [Header("Decisions")]
    [SerializeField] private float attackCooldown = 1.4f;
    [SerializeField] private float decisionInterval = 0.35f;
    [SerializeField] private float minActionDuration = 0.5f;
    [SerializeField] private float maxActionDuration = 1.2f;
    [SerializeField, Range(0f, 1f)] private float approachChance = 0.35f;
    [SerializeField, Range(0f, 1f)] private float attackChance = 0.35f;
    [SerializeField, Range(0f, 1f)] private float retreatChance = 0.25f;
    [SerializeField, Range(0f, 1f)] private float defendChance = 0.2f;
    [SerializeField, Range(0f, 1f)] private float evadeJumpChance = 0.15f;
    [SerializeField, Range(0f, 1f)] private float keepDistanceChance = 0.25f;
    [SerializeField, Range(0f, 1f)] private float postAttackRetreatChance = 0.6f;
    [SerializeField, Range(0f, 1f)] private float shieldChance = 0.25f;
    [SerializeField] private float shieldActivationRange = 2.2f;

    [Header("Evade Jump")]
    [SerializeField] private float evadeJumpForce = 6f;
    [SerializeField] private float evadeBackwardSpeed = 2.5f;
    [SerializeField] private float evadeDuration = 0.45f;
    [SerializeField] private float evadeJumpCooldown = 2.5f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private float nextAttackTime;
    private float nextDecisionTime;
    private float actionEndTime;
    private float nextEvadeJumpTime;
    private float desiredHorizontalVelocity;
    private int facingDirection = -1;
    private AIState currentState = AIState.Idle;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (attackController == null)
        {
            attackController = GetComponent<AttackController>();
        }

        if (attackController != null)
        {
            attackController.Configure(FighterTeam.Enemy, null);
        }

        if (shieldController == null)
        {
            shieldController = GetComponent<EnemyShieldController>();
        }
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        attackController = GetComponent<AttackController>();
        shieldController = GetComponent<EnemyShieldController>();
    }

    private void Update()
    {
        if (targetPlayer == null)
        {
            targetPlayer = FindTargetPlayer();
        }

        if (targetPlayer == null)
        {
            SetState(AIState.Idle);
            return;
        }

        UpdateFacing();

        if (Time.time >= nextDecisionTime && Time.time >= actionEndTime)
        {
            nextDecisionTime = Time.time + Mathf.Max(0.02f, decisionInterval);
            MakeDecision();
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        ApplyStateMovement();
    }

    private void MakeDecision()
    {
        float distance = Mathf.Abs(targetPlayer.position.x - transform.position.x);

        if (distance <= closeDistance)
        {
            DecideCloseRange(distance);
            return;
        }

        if (distance <= attackRange)
        {
            DecideAttackRange(distance);
            return;
        }

        if (distance <= preferredDistance)
        {
            DecideMidRange(distance);
            return;
        }

        DecideFarRange(distance);
    }

    private void DecideCloseRange(float distance)
    {
        if (CanEvadeJump() && Random.value <= evadeJumpChance)
        {
            StartEvadeJump();
            return;
        }

        if (Random.value <= retreatChance)
        {
            StartTimedState(AIState.Retreat);
            return;
        }

        if (TryDefend(distance))
        {
            return;
        }

        StartTimedState(AIState.KeepDistance);
    }

    private void DecideAttackRange(float distance)
    {
        if (TryDefend(distance))
        {
            return;
        }

        if (Time.time >= nextAttackTime && Random.value <= attackChance)
        {
            StartTimedState(AIState.Attack);
            TryAttack(distance);
            return;
        }

        if (Random.value <= keepDistanceChance)
        {
            StartTimedState(AIState.KeepDistance);
            return;
        }

        if (Random.value <= retreatChance)
        {
            StartTimedState(AIState.Retreat);
            return;
        }

        StartTimedState(AIState.Idle);
    }

    private void DecideMidRange(float distance)
    {
        if (TryDefend(distance))
        {
            return;
        }

        if (Random.value <= approachChance)
        {
            StartTimedState(AIState.Approach);
            return;
        }

        if (Random.value <= keepDistanceChance)
        {
            StartTimedState(AIState.KeepDistance);
            return;
        }

        StartTimedState(AIState.Idle);
    }

    private void DecideFarRange(float distance)
    {
        if (distance > maxChaseDistance && Random.value > approachChance)
        {
            StartTimedState(AIState.Idle);
            return;
        }

        if (Random.value <= approachChance)
        {
            StartTimedState(AIState.Approach);
            return;
        }

        StartTimedState(AIState.KeepDistance);
    }

    private bool TryAttack(float distance)
    {
        if (distance > attackRange || Time.time < nextAttackTime || attackController == null)
        {
            return false;
        }

        if (attackController.TryPunch())
        {
            nextAttackTime = Time.time + Mathf.Max(0.01f, attackCooldown);

            if (debugLogs)
            {
                Debug.Log($"{nameof(EnemyAIController)} on {name}: EnemyAI: Attack.");
            }

            if (Random.value <= postAttackRetreatChance)
            {
                StartTimedState(AIState.Retreat);
            }

            return true;
        }

        return false;
    }

    private bool TryDefend(float distance)
    {
        if (Random.value > defendChance)
        {
            return false;
        }

        if (TryUseShield(distance))
        {
            StartTimedState(AIState.Defend);
            return true;
        }

        return false;
    }

    private bool TryUseShield(float distance)
    {
        if (distance > shieldActivationRange || shieldController == null || !CanActivateShield())
        {
            return false;
        }

        if (Random.value <= shieldChance && TryActivateShield())
        {
            if (debugLogs)
            {
                Debug.Log($"{nameof(EnemyAIController)} on {name}: EnemyAI: Defend.");
            }

            return true;
        }

        return false;
    }

    private void UpdateFacing()
    {
        float deltaX = targetPlayer.position.x - transform.position.x;

        if (Mathf.Abs(deltaX) < 0.01f)
        {
            return;
        }

        facingDirection = deltaX >= 0f ? 1 : -1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDirection;
        transform.localScale = scale;
    }

    private Transform FindTargetPlayer()
    {
        GameObject taggedPlayer = null;

        try
        {
            taggedPlayer = GameObject.FindGameObjectWithTag("Player");
        }
        catch (UnityException)
        {
            taggedPlayer = null;
        }

        if (taggedPlayer != null)
        {
            return taggedPlayer.transform;
        }

        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        return playerController != null ? playerController.transform : null;
    }

    private bool CanActivateShield()
    {
        return shieldController != null && shieldController.CanActivate;
    }

    private bool TryActivateShield()
    {
        return shieldController != null && shieldController.TryActivate(facingDirection);
    }

    private bool CanEvadeJump()
    {
        return rb != null && Time.time >= nextEvadeJumpTime;
    }

    private void StartEvadeJump()
    {
        nextEvadeJumpTime = Time.time + Mathf.Max(0.05f, evadeJumpCooldown);
        actionEndTime = Time.time + Mathf.Max(0.05f, evadeDuration);
        SetState(AIState.EvadeJump);

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(-facingDirection * evadeBackwardSpeed, evadeJumpForce);
        }
    }

    private void StartTimedState(AIState newState)
    {
        float duration = Random.Range(Mathf.Min(minActionDuration, maxActionDuration), Mathf.Max(minActionDuration, maxActionDuration));
        actionEndTime = Time.time + Mathf.Max(0.05f, duration);
        SetState(newState);
    }

    private void ApplyStateMovement()
    {
        Vector2 currentVelocity = rb.linearVelocity;

        if (currentState == AIState.EvadeJump && Time.time < actionEndTime)
        {
            currentVelocity.x = -facingDirection * evadeBackwardSpeed;
            rb.linearVelocity = currentVelocity;
            return;
        }

        switch (currentState)
        {
            case AIState.Approach:
                desiredHorizontalVelocity = facingDirection * moveSpeed;
                break;
            case AIState.Retreat:
                desiredHorizontalVelocity = -facingDirection * retreatSpeed;
                break;
            case AIState.KeepDistance:
            case AIState.Attack:
            case AIState.Defend:
            case AIState.Idle:
            default:
                desiredHorizontalVelocity = 0f;
                break;
        }

        currentVelocity.x = desiredHorizontalVelocity;
        rb.linearVelocity = currentVelocity;
    }

    private void SetState(AIState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        if (debugLogs)
        {
            Debug.Log($"{nameof(EnemyAIController)} on {name}: EnemyAI: {currentState}.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, personalSpace);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shieldActivationRange);
    }
}
