using UnityEngine;
public abstract class Openable : MonoBehaviour
{
    public bool isLocked = false;
    public bool isOpened = false;
    public abstract void SwitchState();
}
