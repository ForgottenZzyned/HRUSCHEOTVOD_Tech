using UnityEngine;
using DG.Tweening;
public class HUDManager : Singleton<HUDManager>
{
    public CanvasGroup canvGroup;
    public void SetAlphaTo(float value,float dur = 0.5f)
    {
        canvGroup.DOFade(value, dur);
    }
}
