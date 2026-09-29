using UnityEngine;
using DG.Tweening;

public class Nightstand : Openable
{
    public Transform doorPivot;
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
                seq.Append(doorPivot.DOLocalRotate(new Vector3(0, -85, 0), 0.66f).SetEase(Ease.OutCubic));
                break;
            case false:
                SFXManager.Instance.Play(closeSFX, transform.position);
                seq.Append(doorPivot.DOLocalRotate(new Vector3(0, 0, 0), 0.55f).SetEase(Ease.OutCubic));
                break;
        }
    }
}
