using DG.Tweening;
using TMPro;

public static class TMPTextExtensions
{
    public static Tween DOTypeText(this TMP_Text text, string targetText, float duration)
    {
        text.text = "";

        return DOTween.To(
            () => 0,
            value => text.text = targetText[..value],
            targetText.Length,
            duration
        ).SetEase(Ease.Linear);
    }
}
