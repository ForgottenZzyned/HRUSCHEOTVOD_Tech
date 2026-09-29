using UnityEngine;
using UnityEngine.EventSystems;

public class HardModeVisibility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainMenuButton hardModeToggle;
    public MainMenuButton selfButton;
    public bool canToggle = false;
    private bool entered = false;
    [Header("Timer")]
    private float timer;
    private float idleTime;
    private void Start()
    {
        selfButton = GetComponent<MainMenuButton>();
        idleTime = selfButton.time;
    }
    private void Update()
    {
        if (canToggle != selfButton.isVisible && timer <= 0) RenewIdle();
        CheckTimer();
    }
    private void RenewIdle()
    {
        timer = idleTime;
    }
    private void CheckTimer()
    {
        if (timer <= 0) return;
        if (timer > 0) timer -= Time.deltaTime;
        if (timer <= 0) canToggle = selfButton.isVisible;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canToggle) entered = true;
        if (canToggle)hardModeToggle.ToggleVisibility();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canToggle && entered) hardModeToggle.ToggleVisibility();
        entered = false;
    }
}
