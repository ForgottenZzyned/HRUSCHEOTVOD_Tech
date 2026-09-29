using UnityEngine;

public class DistanceCulling : MonoBehaviour
{
    private void Start()
    {
        DistanceCullingManager.Instance.RegisterObject(gameObject);
    }

    private void OnDestroy()
    {
        DistanceCullingManager.Instance.UnregisterObject(gameObject);
    }
}
