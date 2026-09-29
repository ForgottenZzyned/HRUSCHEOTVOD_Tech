using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : Singleton<ScreenFader>
{
    [SerializeField] private Image overlay;

    public Tween FadeToBlack(float duration)
    {
        return overlay.DOFade(1f, duration);
    }

    public Tween FadeFromBlack(float duration)
    {
        return overlay.DOFade(0f, duration);
    }
}
