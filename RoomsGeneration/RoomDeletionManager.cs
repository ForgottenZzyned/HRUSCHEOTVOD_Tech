using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDeletionManager : Singleton<RoomDeletionManager>
{
    public float baseCloseDelay = 30f;
    [SerializeField] private float openDoorTimeReduction = 2f;

    public float currentTimer = 999f;
    private bool timerStarted;
    private bool stopLoop = false;
    private void OnEnable()
    {
        GameEvents.OnDoorOpened += HandleDoorOpened;
    }
    private void OnDisable()
    {
        GameEvents.OnDoorOpened -= HandleDoorOpened;
    }
    public void HandleDoorOpened(Door openedDoor)
    {
        if (!timerStarted)
        {
            timerStarted = true;
            currentTimer = baseCloseDelay;
            StartCoroutine(RoomCloseLoop());
        }
        else
        {
            currentTimer -= openDoorTimeReduction;
            currentTimer = Mathf.Max(currentTimer, 3f);
        }
    }
    public void ForceStopDeletionLoop()
    {
        StopCoroutine(RoomCloseLoop());
        stopLoop = true;
    }
    private IEnumerator RoomCloseLoop()
    {
        bool isLightsOff = false;
        while (true)
        {
            if (stopLoop) break;
            if (RoomChainManager.Instance.CheckRoomsBehindCount())
            {
                currentTimer = -1f;
            }
            else
            {
                currentTimer -= Time.deltaTime;
            }
            if(currentTimer < 10f && !isLightsOff)
            {
                RoomChainManager.Instance.WarningOldestRoom();
                isLightsOff = true;
            }
            if (currentTimer <= 0f)
            {
                RemoveOldRoom();
                isLightsOff = false;
                if (!PlayerManager.Instance.IsPlayerAlive())
                    break;
            }
            
            yield return null;          
        }
    }
    private void RemoveOldRoom()
    {
        RoomChainManager.Instance.RemoveOldestRoom();
        currentTimer = baseCloseDelay;
    }
}
