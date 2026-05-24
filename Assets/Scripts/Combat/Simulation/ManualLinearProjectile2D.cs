using UnityEngine;
using System.Collections.Generic;

public class ManualLinearProjectile2D : MonoBehaviour
{
    [Header("Optional Chain Visuals")]
    [SerializeField] private Transform chainStart = null;
    [SerializeField] private Transform chainBody = null;
    [SerializeField] private Transform chainTip = null;
    [SerializeField] private SpriteRenderer chainStartRenderer = null;
    [SerializeField] private SpriteRenderer chainBodyRenderer = null;
    [SerializeField] private SpriteRenderer chainTipRenderer = null;
    [SerializeField] private float chainBodyBaseLength = 1f;
    [SerializeField] private float chainBodyMinLength = 0.01f;
    [SerializeField] private float chainVisualOverlap = 0.25f;
    [SerializeField] private bool debugChainVisuals = false;

    private enum ChainState
    {
        Extending,
        Returning
    }

    private Transform owner;
    private Transform originTransform;
    private FighterTeam ownerTeam;
    private int damage;
    private Vector2 origin;
    private Vector2 direction;
    private Vector2 maxPosition;
    private float extensionSpeed;
    private float returnSpeed;
    private float maxDistance;
    private float returnStopDistance;
    private float hitRadius;
    private LayerMask targetLayers;
    private bool canHitMultipleTimes;
    private readonly HashSet<DamageReceiver> damagedTargets = new HashSet<DamageReceiver>();
    private ChainState state;
    private float elapsed;
    private bool launched;
    private Vector3 chainBodyInitialScale = Vector3.one;
    private float nextVisualDebugLogTime;

    private void Awake()
    {
        ResolveVisualReferences();
        CacheVisualDefaults();
    }

    public void Launch(
        Transform newOwner,
        Transform newOriginTransform,
        FighterTeam newOwnerTeam,
        int newDamage,
        Vector2 newDirection,
        float newExtensionSpeed,
        float newReturnSpeed,
        float newMaxDistance,
        float newReturnStopDistance,
        float newHitRadius,
        LayerMask newTargetLayers,
        bool newCanHitMultipleTimes
    )
    {
        owner = newOwner;
        originTransform = newOriginTransform != null ? newOriginTransform : newOwner;
        ownerTeam = newOwnerTeam;
        damage = newDamage;
        origin = originTransform != null ? originTransform.position : transform.position;
        direction = newDirection.sqrMagnitude > 0f ? newDirection.normalized : Vector2.right;
        extensionSpeed = Mathf.Max(0.01f, newExtensionSpeed);
        returnSpeed = Mathf.Max(0.01f, newReturnSpeed);
        maxDistance = Mathf.Max(0.1f, newMaxDistance);
        returnStopDistance = Mathf.Max(0.01f, newReturnStopDistance);
        hitRadius = Mathf.Max(0.01f, newHitRadius);
        targetLayers = newTargetLayers;
        canHitMultipleTimes = newCanHitMultipleTimes;
        damagedTargets.Clear();
        state = ChainState.Extending;
        elapsed = 0f;
        maxPosition = origin + direction * maxDistance;
        launched = true;
        UpdateChainVisuals();
    }

    private void Update()
    {
        if (!launched)
        {
            return;
        }

        if (state == ChainState.Extending)
        {
            UpdateExtension(Time.deltaTime);
        }
        else
        {
            UpdateReturn(Time.deltaTime);
        }

        TryHit();
        UpdateChainVisuals();
    }

    private void UpdateExtension(float deltaTime)
    {
        elapsed += deltaTime;

        // Extension formula:
        // position(t) = startPosition + direction * extensionSpeed * t
        float distance = Mathf.Min(extensionSpeed * elapsed, maxDistance);
        transform.position = origin + direction * distance;

        if (distance >= maxDistance)
        {
            maxPosition = transform.position;
            state = ChainState.Returning;
        }
    }

    private void UpdateReturn(float deltaTime)
    {
        Vector2 targetPosition = GetCurrentOriginPosition();
        Vector2 currentPosition = transform.position;
        Vector2 toOwner = targetPosition - currentPosition;

        if (toOwner.magnitude <= returnStopDistance)
        {
            Destroy(gameObject);
            return;
        }

        // Return is still manual: each frame we compute a step toward the current
        // owner position. No Rigidbody2D forces or automatic physics drive this.
        Vector2 returnDirection = toOwner.normalized;
        transform.position = currentPosition + returnDirection * returnSpeed * deltaTime;
    }

    private void ResolveVisualReferences()
    {
        if (chainStart == null)
        {
            chainStart = transform.Find("ChainStart");
        }

        if (chainBody == null)
        {
            chainBody = transform.Find("ChainBody");
        }

        if (chainTip == null)
        {
            chainTip = transform.Find("ChainTip");
        }

        if (chainStartRenderer == null && chainStart != null)
        {
            chainStartRenderer = chainStart.GetComponent<SpriteRenderer>();
        }

        if (chainBodyRenderer == null && chainBody != null)
        {
            chainBodyRenderer = chainBody.GetComponent<SpriteRenderer>();
        }

        if (chainTipRenderer == null && chainTip != null)
        {
            chainTipRenderer = chainTip.GetComponent<SpriteRenderer>();
        }
    }

    private void CacheVisualDefaults()
    {
        if (chainBody != null)
        {
            chainBodyInitialScale = chainBody.localScale;
        }

        if (chainBodyBaseLength <= 0f && chainBodyRenderer != null && chainBodyRenderer.sprite != null)
        {
            chainBodyBaseLength = Mathf.Max(0.01f, chainBodyRenderer.sprite.bounds.size.x);
        }
        else
        {
            chainBodyBaseLength = Mathf.Max(0.01f, chainBodyBaseLength);
        }
    }

    private void UpdateChainVisuals()
    {
        if (chainStart == null && chainBody == null && chainTip == null)
        {
            return;
        }

        Vector2 visualStart = GetCurrentOriginPosition();
        Vector2 visualTip = transform.position;
        float visualDirection = direction.x >= 0f ? 1f : -1f;
        float angle = visualDirection >= 0f ? 0f : 180f;
        Quaternion visualRotation = Quaternion.Euler(0f, 0f, angle);

        if (chainStart != null)
        {
            chainStart.position = visualStart;
            chainStart.rotation = visualRotation;
        }

        if (chainTip != null)
        {
            chainTip.position = visualTip;
            chainTip.rotation = visualRotation;
        }

        if (chainBody != null)
        {
            Vector2 bodyStart = chainStart != null ? (Vector2)chainStart.position : visualStart;
            Vector2 bodyTip = chainTip != null ? (Vector2)chainTip.position : visualTip;
            float distance = Vector2.Distance(bodyStart, bodyTip);
            float visualLength = distance + Mathf.Max(0f, chainVisualOverlap);
            float bodyBaseLength = Mathf.Max(0.01f, chainBodyBaseLength);
            float newScaleX = Mathf.Abs(chainBodyInitialScale.x) * (visualLength / bodyBaseLength);

            // Visual length formula:
            // visualLength = distance(ChainStart, ChainTip) + chainVisualOverlap.
            // Only local X changes, so ChainBody covers the gap while ChainStart and
            // ChainTip keep their mathematically calculated positions.
            chainBody.position = (bodyStart + bodyTip) * 0.5f;
            chainBody.rotation = visualRotation;
            chainBody.localScale = new Vector3(
                Mathf.Max(chainBodyMinLength, newScaleX),
                chainBodyInitialScale.y,
                chainBodyInitialScale.z
            );

            if (debugChainVisuals && Time.time >= nextVisualDebugLogTime)
            {
                nextVisualDebugLogTime = Time.time + 0.5f;
                Debug.Log($"{nameof(ManualLinearProjectile2D)} on {name}: ChainVisual distance={distance:F2}, visualLength={visualLength:F2}, newScaleX={newScaleX:F2}.");
            }
        }
    }

    private Vector2 GetCurrentOriginPosition()
    {
        if (originTransform != null)
        {
            return originTransform.position;
        }

        if (owner != null)
        {
            return owner.position;
        }

        return origin;
    }

    private bool CanDamage(DamageReceiver receiver)
    {
        if (canHitMultipleTimes)
        {
            return true;
        }

        return damagedTargets.Add(receiver);
    }

    private bool TryHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, targetLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            if (owner != null && hit.transform.root == owner)
            {
                continue;
            }

            DamageReceiver receiver = hit.GetComponent<DamageReceiver>();
            if (receiver == null)
            {
                receiver = hit.GetComponentInParent<DamageReceiver>();
            }

            if (receiver != null && CanDamage(receiver) && receiver.ReceiveDamage(damage, ownerTeam))
            {
                Debug.Log($"{nameof(ManualLinearProjectile2D)} on {name}: hit {receiver.name} for {damage}.");
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
        Gizmos.DrawLine(origin, origin + direction * maxDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(maxPosition, hitRadius);
    }
}
