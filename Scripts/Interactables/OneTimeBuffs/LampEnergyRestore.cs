using UnityEngine;

public class LampEnergyRestore : Interactable
{
    public float amountToAdd = 999f;
    protected override void OnUse()
    {
        EnergyManager.Instance.AddToCurrEnergy(amountToAdd);
    }
}
