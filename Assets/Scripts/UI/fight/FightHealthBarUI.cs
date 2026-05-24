using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FightHealthBarUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image healthFillImage;

    [Header("Target")]
    [SerializeField] private FighterTeam targetTeam = FighterTeam.Player;

    private HealthSystem healthSystem;

    private void Start()
    {
        StartCoroutine(FindHealthSystem());
    }

    private IEnumerator FindHealthSystem()
    {
        yield return null;

        DamageReceiver[] receivers = FindObjectsByType<DamageReceiver>(FindObjectsSortMode.None);

        foreach (DamageReceiver receiver in receivers)
        {
            if (receiver == null) continue;
            if (receiver.Team != targetTeam) continue;

            HealthSystem foundHealth = receiver.GetComponentInParent<HealthSystem>();

            if (foundHealth == null) continue;

            healthSystem = foundHealth;
            healthSystem.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(healthSystem.CurrentHealth, healthSystem.MaxHealth);
            yield break;
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthFillImage == null || maxHealth <= 0) return;

        healthFillImage.fillAmount = (float)currentHealth / maxHealth;
    }
}