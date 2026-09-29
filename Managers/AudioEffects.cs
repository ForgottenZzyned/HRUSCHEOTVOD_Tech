using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioEffects : Singleton<AudioEffects>
{
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;
    public void PlayPanicEffect(float duration = 2f)
    {
        StartCoroutine(PanicRoutine(duration));
    }
    private IEnumerator PanicRoutine(float duration)
    {
        //musicMixer.DOSetFloat("MasterVolume", -20f, 0.3f).SetEase(Ease.InOutQuad);
        //musicMixer.DOSetFloat("LowPassCutoff", 5000f, 0.3f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(duration);
        //musicMixer.DOSetFloat("MasterVolume", 0f, 1f).SetEase(Ease.InOutQuad);
        //musicMixer.DOSetFloat("LowPassCutoff", 22000f, 1f).SetEase(Ease.InOutQuad);
    }
}
