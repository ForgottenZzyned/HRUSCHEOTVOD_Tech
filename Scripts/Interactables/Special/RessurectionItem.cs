using UnityEngine;
public class RessurectionItem : Interactable
{
    protected override void OnUse()
    {
        InteractablesManager.OnInteractableUsed?.Invoke($"You've been ressurected.");
    }
}
