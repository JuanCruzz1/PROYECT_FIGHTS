using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private Transform ownerRoot;

    private readonly HashSet<DamageReceiver> damagedTargets = new HashSet<DamageReceiver>();

    private void OnEnable()
    {
        damagedTargets.Clear();
    }

    public void ActivateHitbox()
    {
        damagedTargets.Clear();
        gameObject.SetActive(true);
    }

    public void DeactivateHitbox()
    {
        gameObject.SetActive(false);
    }

    public void SetDamage(int newDamage)
    {
        damage = Mathf.Max(0, newDamage);
    }

    public void SetOwnerRoot(Transform newOwnerRoot)
    {
        ownerRoot = newOwnerRoot;
    }

    private void OnTriggerEnter2D(Collider2D other)
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
            receiver.ReceiveDamage(damage);
        }
    }
}
