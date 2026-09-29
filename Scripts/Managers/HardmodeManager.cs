using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardmodeManager : Singleton<HardmodeManager>
{
    public static bool isHard = false;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void SetHard(bool value)
    {
        isHard = value;
    }
}

