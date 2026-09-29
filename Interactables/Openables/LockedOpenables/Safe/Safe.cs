using DG.Tweening;
using UnityEngine;
public class Safe : Openable
{
    public Transform doorPivot;
    public Transform knobPivot;
    Sequence seq;
    public override void SwitchState()
    {
        isOpened = !isOpened;
        seq?.Kill();
        seq = DOTween.Sequence();
        switch (isOpened)
        {
            case true:
                seq.Append(knobPivot.DOLocalRotate(new Vector3(0, 0, 90), 0.25f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
                seq.Append(doorPivot.DOLocalRotate(new Vector3(0, -85, 0), 1f).SetEase(Ease.OutQuad));
                break;
            case false:
                seq.Append(doorPivot.DOLocalRotate(new Vector3(0, 0, 0), 0.55f).SetEase(Ease.OutCubic));
                seq.Append(knobPivot.DOLocalRotate(new Vector3(0, 0, 0), 0.25f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
                break;
        }
    }
}
