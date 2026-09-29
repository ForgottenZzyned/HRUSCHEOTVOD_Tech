using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CursedDecisionRoom : Room
{
    public float cursedDoors;
    private Door doorToThisRoom;
    public List<string> badStrings = new();
    public List<string> goodStrings = new();
    public override void ActivateRoom()
    {
        base.ActivateRoom();
        foreach(var socket in data.sockets)
        {
            if (socket.door == null || socket.door.to.transform.root.GetComponent<Room>() == null) continue;
            if (socket.door.to.transform.root.GetComponent<Room>() is CursedDecisionRoom room &&
                room == this)
            {
                socket.door.BlurSignText();
                break;
            }
        }
        AssignDoors();
    }

    private void AssignDoors()
    {
        List<Door> doorsList = new List<Door>();
        string str;
        foreach (var socket in data.sockets)
        {
            if (!socket.isUsed)
            {
                Door door = socket.SetDoor(true);
                doorsList.Add(door);
            }
        }
        if (doorsList.Count == 0)
        {
            Debug.LogError("Can't find any door in Unique Room!");
            return;
        }
        for (int j = 0; j < cursedDoors;j++)
        {
            Door randDoor = doorsList[Random.Range(0, doorsList.Count)];
            str = badStrings[Random.Range(0, badStrings.Count)];
            int randNum;
            if (Random.value <= 0.50f) randNum = 1;
            else randNum = -1;
            randDoor.SetSignText((RoomChainManager.Instance.currentGeneratedRoom + randNum).ToString());
            randDoor.SetCorrectDoor(false);            
            doorsList.Remove(randDoor);
        } 
        foreach (var door in doorsList)
        {
            str = goodStrings[Random.Range(0, goodStrings.Count)];
            door.SetSignText(RoomChainManager.Instance.currentGeneratedRoom.ToString());
        }
    }
}
