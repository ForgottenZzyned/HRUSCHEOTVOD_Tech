using UnityEngine;
using Cinemachine;
using DG.Tweening;
public class FOVManager : Singleton<FOVManager>
{
    private CinemachineVirtualCamera vcam;
    private float baseFOV;
    private bool isTriggered = false;
    private bool isSpeedMult = false;
    public bool IsSpeedMult
    {
        get => isSpeedMult;
        set
        {
            isSpeedMult = value;
            DOTween.Kill("FOVChange");
            if (isSpeedMult)
            {
                DOVirtual.Float(
                vcam.m_Lens.FieldOfView,
                115f,
                0.15f,
                x => vcam.m_Lens.FieldOfView = x
                ).SetEase(Ease.OutQuad).SetId("PermFOV");
            }
            else
            {
                DOVirtual.Float(
                115f,
                baseFOV,
                2f,
                x => vcam.m_Lens.FieldOfView = x
                ).SetId("PermFOV");
            }
        }
    }
    public void SerializeCamera(CameraMovement camS)
    {
        vcam = camS.GetCamera();
        baseFOV = vcam.m_Lens.FieldOfView;
    }
    public void TriggerFOV(float dur, float maxValue)
    {
        if (isTriggered || isSpeedMult) return;
        DOTween.Kill("FOVChange");
        Sequence seq;
        seq = DOTween.Sequence().SetId("FOVChange");
        isTriggered = true;
        seq.Append(DOVirtual.Float(
            baseFOV,
            maxValue,
            0.15f,
            x => vcam.m_Lens.FieldOfView = x
            ).SetEase(Ease.OutQuad));
        seq.Append(DOVirtual.Float(
            maxValue, 
            baseFOV, 
            dur, 
            x => vcam.m_Lens.FieldOfView = x
            ).OnComplete(() =>
            {
                isTriggered = false;
            }));
    }
}
