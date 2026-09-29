using DG.Tweening;
using TMPro;
using UnityEngine;

public class Disclaimer : MonoBehaviour
{
    public TMP_Text warnText;
    public TMP_Text mainText;
    public TMP_Text discretText;

    public Tween ShowDisclaimer()
    {
        Sequence seq = DOTween.Sequence().SetEase(Ease.InOutQuad);
        seq.Insert(0.5f, warnText.DOFade(1, 1.4f))
            .Insert(2f, mainText.DOFade(1, 3f)).SetEase(Ease.InOutQuad)
            .Insert(3f, warnText.DOColor(Color.red, 4f)).SetEase(Ease.InOutQuad)
            .Insert(4f, discretText.DOFade(1, 3f));
        return seq;
    }
    public Tween HideDisclaimer()
    {
        Color col = Color.white;
        col.a = 0f;
        Sequence seq = DOTween.Sequence().SetEase(Ease.InOutQuad);
        seq.Append(warnText.DOFade(0, 1f))
            .Join(warnText.DOColor(col, 1f))
            .Join(mainText.DOFade(0, 1f))
            .Join(discretText.DOFade(0, 1f));
        return seq;
    }
}
