using UnityEngine;

public class EnemyShieldController : MonoBehaviour
{
    [Header("Shield")]
    [SerializeField] private GameObject shieldPrefab = null;
    [SerializeField] private Transform shieldSpawnPoint = null;
    [SerializeField] private float shieldDuration = 1.25f;
    [SerializeField] private float shieldCooldown = 3f;
    [SerializeField] private Vector2 shieldOffset = new Vector2(0.75f, 0f);

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private GameObject activeShield;
    private float activeUntilTime;
    private float nextAvailableTime;
    private int facingDirection = -1;

    public bool IsShieldActive => activeShield != null && activeShield.activeSelf && Time.time < activeUntilTime;
    public bool CanActivate => Time.time >= nextAvailableTime && !IsShieldActive;

    private void Update()
    {
        if (IsShieldActive)
        {
            UpdateShieldTransform();
            return;
        }

        if (activeShield != null && activeShield.activeSelf)
        {
            activeShield.SetActive(false);
        }
    }

    public bool TryActivate(int newFacingDirection)
    {
        if (!CanActivate)
        {
            return false;
        }

        if (shieldPrefab == null)
        {
            Debug.LogWarning($"{nameof(EnemyShieldController)} on {name}: shieldPrefab is missing.");
            return false;
        }

        facingDirection = newFacingDirection >= 0 ? 1 : -1;

        if (activeShield == null)
        {
            activeShield = Instantiate(shieldPrefab);
        }

        activeShield.SetActive(true);
        activeUntilTime = Time.time + Mathf.Max(0.05f, shieldDuration);
        nextAvailableTime = Time.time + Mathf.Max(0.05f, shieldCooldown);
        UpdateShieldTransform();

        if (debugLogs)
        {
            Debug.Log($"{nameof(EnemyShieldController)} on {name}: Shield active.");
        }

        return true;
    }

    public bool ShouldBlockDamage(FighterTeam attackerTeam)
    {
        if (!IsShieldActive)
        {
            return false;
        }

        // TODO: Directional shield blocking. First version blocks any enemy-facing
        // incoming damage while active, regardless of hit direction.
        return attackerTeam != FighterTeam.Neutral;
    }

    private void UpdateShieldTransform()
    {
        if (activeShield == null)
        {
            return;
        }

        Vector3 basePosition = shieldSpawnPoint != null ? shieldSpawnPoint.position : transform.position;
        Vector3 offset = new Vector3(shieldOffset.x * facingDirection, shieldOffset.y, 0f);
        activeShield.transform.position = basePosition + offset;

        Vector3 scale = activeShield.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDirection;
        activeShield.transform.localScale = scale;
    }
}
