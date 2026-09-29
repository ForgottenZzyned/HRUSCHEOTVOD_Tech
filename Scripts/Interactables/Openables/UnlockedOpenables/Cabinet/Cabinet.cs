using DG.Tweening;
using UnityEngine;

public class Cabinet : Openable
{
    public Transform rightPivot;
    public Transform leftPivot;
    public SFXBank openSFX;
    public SFXBank closeSFX;
    Sequence seq;
    public override void SwitchState()
    {
        isOpened = !isOpened;
        seq?.Kill();
        seq = DOTween.Sequence();
        switch (isOpened)
        {
            case true:
                SFXManager.Instance.Play(openSFX, transform.position);
                seq.Append(rightPivot.DOLocalRotate(new Vector3(0, -95, 0), 0.77f).SetEase(Ease.OutCubic));
                seq.Join(leftPivot.DOLocalRotate(new Vector3(0, 95, 0), 0.77f).SetEase(Ease.OutCubic));
                break;
            case false:
                SFXManager.Instance.Play(closeSFX, transform.position);
                seq.Append(rightPivot.DOLocalRotate(new Vector3(0, 0, 0), 0.55f).SetEase(Ease.OutCubic));
                seq.Join(leftPivot.DOLocalRotate(new Vector3(0, 0, 0), 0.55f).SetEase(Ease.OutCubic));
                break;
        }
    }
}
