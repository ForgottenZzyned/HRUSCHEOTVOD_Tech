using System.Collections.Generic;
using UnityEngine;

public class FreecamManager : Singleton<FreecamManager>
{
    public List<TrailerWindow> windows;
    public List<TrailerWindow> forcedWindows;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DeactivateForcedWindow();
        }
        if (Input.GetKey(KeyCode.T))
        {
            DeactivateRandWindow();
        }
    }
    public void DeactivateRandWindow()
    {
        int rand = Random.Range(0, windows.Count);
        windows[rand].TriggerDeactivation();
        windows.Remove(windows[rand]);
    }
    public void DeactivateForcedWindow()
    {
        forcedWindows[0].TriggerDeactivation();
        windows.Remove(forcedWindows[0]);
        forcedWindows.Remove(forcedWindows[0]);
    }
    public void AddWindow(TrailerWindow w)
    {
        windows.Add(w);
    }
}
