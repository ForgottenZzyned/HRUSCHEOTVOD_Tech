using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInteractableSpot : InteractableSpot
{
    [Range(0,1)]
    public float spawnChance = 1f;
    public List<Interactable> customPool;
    public override void TrySpawnInteractable(Interactable prefab)
    {
        if (customPool.Count > 0) prefab = InteractablesManager.Instance.GetRandomInteractable(maxWeight, customPool);
        //Local Spawn chance
        if (Random.value > spawnChance)
            return;
        Interactable interact = Instantiate(
            prefab,
            Vector3.zero,
            Quaternion.Euler(0, Random.Range(0f, 360f), 0),
            transform.root
            );
        interact.transform.position = GetSpawnPos();
        interact.transform.parent = transform;
        interact.currSpot = this;
    }
    public void SetInteractable(Interactable instance)
    {
        instance.transform.position = GetSpawnPos();
        instance.transform.parent = transform;
        instance.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        instance.currSpot = this;
    }
    public Vector3 GetSpawnPos()    
    {
        int mask = ~LayerMask.GetMask("Interactable");
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f,mask))
        {
            return hit.point;
        }
        return Vector3.zero + (Vector3.up*50);
    }
}
