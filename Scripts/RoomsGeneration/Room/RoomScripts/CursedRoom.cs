using UnityEngine;
using System.Collections.Generic;

public class CursedRoom : Room
{
    public List<EntityData> entityToSpawn;
    public int entityCount;
    public override void ActivateRoom()
    {
        base.ActivateRoom();
        SpawnEntities();
    }
    private void SpawnEntities()
    {

        for(int i = 0; i < entityCount; i++)
        {
            EntityData data = entityToSpawn[Random.Range(0, entityToSpawn.Count)];
            EntitiesManager.Instance.ForceSpawn(data);
        }
    }
}
