using UnityEngine;

public class CheersBoomPower : SpecialPowerBase
{
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private float initialSpeed = 8f;
    [SerializeField] private float launchAngleDegrees = 45f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float hitRadius = 0.25f;
    [SerializeField] private float maxLifetime = 2.5f;
    [SerializeField] private LayerMask targetLayers = 0;
    [SerializeField] private bool debugLogs = true;

    public override bool TryActivate(SpecialPowerContext context)
    {
        WarnIfTargetLayerMissing();

        if (projectilePrefab == null)
        {
            Debug.LogWarning($"{nameof(CheersBoomPower)} on {name}: projectilePrefab is missing.");
            return false;
        }

        GameObject projectile = Instantiate(projectilePrefab, context.SpawnPoint.position, Quaternion.identity);
        ManualParabolicProjectile2D simulation = projectile.GetComponent<ManualParabolicProjectile2D>();

        if (simulation == null)
        {
            simulation = projectile.AddComponent<ManualParabolicProjectile2D>();
        }

        simulation.Launch(
            context.Owner,
            context.OwnerTeam,
            context.Damage,
            context.FacingDirection,
            initialSpeed,
            launchAngleDegrees,
            gravity,
            hitRadius,
            maxLifetime,
            targetLayers
        );

        if (debugLogs)
        {
            Debug.Log($"{nameof(CheersBoomPower)} on {name}: CheersBoom launched.");
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }

    private void WarnIfTargetLayerMissing()
    {
        if (targetLayers.value == 0)
        {
            Debug.LogWarning($"{nameof(CheersBoomPower)} on {name}: targetLayers is not configured. Assign the opponent hurtbox layer in Unity.");
        }
    }
}
