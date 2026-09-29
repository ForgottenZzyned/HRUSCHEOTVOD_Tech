using UnityEngine;
public class Corner : MonoBehaviour
{
    public Transform peekPosition; 
    public Transform lookPoint;

    public bool isOccupied = false;
    private void Start()
    {
        peekPosition = transform;
        EntitiesManager.Instance.SerializeCorner(this);
    }
    private void OnDestroy()
    {
        EntitiesManager.Instance.DeserealizeCorner(this);
    }
}
