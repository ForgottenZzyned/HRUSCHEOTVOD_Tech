using UnityEngine;
using DG.Tweening;

public class Closet : Openable
{
    public Transform rightPivot;
    public Transform leftPivot;
    public Transform lockCube;
    public SFXBank openSFX;
    public SFXBank closeSFX;
    public bool isLockUnlocked = false;
    Sequence seq;
    public override void SwitchState()
    {
        isOpened = !isOpened;
        seq?.Kill();
        seq = DOTween.Sequence();
        switch (isOpened)
        {
            case true:
                if (!isLockUnlocked)
                {
                    seq.Append(lockCube.DOLocalMoveX(0.032f, 0.5f));
                    isLockUnlocked = true;
                }
                SFXManager.Instance.Play(openSFX,transform.position);
                seq.Append(rightPivot.DOLocalRotate(new Vector3(0, -105, 0), 1f).SetEase(Ease.OutQuad));
                seq.Join(leftPivot.DOLocalRotate(new Vector3(0, 105, 0), 1f).SetEase(Ease.OutQuad));
                break;
            case false:
                SFXManager.Instance.Play(closeSFX, transform.position);
                seq.Append(rightPivot.DOLocalRotate(new Vector3(0, 0, 0), 0.55f).SetEase(Ease.OutCubic));
                seq.Join(leftPivot.DOLocalRotate(new Vector3(0, 0, 0), 0.55f).SetEase(Ease.OutCubic));
                break;
        }
    }
}
