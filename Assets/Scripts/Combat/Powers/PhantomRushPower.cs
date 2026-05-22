using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomRushPower : SpecialPowerBase
{
    [SerializeField] private float accelerationDuration = 0.12f;
    [SerializeField] private float impactDuration = 0.18f;
    [SerializeField] private float decelerationDuration = 0.18f;
    [SerializeField] private float peakSpeed = 16f;
    [SerializeField] private Vector2 impactOffset = new Vector2(0.85f, 0f);
    [SerializeField] private Vector2 impactSize = new Vector2(1.1f, 0.8f);
    [SerializeField] private LayerMask targetLayers = 0;
    [SerializeField] private bool debugLogs = true;

    private readonly HashSet<DamageReceiver> damagedTargets = new HashSet<DamageReceiver>();
    private bool running;

    public override bool TryActivate(SpecialPowerContext context)
    {
        WarnIfTargetLayerMissing();

        if (running || context.PlayerController == null)
        {
            return false;
        }

        StartCoroutine(RushRoutine(context));
        return true;
    }

    private IEnumerator RushRoutine(SpecialPowerContext context)
    {
        running = true;
        damagedTargets.Clear();

        float elapsed = 0f;
        float totalDuration = accelerationDuration + impactDuration + decelerationDuration;

        if (debugLogs)
        {
            Debug.Log($"{nameof(PhantomRushPower)} on {name}: PhantomRush started.");
        }

        while (elapsed < totalDuration)
        {
            float deltaTime = Time.deltaTime;
            elapsed += deltaTime;

            float speed = EvaluateSpeed(elapsed);
            Vector2 direction = Vector2.right * context.FacingDirection;
            context.PlayerController.MoveBy(direction * speed * deltaTime);

            if (elapsed >= accelerationDuration && elapsed <= accelerationDuration + impactDuration)
            {
                TryImpact(context);
            }

            yield return null;
        }

        running = false;
    }

    private float EvaluateSpeed(float time)
    {
        if (time <= accelerationDuration)
        {
            return Mathf.Lerp(0f, peakSpeed, time / Mathf.Max(0.001f, accelerationDuration));
        }

        if (time <= accelerationDuration + impactDuration)
        {
            return peakSpeed;
        }

        float decelTime = time - accelerationDuration - impactDuration;
        return Mathf.Lerp(peakSpeed, 0f, decelTime / Mathf.Max(0.001f, decelerationDuration));
    }

    private void TryImpact(SpecialPowerContext context)
    {
        Vector2 center = (Vector2)context.Owner.position + new Vector2(impactOffset.x * context.FacingDirection, impactOffset.y);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, impactSize, 0f, targetLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            if (hit.transform.root == context.Owner)
            {
                continue;
            }

            DamageReceiver receiver = hit.GetComponent<DamageReceiver>();
            if (receiver == null)
            {
                receiver = hit.GetComponentInParent<DamageReceiver>();
            }

            if (receiver != null && damagedTargets.Add(receiver))
            {
                receiver.ReceiveDamage(context.Damage, context.OwnerTeam);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + impactOffset, impactSize);
    }

    private void WarnIfTargetLayerMissing()
    {
        if (targetLayers.value == 0)
        {
            Debug.LogWarning($"{nameof(PhantomRushPower)} on {name}: targetLayers is not configured. Assign the opponent hurtbox layer in Unity.");
        }
    }
}
