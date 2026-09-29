using UnityEngine;
using DG.Tweening;

public class DotVisibility : MonoBehaviour
{
    public CanvasGroup canvGroup;
    public bool isVisible = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchVisibility();
        }
    }
    public void SwitchVisibility()
    {
        isVisible = !isVisible;
        canvGroup.DOFade(isVisible ? 1:0,0.2f);
    }
}
