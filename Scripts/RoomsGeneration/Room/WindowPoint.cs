using UnityEngine;

public class WindowPoint : MonoBehaviour
{
    public bool isOccupied = false;
    private void Start()
    {
        EntitiesManager.Instance.SerializeWindowP(this);
    }
    private void OnDestroy()
    {
        EntitiesManager.Instance.DeserealizeWindowP(this);
    }
}
