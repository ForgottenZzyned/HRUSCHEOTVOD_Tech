using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Slot
{
    public TMP_Text interactSlotNameText;
    public TMP_Text interactSlotUseText;
    public bool isSlotFree = true;
    public Interactable slotInteract = null;
    public KeyCode slotUse = KeyCode.E;
    public void SlotUse()
    {
        slotInteract.Use();
        isSlotFree = true;
        slotInteract = null;
        interactSlotNameText.text = "";
        interactSlotUseText.text = "";
        SlotManager.Instance.slotCount--;
        SlotManager.Instance.RefreshSlots(0);
    }
    public void SetInteractable(Interactable instance)
    {
        isSlotFree = false;
        slotInteract = instance;
        slotInteract.transform.parent = null;
        interactSlotNameText.text = slotInteract.nameText;
        if (slotInteract.isGotEffect) interactSlotUseText.text = $"[{slotUse}] to Use";
        else interactSlotUseText.text = "";
    }
}
[System.Serializable]
public class RessurectionSlot
{
    public TMP_Text interactSlotNameText;
    public bool isSlotFree = true;
    public Interactable slotInteract = null;
    public void SlotUse()
    {
        slotInteract.Use();
        isSlotFree = true;
        slotInteract = null;
        interactSlotNameText.text = "";
    }
    public void SetInteractable(Interactable instance)
    {
        isSlotFree = false;
        slotInteract = instance;
        slotInteract.transform.parent = null;
        interactSlotNameText.text = slotInteract.nameText;
    }
}
public class SlotManager : Singleton<SlotManager>
{
    public List<Slot> slots = new();
    public List<RessurectionSlot> resurSlot = new();
    public TMP_Text countText;
    private int _slotCount = 0;
    public int slotCount
    {
        get => _slotCount;
        set
        {
            _slotCount = value;
            countText.text = $"{_slotCount}/{slots.Count}";
        }
    }
    private void Update()
    {
        CheckInput();
    }
    private void CheckInput()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (Input.GetKeyDown(slot.slotUse) 
                && slot.slotInteract != null 
                && slot.slotInteract.isGotEffect)
            {
                slot.SlotUse();
            }
        }
    }
    public void RefreshSlots(int currNum)
    {
        for (int i = currNum; i < slots.Count; i++)
        {
            var slot = slots[i];
            Slot nextSlot = null;
            if (slot.isSlotFree && (i+1) < slots.Count) nextSlot = slots[i + 1];
            if (nextSlot != null && nextSlot.slotInteract != null) 
            {
                slots[i].SetInteractable(nextSlot.slotInteract);
                ClearSlot(nextSlot);
            }
        }
    }
    public void SetInteractableToSlot(Interactable instance)
    {
        if(instance is RessurectionItem)
        {
            if (resurSlot[0].isSlotFree)
            {
                resurSlot[0].SetInteractable(instance);
                return;
            }
            resurSlot[0].slotInteract.currSpot = instance.currSpot;
            resurSlot[0].slotInteract.currSpot.SetInteractable(resurSlot[0].slotInteract);
            resurSlot[0].SetInteractable(instance);
            return;
        }
        foreach(var slot in slots)
        {
            if (!slot.isSlotFree) continue;
            slot.SetInteractable(instance);
            slotCount++;
            return;
        }
        if (!slots[0].isSlotFree)
        {
            slots[0].slotInteract.currSpot = instance.currSpot;
            slots[0].slotInteract.currSpot.SetInteractable(slots[0].slotInteract);
            slots[0].SetInteractable(instance);
        }
    }
    public void ClearSlot(Slot slot)
    {
        slot.isSlotFree = true;
        slot.slotInteract = null;
        slot.interactSlotNameText.text = "";
        slot.interactSlotUseText.text = "";
    }
    public Slot IsContainingInteractable(string name)
    {
        foreach(var slot in slots)
        {
            if (slot.slotInteract == null) continue;
            if(slot.slotInteract.nameText == name) return slot;
        }
        return null;
    }
    public RessurectionSlot IsContainingRessurection()
    {
        if (!resurSlot[0].isSlotFree) return resurSlot[0];
        return null;
    }
}
