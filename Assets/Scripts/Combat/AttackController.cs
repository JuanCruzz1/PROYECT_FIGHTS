using UnityEngine;

public class AttackController : MonoBehaviour
{
    [Header("Owner")]
    [SerializeField] private Transform ownerRoot = null;
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private FighterTeam ownerTeam = FighterTeam.Player;

    [Header("Punch")]
    [SerializeField] private int punchDamage = 8;
    [SerializeField] private Vector2 punchOffset = new Vector2(0.75f, 0.15f);
    [SerializeField] private Vector2 punchSize = new Vector2(0.7f, 0.45f);
    [SerializeField] private float punchCooldown = 0.35f;

    [Header("Kick")]
    [SerializeField] private int kickDamage = 12;
    [SerializeField] private Vector2 kickOffset = new Vector2(0.95f, -0.1f);
    [SerializeField] private Vector2 kickSize = new Vector2(0.9f, 0.45f);
    [SerializeField] private float kickCooldown = 0.55f;

    [Header("Detection")]
    [SerializeField] private LayerMask targetLayers = 0;
    [SerializeField] private bool debugLogs = true;

    private float nextPunchTime;
    private float nextKickTime;

    private void Awake()
    {
        if (ownerRoot == null)
        {
            ownerRoot = transform;
        }

        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        WarnIfTargetLayerMissing();
    }

    public void Configure(FighterTeam team, CharacterData characterData)
    {
        ownerTeam = team;

        if (characterData == null)
        {
            return;
        }

        punchDamage = characterData.punchDamage;
        kickDamage = characterData.kickDamage;
    }

    public bool TryPunch()
    {
        if (Time.time < nextPunchTime)
        {
            return false;
        }

        nextPunchTime = Time.time + punchCooldown;
        HitArea("Punch", punchDamage, punchOffset, punchSize);
        return true;
    }

    public bool TryKick()
    {
        if (Time.time < nextKickTime)
        {
            return false;
        }

        nextKickTime = Time.time + kickCooldown;
        HitArea("Kick", kickDamage, kickOffset, kickSize);
        return true;
    }

    private void HitArea(string attackName, int damage, Vector2 offset, Vector2 size)
    {
        WarnIfTargetLayerMissing();

        Vector2 center = GetAreaCenter(offset);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, targetLayers);

        if (debugLogs)
        {
            Debug.Log($"{nameof(AttackController)} on {name}: {attackName} checked {hits.Length} colliders.");
        }

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            if (ownerRoot != null && hit.transform.root == ownerRoot)
            {
                continue;
            }

            DamageReceiver receiver = hit.GetComponent<DamageReceiver>();
            if (receiver == null)
            {
                receiver = hit.GetComponentInParent<DamageReceiver>();
            }

            if (receiver != null && receiver.ReceiveDamage(damage, ownerTeam))
            {
                if (debugLogs)
                {
                    Debug.Log($"{nameof(AttackController)} on {name}: {attackName} hit {receiver.name} for {damage}.");
                }

                return;
            }
        }
    }

    private Vector2 GetAreaCenter(Vector2 offset)
    {
        int direction = playerController != null ? playerController.FacingDirection : transform.localScale.x < 0f ? -1 : 1;
        Vector2 basePosition = ownerRoot != null ? ownerRoot.position : transform.position;
        return basePosition + new Vector2(offset.x * direction, offset.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetAreaCenter(punchOffset), punchSize);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(GetAreaCenter(kickOffset), kickSize);
    }

    private void WarnIfTargetLayerMissing()
    {
        if (targetLayers.value == 0)
        {
            Debug.LogWarning($"{nameof(AttackController)} on {name}: targetLayers is not configured. Assign the opponent hurtbox layer in Unity.");
        }
    }
}
