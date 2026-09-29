using UnityEngine;

public class HouseRebuildingObject : MonoBehaviour
{
    private void Start()
    {
        SFXManager.Instance.Play(HouseRebuildingAmbient.Instance.ambientSounds,transform.position,0.5f);
    }
    private void OnDestroy()
    {
        SFXManager.Instance.Play(HouseRebuildingAmbient.Instance.ambientSounds, transform.position, 0.5f);
    }
}
