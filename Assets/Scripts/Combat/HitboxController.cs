using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private Transform ownerRoot;
    [SerializeField] private FighterTeam ownerTeam = FighterTeam.Neutral;
    [SerializeField] private Vector2 localOffset = new Vector2(0.8f, 0f);
    [SerializeField] private Vector2 boxSize = new Vector2(0.8f, 0.5f);
    [SerializeField] private LayerMask targetLayers = 0;
    [SerializeField] private bool useManualOverlap = true;
    [SerializeField] private bool debugLogs = true;

    private readonly HashSet<DamageReceiver> damagedTargets = new HashSet<DamageReceiver>();
    private bool active;

    private void Awake()
    {
        WarnIfTargetLayerMissing();
    }

    private void OnEnable()
    {
        damagedTargets.Clear();
    }

    public void ActivateHitbox()
    {
        damagedTargets.Clear();
        active = true;

        if (!useManualOverlap)
        {
            gameObject.SetActive(true);
        }
    }

    public void DeactivateHitbox()
    {
        active = false;

        if (!useManualOverlap)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = Mathf.Max(0, newDamage);
    }

    public void SetOwnerRoot(Transform newOwnerRoot)
    {
        ownerRoot = newOwnerRoot;
    }

    public void SetOwnerTeam(FighterTeam newOwnerTeam)
    {
        ownerTeam = newOwnerTeam;
    }

    public void Configure(int newDamage, Transform newOwnerRoot, FighterTeam newOwnerTeam)
    {
        SetDamage(newDamage);
        SetOwnerRoot(newOwnerRoot);
        SetOwnerTeam(newOwnerTeam);
    }

    public void HitOnce()
    {
        damagedTargets.Clear();
        TryHitOverlaps();
    }

    private void Update()
    {
        if (active && useManualOverlap)
        {
            TryHitOverlaps();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (useManualOverlap)
        {
            return;
        }

        TryDamageCollider(other);
    }

    private void TryHitOverlaps()
    {
        WarnIfTargetLayerMissing();

        Collider2D[] hits = Physics2D.OverlapBoxAll(GetWorldCenter(), boxSize, 0f, targetLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            TryDamageCollider(hits[i]);
        }
    }

    private void TryDamageCollider(Collider2D other)
    {
        if (ownerRoot != null && other.transform.root == ownerRoot)
        {
            return;
        }

        DamageReceiver receiver = other.GetComponent<DamageReceiver>();

        if (receiver == null)
        {
            receiver = other.GetComponentInParent<DamageReceiver>();
        }

        if (receiver != null && damagedTargets.Add(receiver))
        {
            if (receiver.ReceiveDamage(damage, ownerTeam) && debugLogs)
            {
                Debug.Log($"{nameof(HitboxController)} on {name}: hit {receiver.name} for {damage}.");
            }
        }
    }

    private Vector2 GetWorldCenter()
    {
        int direction = 1;

        if (ownerRoot != null)
        {
            PlayerController controller = ownerRoot.GetComponent<PlayerController>();
            direction = controller != null ? controller.FacingDirection : ownerRoot.localScale.x < 0f ? -1 : 1;
        }

        Vector2 basePosition = ownerRoot != null ? ownerRoot.position : transform.position;
        return basePosition + new Vector2(localOffset.x * direction, localOffset.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GetWorldCenter(), boxSize);
    }

    private void WarnIfTargetLayerMissing()
    {
        if (targetLayers.value == 0)
        {
            Debug.LogWarning($"{nameof(HitboxController)} on {name}: targetLayers is not configured. Assign the opponent hurtbox layer in Unity.");
        }
    }
}
