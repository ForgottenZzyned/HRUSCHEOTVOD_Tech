using UnityEngine;
public class SpeedMultiplicator : Interactable
{
    public float speedMult;
    public float duration;
    protected override void OnUse()
    {
        PlayerMovement.Instance.SetSpeedMult(speedMult,duration);
        InteractablesManager.OnInteractableUsed?.Invoke($"You are bursting with energy [x{speedMult} Speed for {duration} seconds].");
    }
}
