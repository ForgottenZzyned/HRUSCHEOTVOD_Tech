using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    private bool isActivated = false;
    private void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;
        if (!other.CompareTag("Player"))
            return;
        isActivated = true;
        GetComponentInParent<EndingRoom>().StartCutscene();
    }
}
