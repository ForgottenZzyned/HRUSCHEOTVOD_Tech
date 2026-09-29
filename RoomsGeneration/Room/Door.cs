using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Door : MonoBehaviour
{
    public DoorSocket from;
    public DoorSocket to;
    public GameObject root;
    public Transform knobPivot;
    public float distanceToOpen = 5f;
    public SFXBank doorSound;
    public SFXBank closeDoorBank;
    public SFXBank closeDoorLoudBank;
    public bool isLocked = false;
    private bool isOpened = false;
    private bool isUnique = false;
    private bool isCorrect = true;
    public void SetCorrectDoor(bool value = true)
    {
        isCorrect = value;
    }
    public void SetUniqueDoor(bool value)
    {
        isUnique = value;
    }
    public bool IsUniqueDoor()
    {
        return isUnique;
    }
    public bool IsCorrectDoor()
    {
        return isCorrect;
    }
    public void BlurSignText()
    {
        GetComponentInChildren<DepthText>().RemoveText();
    }
    public void Connect(DoorSocket from, DoorSocket to)
    {
        this.from = from;
        this.to = to;
    }
    public bool IsConnectedToRoom(Room room)
    {
        if (from == null) return false;
        if (to == null) return false;
        if (to.transform.root == room.transform || from.transform.root == room.transform)
            return true;
        return false;
    }
    public void Open()
    {
        GameEvents.OnDoorOpened?.Invoke(this);
        SFXManager.Instance.Play(doorSound, transform.position);
        Sequence sequence = DOTween.Sequence();

        sequence
        .Append(knobPivot.DOLocalRotate(new Vector3(0, 0, -20), 0.1f)
            .SetEase(Ease.OutQuad))
        .AppendInterval(0.3f)
        .Append(knobPivot.DOLocalRotate(Vector3.zero, 0.7f)
            .SetEase(Ease.InOutQuad));
        transform.DOLocalRotate(new Vector3(0, 90f, 0),1f)
            .SetEase(Ease.OutCubic);
    }
    public void Close()
    {
        transform.DOLocalRotate(new Vector3(0, 0, 0),0.3f)
            .SetEase(Ease.OutBack);
        Destroy(transform.root.gameObject,RoomDeletionManager.Instance.baseCloseDelay * 2f);
    }
    public void SetSignText(string value)
    {
        GetComponentInChildren<DepthText>().SetText(value);
    }
    private void Update()
    {
        if(PlayerManager.Instance.IsPlayerAlive())
            CheckDistance();
    }
    private void CheckDistance()
    {
        if (Vector3.Distance(
            PlayerMovement.Instance.gameObject.transform.position,
            transform.position) < distanceToOpen && !isOpened && !isLocked)
        {
            isOpened = true;
            Open();
        }
    }
}
