using UnityEngine;
public interface IInteractable
{
    void OnLookEnter();
    void OnLookExit();
}
public enum InputType
{
    Key,
    Mouse
}
[System.Serializable]
public struct InputKey
{
    public InputType type;
    public KeyCode key;
    public int mouseButton;
}
public abstract class Interactable : MonoBehaviour, IInteractable
{
    [HideInInspector] public BasicInteractableSpot currSpot;
    public float weight;
    public InputKey keyToUse;
    public string nameText = "???";
    public string descText = "???";
    public SFXBank sfxOnUse;
    public SFXBank sfxOnPickUp;
    public bool isGotEffect = false;
    public bool canBeStored = false;
    private bool isLookedOn = false;
    protected virtual void Update()
    {
        if (!isLookedOn) return;
        if (PauseMenu.IsPaused) return;
        if (IsPressed(keyToUse))
            Use(); 
    }

    public void SetToSlot()
    {
        transform.position = Vector3.zero + (Vector3.up * 2);
        SlotManager.Instance.SetInteractableToSlot(this);
        SFXManager.Instance.Play(sfxOnPickUp,Vector3.zero,0,int.MaxValue);
    }
    public virtual void Use()
    {
        if (DestroyAfterUse()) OnLookExit();
        OnUse();
        if(sfxOnUse != null) SFXManager.Instance.Play(sfxOnUse, Vector3.zero, 0, int.MaxValue);
        if (DestroyAfterUse())
        {
            PlayerLookDetector.Instance.ClearCurrTarget();
            Destroy(gameObject, 0.25f);
            gameObject.SetActive(false);
        }
    }
    protected abstract void OnUse();
    protected virtual bool DestroyAfterUse() => true;
    public virtual void OnLookEnter()
    {
        isLookedOn = true;
        InteractablesManager.Instance.SetInteractText(this,true);
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Interactable"));
    }
    public virtual void OnLookExit()
    {
        isLookedOn = false;
        InteractablesManager.Instance.SetInteractText(this,false);
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
    }
    public virtual void SetLayerRecursively(GameObject obj, LayerMask newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = newLayer;
        }
    }
    private void OnDestroy()
    {
        isLookedOn = false;
    }
    private bool IsPressed(InputKey input)
    {
        return input.type switch
        {
            InputType.Key => Input.GetKeyDown(input.key),
            InputType.Mouse => Input.GetMouseButtonDown(input.mouseButton),
            _ => false
        };
    }
}
