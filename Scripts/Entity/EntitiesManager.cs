using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EntitiesManager : Singleton<EntitiesManager>
{
    public List<EntityData> entities = new();
    [HideInInspector] public List<Corner> corners = new();
    [HideInInspector] public List<WindowPoint> windowPoints = new();
    [Header("Interactables Effects")]
    public int safeRoomCount = 0;
    [Header("Spawn Timings")]
    public float checkTiming;
    private float lastChecked;
    [Header("Spawn Settings")]
    public int enemiesCap;
    public int currEnemies;
    [Header("Spawn Chances")]
    [Range(0, 1)]
    public float onTimerChance = 0;
    [Range(0, 1)]
    public float onDoorChance = 0;
    public float maxTimerChance = 0.05f;
    public float maxDoorChance = 0.2f;
    public int maxTimerChanceOnDoor = 200;
    public int maxDoorChanceOnDoor = 200;

    private void OnEnable()
    {
        GameEvents.OnDoorOpened += HandleDoorOpened;
    }

    private void OnDisable()
    {
        GameEvents.OnDoorOpened -= HandleDoorOpened;
    }

    private void Update()
    {
        if (!PlayerManager.Instance.IsPlayerAlive()) return;
        CheckTimers();
    }

    public void HandleDoorOpened(Door door)
    {
        if (CheckChance())
        {
            TrySpawnRandom(SpawnType.OnDoorOpen);
        }
        if (safeRoomCount != 0) safeRoomCount--;
        IncreaseChances();
    }

    private void CheckTimers()
    {
        if(checkTiming + lastChecked < Time.time)
        {
            lastChecked = Time.time;
            if (Random.value <= onTimerChance)
            {
                TrySpawnRandom(SpawnType.Timer);
            }
        }
    }

    private bool CheckChance()
    {
        return Random.value <= onDoorChance;
    }

    public void ForceSpawn(EntityData data)
    {
        Vector3 pos = PositionFinder.FindHiddenSpotDummy(PlayerManager.Instance.playerObject.transform);

        GameObject obj = Instantiate(data.prefab, pos, Quaternion.identity);

        Entity entity = obj.GetComponent<Entity>();
        entity.Initialize(data, PlayerManager.Instance.playerObject.transform);
    }

    public void TrySpawnRandom(SpawnType entityType,EntityData forceData = null)
    {
        if (safeRoomCount > 0) return;
        if (currEnemies >= enemiesCap)
            return;
        EntityData data;
        if (forceData == null) data = GetRandomEntity(entityType);
        else data = forceData;
        if (RoomChainManager.Instance.currentRoom <= data.spawnThreshold) return;
        Vector3 pos = PositionFinder.FindHiddenSpotDummy(PlayerManager.Instance.playerObject.transform);

        GameObject obj = Instantiate(data.prefab, pos, Quaternion.identity);

        Entity entity = obj.GetComponent<Entity>();
        entity.Initialize(data, PlayerManager.Instance.playerObject.transform);
        if (entityType == SpawnType.OnDoorOpen)
            if (CheckChance())
                TrySpawnRandom(SpawnType.OnDoorOpen);
    }

    private EntityData GetRandomEntity(SpawnType spawnType)
    {
        List<EntityData> temp = entities
            .Where(e => e.spawnType == spawnType)
            .ToList();
        float totalWeight = 0;
        foreach(var e in temp)
            totalWeight += e.spawnWeight;
        float rand = Random.Range(0f, totalWeight);
        foreach (var e in temp)
        {
            rand -= e.spawnWeight;
            if (rand <= 0f)
                return e;
        }
        return temp[Random.Range(0,temp.Count)];
    }

    private void IncreaseChances()
    {
        //value += ((max - min) / (durInRoomCount));
        onTimerChance += ((maxTimerChance - 0f) / (maxTimerChanceOnDoor));
        onDoorChance += ((maxDoorChance - 0f) / (maxDoorChanceOnDoor));
    }

    public void SerializeCorner(Corner c)
    {
        corners.Add(c);
    }

    public void DeserealizeCorner(Corner c)
    {
        corners.Remove(c);
    }

    public void SerializeWindowP(WindowPoint w)
    {
        windowPoints.Add(w);
    }

    public void DeserealizeWindowP(WindowPoint w)
    {
        windowPoints.Remove(w);
    }
}
