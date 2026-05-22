using UnityEngine;

public class ManualParabolicProjectile2D : MonoBehaviour
{
    private Transform owner;
    private FighterTeam ownerTeam;
    private int damage;
    private Vector2 origin;
    private float direction;
    private float initialSpeed;
    private float angleRadians;
    private float gravity;
    private float hitRadius;
    private float maxLifetime;
    private LayerMask targetLayers;
    private float elapsed;
    private bool launched;

    public void Launch(
        Transform newOwner,
        FighterTeam newOwnerTeam,
        int newDamage,
        float newDirection,
        float newInitialSpeed,
        float newAngleDegrees,
        float newGravity,
        float newHitRadius,
        float newMaxLifetime,
        LayerMask newTargetLayers
    )
    {
        owner = newOwner;
        ownerTeam = newOwnerTeam;
        damage = newDamage;
        origin = transform.position;
        direction = Mathf.Sign(newDirection);
        initialSpeed = Mathf.Max(0f, newInitialSpeed);
        angleRadians = newAngleDegrees * Mathf.Deg2Rad;
        gravity = Mathf.Max(0f, newGravity);
        hitRadius = Mathf.Max(0.01f, newHitRadius);
        maxLifetime = Mathf.Max(0.1f, newMaxLifetime);
        targetLayers = newTargetLayers;
        elapsed = 0f;
        launched = true;
    }

    private void Update()
    {
        if (!launched)
        {
            return;
        }

        elapsed += Time.deltaTime;

        // x(t) = x0 + v0*cos(theta)*t
        // y(t) = y0 + v0*sin(theta)*t - 0.5*g*t^2
        float x = origin.x + direction * initialSpeed * Mathf.Cos(angleRadians) * elapsed;
        float y = origin.y + initialSpeed * Mathf.Sin(angleRadians) * elapsed - 0.5f * gravity * elapsed * elapsed;
        transform.position = new Vector2(x, y);

        if (TryHit() || elapsed >= maxLifetime)
        {
            Destroy(gameObject);
        }
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

            if (receiver != null && receiver.ReceiveDamage(damage, ownerTeam))
            {
                Debug.Log($"{nameof(ManualParabolicProjectile2D)} on {name}: hit {receiver.name} for {damage}.");
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
