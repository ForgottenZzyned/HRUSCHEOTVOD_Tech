using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class InteractablesManager : Singleton<InteractablesManager>
{
    [SerializeField] private TMP_Text interactNameText;
    [SerializeField] private TMP_Text interactUseText;
    [SerializeField] private TMP_Text interactEffectText;
    [SerializeField] private TMP_Text interactStoreText;
    public static Action<string> OnInteractableUsed;
    [Header("Interactable Spawn Settings")]
    public List<Interactable> basicInteractables = new();
    [Range(0,1)]
    public float spawnChance = 1f;
    [Header("Interactable Slot Settings")]
    [SerializeField] private TMP_Text interactSlotNameText;
    [SerializeField] private TMP_Text interactSlotUseText;
    public bool isSlotFree = true;
    public Interactable slotInteract = null;

    private void OnEnable()
    {
        OnInteractableUsed += SetEffectText;
    }
    private void OnDisable()
    {
        OnInteractableUsed += SetEffectText;
    }
    public void SerializeInteractableSpot(InteractableSpot spot)
    {
        //Overall spawnChance
        if(UnityEngine.Random.value <= spawnChance)
        {
            spot.TrySpawnInteractable(GetRandomInteractable(spot.maxWeight, basicInteractables));
        }
    }
    public Interactable GetRandomInteractable(float maxWeight, List<Interactable> pool)
    {
        float totalWeight = 0f;
        Interactable lastInteract;
        List<Interactable> temp = new List<Interactable>();
        foreach (var interact in pool)
            if(interact.weight < maxWeight)temp.Add(interact);
        foreach (var interact in temp)
            totalWeight += interact.weight;
        float randomPoint = UnityEngine.Random.Range(0f, totalWeight);

        foreach (var interact in temp)
        {
            randomPoint -= interact.weight;

            if (randomPoint <= 0f)
            {
                lastInteract = interact;
                return interact;
            }

        }
        if(temp != null) return temp[temp.Count - 1];
        return pool[pool.Count - 1];
    }
    public void SetInteractText(Interactable instance, bool visibility)
    {
        if(visibility)
        {
            interactNameText.text = instance.nameText;
            interactUseText.text = instance.descText;
            if (instance.canBeStored)
                interactStoreText.text = "[R to Store]";
        }
        else
        {
            interactNameText.text = "";
            interactUseText.text = "";
            interactStoreText.text = "";
        }
    }
    public void SetEffectText(string text)
    {
        interactEffectText.DOKill();
        interactEffectText.DOFade(1,0f);
        interactEffectText.DOTypeText(text, 2f)
            .OnComplete
            (
                ()=> interactEffectText.DOFade(0, 4f).SetEase(Ease.InQuad)
            );
    }
}
