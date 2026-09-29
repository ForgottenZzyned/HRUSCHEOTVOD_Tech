using DG.Tweening;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    public Vector3 visibleOffset;
    public bool isVisible = true;
    public float time = 0.5f;
    public Ease ease = Ease.Linear;
    public bool isOff = true;
    private void OnEnable()
    {
        if(isOff) ToggleVisibility(-0.1f);
    }
    
    public void ToggleVisibility(float forcedDur = 0)
    {
        DOTween.Kill(transform);
        float dur = time;
        if (forcedDur != 0) dur = forcedDur;
        isVisible = !isVisible;
        if (isVisible)
        {
            Vector2 finalPos = transform.position + visibleOffset;
            transform.DOMove(finalPos, dur).SetEase(ease)
            .SetUpdate(UpdateType.Normal, true).SetId("test")
            .OnKill(() =>
            {
                transform.position = finalPos;
            });
        }
        else
        {
            Vector2 finalPos = transform.position - visibleOffset;
            transform.DOMove(finalPos, dur).SetEase(ease)
            .SetUpdate(UpdateType.Normal, true).SetId("test")
            .OnKill(() =>
            {
                transform.position = finalPos;
            });
        }
    }
}
