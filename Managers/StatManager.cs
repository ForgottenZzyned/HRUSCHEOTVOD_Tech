using System;
using System.Collections.Generic;
using UnityEngine;
public enum StatType
{
    MaxHealth,
    Heal,
    HealthRegen,
    UnlimitedStamina,
    MaxStamina,
    StaminaRegen,
    EnergyRecover,
    MaxEnergy,
    EnergyRegen,
    StaminaRegenDebuff,
    HealthRegenDebuff,
    HealthHealDebuff
}
[System.Serializable]
public struct StatBonus
{
    public StatType stat;
    public float amount;
    public float weight;
}
public class StatManager : MonoBehaviour
{
    private static readonly Dictionary<StatType, Action<float>> actions = new();
    private void OnEnable()
    {
        Initialize();
    }
    public static void Initialize()
    {
        actions.Clear();

        RegisterHealthActions();
        RegisterStaminaActions();
        RegisterEnergyActions();
    }
    public static void Apply(StatBonus bonus)
    {
        if (actions.TryGetValue(bonus.stat, out var action))
            action(bonus.amount);
    }
    private static void RegisterHealthActions()
    {
        actions.Add(StatType.MaxHealth,
            value =>
            {
                HealthManager.Instance.AddMaxHealth(value);
                InteractablesManager.OnInteractableUsed?.Invoke($"Your heartbeat grows louder [+{value.ToString()} Max Health].");
            });
        actions.Add(StatType.Heal,
            value =>
            {
                HealthManager.Instance.AddHealth(value);
                InteractablesManager.OnInteractableUsed?.Invoke($"You feel better [+{value.ToString()} Health].");
            });
        actions.Add(StatType.HealthRegen,
            value =>
            {
                HealthManager.Instance.AddRegenSpeed(value);
                InteractablesManager.OnInteractableUsed?.Invoke($"You feel your body recovering [+{value.ToString()} Health Regen].");
            });
        actions.Add(StatType.HealthRegenDebuff,
            value =>
            {
                HealthManager.Instance.AddRegenSpeed(-value);
                InteractablesManager.OnInteractableUsed?.Invoke($"Your body feels cold [-{value.ToString()} Health Regen].");
            });
        actions.Add(StatType.HealthHealDebuff,
            value =>
            {
                GameOverController.Instance.killedString = "You died to medical reasons.";
                HealthManager.Instance.AddHealth(-value);
                InteractablesManager.OnInteractableUsed?.Invoke($"You feel bad, but you got used to it [-{value.ToString()} Health].");
            });
    }
    private static void RegisterEnergyActions()
    {
        actions.Add(StatType.EnergyRecover,
            value =>
            {
                EnergyManager.Instance.AddToCurrEnergy(value);
                InteractablesManager.OnInteractableUsed?.Invoke($"Of course, your flash light can be restored with a lamp, lucky you! [Flashlight energy set to Maximum].");
            });
        actions.Add(StatType.MaxEnergy,
            value =>
            {
                EnergyManager.Instance.AddMaxEnergy(value);
                InteractablesManager.OnInteractableUsed?.Invoke($"What a find! This battery is exactly what your flashlight needs [+{value.ToString()} Max Energy Capacity].");
            });
        actions.Add(StatType.EnergyRegen,
            value =>
            {
                new NullReferenceException("Don't have an action for that stat");
            });
    }
    private static void RegisterStaminaActions()
    {
        actions.Add(StatType.UnlimitedStamina,
            value =>
            {
                StaminaManager.Instance.AddUnlimitedStamina(value);
                InteractablesManager.OnInteractableUsed?.Invoke($"You no longer feel exhausted [Unlimited Stamina for {value.ToString()} sec].");
            });
        actions.Add(StatType.MaxStamina,
            value =>
            {
                StaminaManager.Instance.AddMaxStamina(value);
                InteractablesManager.OnInteractableUsed?.Invoke($"Your body feels lighter [+{value.ToString()} Max Stamina].");
            });
        actions.Add(StatType.StaminaRegen,
            value =>
            {
                StaminaManager.Instance.AddRegenSpeed(value);
                InteractablesManager.OnInteractableUsed?.Invoke($"You feel refreshed [+{value.ToString()} Stamina Regen].");
            });
        actions.Add(StatType.StaminaRegenDebuff,
            value =>
            {
                StaminaManager.Instance.AddRegenSpeed(-value);
                InteractablesManager.OnInteractableUsed?.Invoke($"You feel completely drained [-{value.ToString()} Stamina Regen].");
            });
    }
}
