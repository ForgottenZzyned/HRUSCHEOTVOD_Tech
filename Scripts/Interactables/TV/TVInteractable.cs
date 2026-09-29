using UnityEngine;
[RequireComponent(typeof(TVScript))]
public class TVInteractable : Interactable
{
    protected override void OnUse()
    {
        TVScript tvS = GetComponent<TVScript>();
        if (!tvS.isActive)tvS.ActivateNoise();
        else tvS.Shutdown();
    }
    protected override bool DestroyAfterUse() => false;
}
