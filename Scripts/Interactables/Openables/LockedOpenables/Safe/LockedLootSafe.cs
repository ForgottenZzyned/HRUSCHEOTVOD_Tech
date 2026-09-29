using UnityEngine;
[RequireComponent(typeof(Safe))]
public class LockedLootSafe : Interactable
{
    private Safe safe;
    private void OnEnable()
    {
        safe = GetComponent<Safe>();
    }
    public override void OnLookEnter()
    {
        if (safe.isLocked)
        {
            descText = "[LMB to try Unlock]";
        }
        else
        {
            descText = "[LMB to Open]";
        }
        base.OnLookEnter();
    }
    protected override void OnUse()
    {
        if (safe.isLocked)
        {
            Slot keySlot = SlotManager.Instance.IsContainingInteractable("Old Key");
            if (keySlot != null)
            {
                keySlot.SlotUse();
                safe.isLocked = false;
                safe.SwitchState();
                OnLookExit();
                return;
            }
            return;
        }
        safe.SwitchState();
    }
    protected override bool DestroyAfterUse() => false;
}
