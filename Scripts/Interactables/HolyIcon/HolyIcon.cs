using UnityEngine;
public class HolyIcon : Interactable
{
    public GameObject emmisionObject;
    public int safeRoomAdd;
    private Material emmisionMat;
    private float minDist = 0.25f;
    private float maxDist = 3.5f;   
    private float maxEmmision = 0.3f;
    private float minEmmision = 1.2f;
    private void OnEnable()
    {
        MeshRenderer renderer = emmisionObject.GetComponent<MeshRenderer>();
        emmisionMat = renderer.material;
    }
    protected override void Update()
    {
        base.Update();
        float distance = Vector3.Distance(PlayerManager.Instance.playerObject.transform.position, transform.position);

        float t = Mathf.InverseLerp(minDist, maxDist, distance);

        float exposure = Mathf.Lerp(maxEmmision, minEmmision, t);

        emmisionMat.SetFloat("_EmissiveExposureWeight", exposure);
    }
    protected override void OnUse()
    {
        EntitiesManager.Instance.safeRoomCount += safeRoomAdd;
        InteractablesManager.OnInteractableUsed?.Invoke($"You feel safe for [{safeRoomAdd}] rooms");
    }
}
