using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SocketDirection
{
    Forward,
    Back,
    Left,
    Right
}
public class DoorSocket : MonoBehaviour
{
    public bool isUsed;
    public Door door;
    public SocketDirection direction;
    
    public void LockDoor()
    {
        door.isLocked = true;
        door.Close();
        SFXManager.Instance.Play(door.closeDoorBank, transform.position);
    }
    public void LockDoorLoud()
    {
        door.isLocked = true;
        door.Close();
        SFXManager.Instance.Play(door.closeDoorLoudBank, transform.position, 0.5f);
    }
    public Door SetDoor(bool unique = false, string signText = "???")
    {
        Door door = DoorBuilder.Instance.SpawnCursedDoor(
                    gameObject.transform.position,
                    gameObject.transform.rotation,
                    gameObject.transform.root);
        if (unique) door.SetUniqueDoor(true);
        this.door = door;
        door.SetSignText(signText);
        door.Connect(this,null);
        return door;
    }
}
public static class SocketDirectionUtility
{
    public static SocketDirection GetOpposite(SocketDirection dir)
    {
        switch (dir)
        {
            case SocketDirection.Forward: return SocketDirection.Back;
            case SocketDirection.Back: return SocketDirection.Forward;
            case SocketDirection.Left: return SocketDirection.Right;
            case SocketDirection.Right: return SocketDirection.Left;
        }
        return dir;
    }
}
