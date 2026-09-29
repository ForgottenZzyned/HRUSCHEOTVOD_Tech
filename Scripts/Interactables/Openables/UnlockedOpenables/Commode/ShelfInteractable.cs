using UnityEngine;

[RequireComponent(typeof(Shelf))]
public class ShelfInteractable : Interactable
{
    private Shelf shelf;
    private void OnEnable()
    {
        shelf = GetComponent<Shelf>();
    }
    protected override void OnUse()
    {
        shelf.SwitchState();
    }
    protected override bool DestroyAfterUse() => false;
}
