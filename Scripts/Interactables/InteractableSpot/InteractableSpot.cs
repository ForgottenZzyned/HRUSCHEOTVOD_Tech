using UnityEngine;
public abstract class InteractableSpot : MonoBehaviour
{
    public float maxWeight = int.MaxValue;
    public bool isInited = false;

    public void Init()
    {
        if (isInited) return;
        isInited = true;
        InteractablesManager.Instance.SerializeInteractableSpot(this);
    }

    public abstract void TrySpawnInteractable(Interactable prefab);
}
