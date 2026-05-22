using UnityEngine;

public class ChainWhipPower : SpecialPowerBase
{
    [SerializeField] private GameObject chainPrefab = null;
    [SerializeField] private float extensionSpeed = 12f;
    [SerializeField] private float returnSpeed = 16f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float returnStopDistance = 0.25f;
    [SerializeField] private float hitRadius = 0.25f;
    [SerializeField] private LayerMask targetLayers = 0;
    [SerializeField] private bool canHitMultipleTimes = false;
    [SerializeField] private bool debugLogs = true;

    public override bool TryActivate(SpecialPowerContext context)
    {
        WarnIfTargetLayerMissing();

        if (chainPrefab == null)
        {
            Debug.LogWarning($"{nameof(ChainWhipPower)} on {name}: chainPrefab is missing.");
            return false;
        }

        GameObject chain = Instantiate(chainPrefab, context.SpawnPoint.position, Quaternion.identity);
        ManualLinearProjectile2D simulation = chain.GetComponent<ManualLinearProjectile2D>();

        if (simulation == null)
        {
            simulation = chain.AddComponent<ManualLinearProjectile2D>();
        }

        simulation.Launch(
            context.Owner,
            context.OwnerTeam,
            context.Damage,
            Vector2.right * context.FacingDirection,
            extensionSpeed,
            returnSpeed,
            maxDistance,
            returnStopDistance,
            hitRadius,
            targetLayers,
            canHitMultipleTimes
        );

        if (debugLogs)
        {
            Debug.Log($"{nameof(ChainWhipPower)} on {name}: ChainWhip launched.");
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * maxDistance);
    }

    private void WarnIfTargetLayerMissing()
    {
        if (targetLayers.value == 0)
        {
            Debug.LogWarning($"{nameof(ChainWhipPower)} on {name}: targetLayers is not configured. Assign the opponent hurtbox layer in Unity.");
        }
    }
}
