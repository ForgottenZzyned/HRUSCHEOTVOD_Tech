using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoom : Room
{
    public Transform spawnPos;

    private void Start()
    {
        ActivateRoom();
    }
}
