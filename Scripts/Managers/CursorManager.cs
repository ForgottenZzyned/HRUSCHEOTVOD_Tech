using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    public static void SetCursor(bool active)
    {
        Cursor.visible = active;
        Cursor.lockState = active ?
            Cursor.lockState = CursorLockMode.None
            : Cursor.lockState = CursorLockMode.Locked;
    }
}
