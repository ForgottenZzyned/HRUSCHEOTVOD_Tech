using UnityEngine;

public class RandomInteriorReplacer : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [Range(0,1)]
    [SerializeField] private float chance;
    public void Randomize()
    {
        if (Random.value > chance)
            return;

        GameObject obj = Instantiate(
            prefabs[Random.Range(0, prefabs.Length)],
            transform.position,
            transform.rotation,
            transform.parent);
        var itemSpots = obj.GetComponentsInChildren<InteractableSpot>(true);

        foreach (var item in itemSpots)
            item.Init();
        Destroy(gameObject);
    }
}
