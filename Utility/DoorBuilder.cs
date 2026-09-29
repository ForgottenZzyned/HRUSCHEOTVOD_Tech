using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorBuilder : Singleton<DoorBuilder>
{
    public Door SpawnDoor(Vector3 pos, Quaternion rot, bool isLocked, Transform parent = null)
    {
        Door door = Instantiate(Resources.Load("DoorRoot"),pos,rot,parent).GetComponentInChildren<Door>();
        door.isLocked = isLocked;
        return door;
    }
    public Door SpawnCursedDoor(Vector3 pos, Quaternion rot, Transform parent = null)
    {
        Door door = Instantiate(Resources.Load("DoorRoot"), pos, rot, parent).GetComponentInChildren<Door>();
        door.SetUniqueDoor(true);
        door.SetCorrectDoor(true);
        return door;
    }
}
