using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DistanceCullingManager : Singleton<DistanceCullingManager>
{
    private readonly List<GameObject> cullingObjects = new();
    private Transform player;

    [SerializeField] private float objecDisableDistance = 30f;
    public void RegisterPlayer(Transform player)
    {
        this.player = player;
    }
    public void RegisterObject(GameObject obj)
    {
        cullingObjects.Add(obj);
    }
    public void UnregisterObject(GameObject obj)
    {
        cullingObjects.Add(obj);
    }

    private void Update()
    {
        CheckDistance();
    }

    private void CheckDistance()
    {
        Vector3 playerPos = player.position;
        float maxDistanceSqr = objecDisableDistance * objecDisableDistance;

        for (int i = cullingObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = cullingObjects[i];
            if (obj == null)
            {
                cullingObjects.RemoveAt(i);
                continue;
            }

            float distanceSqr = (obj.transform.position - playerPos).sqrMagnitude;
            bool shouldBeActive = distanceSqr <= maxDistanceSqr;

            if (obj.activeSelf != shouldBeActive)
                obj.SetActive(shouldBeActive);
        }
    }
}
