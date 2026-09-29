using UnityEngine;
[RequireComponent(typeof(Cabinet))]
public class CabinetInteractable : Interactable
{
    private Cabinet cabinet;
    private void OnEnable()
    {
        cabinet = GetComponent<Cabinet>();
    }
    protected override void OnUse()
    {
        cabinet.SwitchState();
    }
    protected override bool DestroyAfterUse() => false;
}
