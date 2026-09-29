using UnityEngine;
public class PlayerLookDetector : Singleton<PlayerLookDetector>
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float distance = 5f;
    [SerializeField] private LayerMask interactableMask;

    private IInteractable currentTarget;

    private void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out RaycastHit hit, distance, interactableMask))
        {
            IInteractable target = hit.collider.GetComponentInParent<IInteractable>();

            if (target != currentTarget)
            {
                currentTarget?.OnLookExit();

                currentTarget = target;

                currentTarget?.OnLookEnter();
            }
        }
        else
        {
            if (currentTarget == null) return;
            if (currentTarget != null)
            {
                currentTarget?.OnLookExit();
                currentTarget = null;
            }
        }
        if (currentTarget == null) return;
        if (Input.GetKeyDown(KeyCode.R) && (currentTarget as Interactable).canBeStored)
            (currentTarget as Interactable).SetToSlot();
    }
    public void ClearCurrTarget()
    {
        currentTarget = null;
    }
}
