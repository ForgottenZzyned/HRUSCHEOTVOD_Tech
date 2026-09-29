using UnityEngine;
[RequireComponent(typeof(Closet))]
public class LockedLootCloset : Interactable
{
    private Closet closet;
    private void OnEnable()
    {
        closet = GetComponent<Closet>();
    }
    public override void OnLookEnter()
    {
        if (closet.isLocked)
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
        if (closet.isLocked)
        {
            Slot keySlot = SlotManager.Instance.IsContainingInteractable("Old Key");
            if (keySlot != null)
            {
                keySlot.SlotUse();
                closet.isLocked = false;
                closet.SwitchState();
                OnLookExit();
                return;
            }
            return;
        }
        closet.SwitchState();
    }
    protected override bool DestroyAfterUse() => false;
}
