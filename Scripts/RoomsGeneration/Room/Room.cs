using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RoomType
{
    Basic,
    Unique,
    Ending
}
[System.Serializable]
public class RoomData
{
    public float weight;
    public float roomThreshold;
    public List<DoorSocket> sockets;
    public List<LightSource> lightSources;
    public RoomType type = RoomType.Basic;
}
public abstract class Room : MonoBehaviour
{
    public RoomData data;
    public BoxCollider roomTrigger;
    public bool playerInside;
    public bool roomActive = false;

    public void Awake()
    {
        var itemSpots = GetComponentsInChildren<InteractableSpot>(true);

        foreach (var item in itemSpots)
            item.Init();
    }
    public DoorSocket GetRandomFreeSocket()
    {
        List<DoorSocket> free = data.sockets.FindAll(s => !s.isUsed);
        return free[Random.Range(0, free.Count)];
    }
    public DoorSocket GetSocketByDirection(SocketDirection neededDirection, bool ignoreUsed = false)
    {
        foreach (var socket in data.sockets)
        {
            if(ignoreUsed && socket.direction == neededDirection)
                return socket;
            if (!socket.isUsed && socket.direction == neededDirection)
                return socket;
        }

        return null;
    }
    public virtual void ActivateRoom()
    {
        TriggerLights();
        RoomChainManager.Instance.currentRoom++;
        AnalyticsManager.RoomEntered(RoomChainManager.Instance.currentRoom,data.type.ToString());
        foreach (var randomizer in GetComponentsInChildren<RandomInteriorReplacer>())
        {
            randomizer.Randomize();
        }
        roomActive = true;
    }
    public virtual void CloseRoom()
    {
        if (this == null)
        {
            Debug.LogError("[ROOM] Tried closing already destroyed room");
            return;
        }
        if (playerInside)
        {
            StartCoroutine(SuffocatePlayer());
            return;
        }
        Destroy(gameObject);
    }
    public void CloseBackDoor(bool playerIn = false)
    {
        foreach (var socket in data.sockets)
        {
            if (socket.direction == SocketDirection.Back)
            {
                if(!playerIn) socket.LockDoor();
                else socket.LockDoorLoud();
            }
        }
    }
    public bool HasSocketDirection(SocketDirection dir)
    {
        foreach (var socket in data.sockets)
        {
            if (socket.direction == dir)
                return true;
        }
        return false;
    }
    public virtual void SetLightsOff()
    {
        if(data.lightSources.Count == 0)
        if (AtmosphereManager.Instance.mainLight.intensity > 30000f) return;
        List<LightSource> temp = new(data.lightSources);
        foreach (var lightSource in temp)
        {
            lightSource.ForceBurnout();
            data.lightSources.Remove(lightSource);
        }
    }
    public Vector3 GetRandomPointInRoom()
    {
        Vector3 localPoint = new Vector3(
            Random.Range(-0.5f, 0.5f) * roomTrigger.size.x,
            Random.Range(-0.5f, 0.5f) * roomTrigger.size.y,
            Random.Range(-0.5f, 0.5f) * roomTrigger.size.z
            );
        localPoint += roomTrigger.center;
        if (Random.value < 0.7f)
        {
            localPoint.x = Mathf.Sign(localPoint.x) * roomTrigger.size.x * 0.5f;
        }
        return roomTrigger.transform.TransformPoint(localPoint);
    }
    private IEnumerator SuffocatePlayer()
    {
        RoomDeletionManager.Instance.ForceStopDeletionLoop();
        AtmosphereManager.Instance.SetSunIntensity(0f, 5f);
        yield return new WaitForSeconds(7f);
        Flashlight.Instance.ForceBreakFlashlight();
        yield return new WaitForSeconds(3f);
        while (PlayerManager.Instance.IsPlayerAlive())
        {
            GameOverController.Instance.killedString = "You got left behind...";
            PlayerManager.Instance.HitPlayer(25f,"Suffocation");
            yield return new WaitForSeconds(1f);
        }
    }

    private void TriggerLights()
    {
        foreach (var lightSource in data.lightSources)
        {
            lightSource.ActivateSource();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (!roomActive)
                ActivateRoom();
        }    
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
