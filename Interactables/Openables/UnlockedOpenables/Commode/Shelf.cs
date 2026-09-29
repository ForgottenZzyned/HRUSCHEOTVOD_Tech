using DG.Tweening;
using UnityEngine;

public class Shelf : Openable
{
    public Transform shelfPivot;
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
                seq.Append(shelfPivot.DOLocalMoveX(Random.Range(0.45f, 0.6f),0.3f)
                    .SetEase(Ease.OutCubic));
                SFXManager.Instance.Play(openSFX, transform.position);
                break;
            case false:
                seq.Append(shelfPivot.DOLocalMoveX(0.018f, 0.3f)
                    .SetEase(Ease.OutCubic));
                SFXManager.Instance.Play(closeSFX, transform.position);
                break;
        }
    }
}
