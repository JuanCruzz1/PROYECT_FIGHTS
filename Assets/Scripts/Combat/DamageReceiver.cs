using UnityEngine;

public enum FighterTeam
{
    Neutral,
    Player,
    Enemy
}

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem = null;
    [SerializeField] private EnemyShieldController shieldController = null;
    [SerializeField] private FighterTeam team = FighterTeam.Neutral;
    [SerializeField] private bool debugLogs = true;

    public FighterTeam Team => team;

    private void Awake()
    {
        if (healthSystem == null)
        {
            healthSystem = GetComponentInParent<HealthSystem>();
        }

        if (shieldController == null)
        {
            shieldController = GetComponent<EnemyShieldController>();

            if (shieldController == null)
            {
                shieldController = GetComponentInParent<EnemyShieldController>();
            }
        }
    }

    private void Reset()
    {
        healthSystem = GetComponentInParent<HealthSystem>();
        shieldController = GetComponent<EnemyShieldController>();

        if (shieldController == null)
        {
            shieldController = GetComponentInParent<EnemyShieldController>();
        }
    }

    public void SetHealthSystem(HealthSystem targetHealthSystem)
    {
        healthSystem = targetHealthSystem;
    }

    public void SetTeam(FighterTeam newTeam)
    {
        team = newTeam;
    }

    public bool CanReceiveDamageFrom(FighterTeam attackerTeam)
    {
        return attackerTeam == FighterTeam.Neutral || team == FighterTeam.Neutral || attackerTeam != team;
    }

    public bool ReceiveDamage(int damage)
    {
        return ReceiveDamage(damage, FighterTeam.Neutral);
    }

    public bool ReceiveDamage(int damage, FighterTeam attackerTeam)
    {
        if (!CanReceiveDamageFrom(attackerTeam))
        {
            return false;
        }

        if (shieldController != null && IsDamageBlockedByShield(attackerTeam))
        {
            if (debugLogs)
            {
                Debug.Log($"{nameof(DamageReceiver)} on {name}: Damage blocked by shield.");
            }

            return true;
        }

        if (healthSystem == null)
        {
            Debug.LogWarning($"{nameof(DamageReceiver)} on {name}: HealthSystem reference is missing.");
            return false;
        }

        healthSystem.TakeDamage(damage);
        if (debugLogs)
        {
            Debug.Log($"{nameof(DamageReceiver)} on {name}: received {damage} damage from {attackerTeam}.");
        }

        return true;
    }

    private bool IsDamageBlockedByShield(FighterTeam attackerTeam)
    {
        return shieldController != null && shieldController.ShouldBlockDamage(attackerTeam);
    }
}
