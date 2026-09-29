using UnityEngine;
[RequireComponent(typeof(Nightstand))]
public class NightstandInteractable : Interactable
{
    private Nightstand nightstand;
    private void OnEnable()
    {
        nightstand = GetComponent<Nightstand>();
    }
    public override void OnLookEnter()
    {
        base.OnLookEnter();
    }
    protected override void OnUse()
    {
        nightstand.SwitchState();
    }
    protected override bool DestroyAfterUse() => false;
}
