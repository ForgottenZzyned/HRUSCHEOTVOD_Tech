using System.Collections.Generic;
using UnityEngine;
public enum GenerationState
{
    Normal,
    WaitingForUniqueDecision,
    SpecialChain
}
public class RoomChainManager : Singleton<RoomChainManager>
{
    public int currentRoom = 0;
    public int currentGeneratedRoom = 1;
    public int activeRoomsCount = 1;
    public GenerationState state = GenerationState.Normal;
    [Header("Generation Settings")]
    public StartRoom startRoom;
    public EndingRoom endingRoom;
    public List<Room> roomPrefabs;
    public int forwardBuffer = 3;
    public int backwardBuffer = 3;
    public int lastRoomCount = 200;
    [Header("Special Rooms")]
    public List<Room> specialRoomPrefabs;
    private int specialRoomsLeft = 0;
    [Header("Custom Spawn Settings")]
    public int customRoomNum = 0;
    public Room customRoom;

    //private bool isGenerationFrozen = false;
    //private Room activeUniqueRoom = null;

    private LinkedList<Room> activeRooms = new LinkedList<Room>();
    private Room lastRoom;
    private void Start()
    {
        Room first = Instantiate(startRoom, Vector3.zero, Quaternion.identity); 
        PlayerManager.Instance.SpawnPlayer(startRoom.spawnPos.position,startRoom.spawnPos.rotation);
        activeRooms.AddFirst(first);
        lastRoom = first;
        PreGenerateForward(first);
    }
    private void Update()
    {
        activeRoomsCount = activeRooms.Count;
    }
    private void OnEnable()
    {
        GameEvents.OnDoorOpened += HandleDoorOpened;
    }
    private void OnDisable()
    {
        GameEvents.OnDoorOpened -= HandleDoorOpened;
    }
    public void PreGenerateForward(Room fromRoom, DoorSocket firstCustom = null)
    {
        Room current = fromRoom;

        for (int i = 0; i < forwardBuffer; i++)
        {
            switch (state)
            {
                case GenerationState.Normal:
                    GenerateNormalRoom(firstCustom);
                    break;
                case GenerationState.SpecialChain:
                    GenerateSpecialRoom(firstCustom);
                    break;
                default:
                    return;
            }
            firstCustom = null;
        }
    }
    private void GenerateNormalRoom(DoorSocket customSocket = null)
    {
        Room lastRoom = activeRooms.Last.Value;
        Room newRoom;

        if (currentGeneratedRoom == lastRoomCount)
        {
            newRoom = GenerateLastRoom(lastRoom);
            activeRooms.AddLast(newRoom);
            return;
        }

        if (currentGeneratedRoom > lastRoomCount) return;

        newRoom = GenerateNextRoom(lastRoom, customSocket);
        activeRooms.AddLast(newRoom);

        if (newRoom.data.type == RoomType.Unique)
        {
            state = GenerationState.WaitingForUniqueDecision;
        }
    }
    private Room GenerateNextRoom(Room previousRoom, DoorSocket customSocket = null)
    {
        DoorSocket exitSocket;
        if (customSocket == null)
            exitSocket = previousRoom.GetRandomFreeSocket();
        else
            exitSocket = customSocket;
        SocketDirection neededDirection =
            SocketDirectionUtility.GetOpposite(exitSocket.direction);

        Room newRoom = Instantiate(GetRandomRoom(neededDirection,roomPrefabs));

        DoorSocket entrySocket = newRoom.GetSocketByDirection(neededDirection);

        Quaternion rot =
            exitSocket.transform.rotation *
            Quaternion.Inverse(entrySocket.transform.localRotation);

        Vector3 pos =
            exitSocket.transform.position -
            (rot * entrySocket.transform.localPosition);
        newRoom.transform.position = pos;
        newRoom.transform.rotation = rot;
        entrySocket.isUsed = true;
        exitSocket.isUsed = true;
        
        if(customSocket == null) SpawnDoor(exitSocket, entrySocket);

        currentGeneratedRoom++;

        return newRoom;
    }
    private void GenerateSpecialRoom(DoorSocket customSocket = null)
    {
        if (specialRoomsLeft <= 0)
        {
            state = GenerationState.Normal;
            GenerateNormalRoom();
            return;
        }

        Room lastRoom = activeRooms.Last.Value;
        DoorSocket exitSocket;
        if (customSocket == null)
            exitSocket = lastRoom.GetRandomFreeSocket();
        else
            exitSocket = customSocket;
        SocketDirection neededDirection =
                SocketDirectionUtility.GetOpposite(exitSocket.direction);

        Room newRoom = Instantiate(GetRandomRoom(neededDirection, specialRoomPrefabs));

        DoorSocket entrySocket = newRoom.GetSocketByDirection(neededDirection);
        Quaternion rot =
            exitSocket.transform.rotation *
            Quaternion.Inverse(entrySocket.transform.localRotation);

        Vector3 pos =
            exitSocket.transform.position -
            (rot * entrySocket.transform.localPosition);
        newRoom.transform.position = pos;
        newRoom.transform.rotation = rot;

        if (!exitSocket.isUsed && customSocket == null) SpawnDoor(exitSocket, entrySocket);

        entrySocket.isUsed = true;
        if(!exitSocket.isUsed) exitSocket.isUsed = true;

        activeRooms.AddLast(newRoom);
        specialRoomsLeft--;
    }
    private Room GenerateLastRoom(Room previousRoom)
    {
        DoorSocket exitSocket = previousRoom.GetRandomFreeSocket();

        SocketDirection neededDirection =
            SocketDirectionUtility.GetOpposite(exitSocket.direction);

        Room newRoom = Instantiate(endingRoom);

        DoorSocket entrySocket = newRoom.GetSocketByDirection(neededDirection);

        Quaternion rot =
            exitSocket.transform.rotation *
            Quaternion.Inverse(entrySocket.transform.localRotation);

        Vector3 pos =
            exitSocket.transform.position -
            (rot * entrySocket.transform.localPosition);
        newRoom.transform.position = pos;
        newRoom.transform.rotation = rot;
        entrySocket.isUsed = true;
        exitSocket.isUsed = true;

        SpawnDoor(exitSocket, entrySocket);

        currentGeneratedRoom++;

        return newRoom;
    }
    private Room GetRandomRoom(SocketDirection requiredDirection, List<Room> roomList)
    {
        List<Room> validRooms = new List<Room>();
        foreach (var prefab in roomList)
        {
            if (!prefab.HasSocketDirection(requiredDirection))
                continue;
            if (prefab == lastRoom)
                continue;
            if (currentRoom < prefab.data.roomThreshold)
                continue;

            validRooms.Add(prefab);
        }
        //fallback
        if (validRooms.Count == 0)
        {
            foreach (var prefab in roomList)
            {
                if (prefab.HasSocketDirection(requiredDirection))
                    validRooms.Add(prefab);
            }
        }
        //fallback2
        if (validRooms.Count == 0)
        {
            Debug.LogError($"No rooms with direction {requiredDirection}");
            return null;
        }

        float totalWeight = 0f;

        foreach (var room in validRooms)
            totalWeight += room.data.weight;

        float randomPoint = Random.Range(0f, totalWeight);

        foreach (var room in validRooms)
        {
            randomPoint -= room.data.weight;

            if (randomPoint <= 0f)
            {
                lastRoom = room;
                return room;
            }
                
        }
        return validRooms[validRooms.Count - 1];
    }
    private void SpawnDoor(DoorSocket a, DoorSocket b)
    {
        Vector3 pos = a.transform.position;
        Quaternion rot = a.transform.rotation;

        Door door = DoorBuilder.Instance.SpawnDoor(pos,rot,false);
        a.door = door;
        b.door = door;
        door.Connect(a, b);
        door.SetSignText(currentGeneratedRoom == lastRoomCount ? 
            "EXIT" : currentGeneratedRoom.ToString());
    }
    private void HandleDoorOpened(Door openedDoor)
    {
        if (state == GenerationState.WaitingForUniqueDecision)
        {
            if (!openedDoor.IsUniqueDoor()) return;
            HandleUniqueDecision(openedDoor);
            return;
        }
        if (state == GenerationState.SpecialChain)
        {
            GenerateSpecialRoom();
            return;
        }
        GenerateNormalRoom();
    }
    private void HandleUniqueDecision(Door openedDoor)
    {
        bool isCorrect = openedDoor.IsCorrectDoor();

        Room room = openedDoor.from.transform.root.GetComponent<Room>();
        DoorSocket customSock = null;
        foreach (var socket in room.data.sockets)
        {
            if (socket.door == null) continue;
            if (socket.door == openedDoor)
            {
                socket.isUsed = false;
                customSock = socket;
                continue;
            }
            if (!socket.door.isLocked)
                socket.LockDoor();
        }
        if (!isCorrect)
        {
            state = GenerationState.SpecialChain;
            specialRoomsLeft = Random.Range(3, 6);
            currentRoom -= specialRoomsLeft;
        }
        else
        {
            state = GenerationState.Normal;
        }

        PreGenerateForward(room, customSock);
        var nextRoom = activeRooms.Find(room).Next;
        nextRoom.Value.GetSocketByDirection(SocketDirectionUtility.GetOpposite(customSock.direction),true).door = customSock.door;
        customSock.door.root.transform.parent = nextRoom.Value.transform;
    }
    public bool CheckRoomsBehindCount()
    {
        if (Mathf.Max(activeRooms.Count - forwardBuffer - 1, 0) > backwardBuffer)
        {
            return true;
        }
        return false;
    }
    public void WarningOldestRoom()
    {
        if (activeRooms.Last.Value.data.type == RoomType.Ending
            && activeRooms.Count == 1)
        {
            RoomDeletionManager.Instance.ForceStopDeletionLoop();
            return;
        }
        Room oldest = activeRooms.First.Value;
        oldest.SetLightsOff();
    }
    public void BreakAllAvaibleLights()
    {
        var node = activeRooms.First;
        int index = 0;
        while (node != null)
        {
            if (!node.Value.roomActive)
            {
                node = node.Next;
                continue;
            }
            node.Value.SetLightsOff();
            node = node.Next;
            index++;
        }
    }
    public void RemoveOldestRoom()
    {
        if (activeRooms.Last.Value.data.type == RoomType.Ending
            && activeRooms.Count == 1)
        {
            RoomDeletionManager.Instance.ForceStopDeletionLoop();
            return;
        }
            
        Room oldest = activeRooms.First.Value;
        Room next = activeRooms.First.Next.Value;

        next.CloseBackDoor(oldest.playerInside);
        activeRooms.RemoveFirst();

        oldest.CloseRoom();
    }
    public void CloseAllRooms()
    {
        for(int i = 0; i < 5; i++)
        {
            RemoveOldestRoom();
        }
    }
}
