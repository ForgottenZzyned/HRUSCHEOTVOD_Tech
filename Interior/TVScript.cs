using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TVScript : MonoBehaviour
{
    public MeshRenderer meshR;
    public Light areaLight;
    public bool isActive;
    [Header("Activation Settings")]
    [Range(0, 1)]
    public float activationChance = 0;
    public Vector2 minMaxTimeOfActivation = new Vector2(5,15);
    public float activationCD = 5f;
    public Ease easing = Ease.Linear;
    [Header("Sound Settings")]
    public SFXBank enabledSFX;
    public SFXBank disabledSFX;
    public AudioClip staticSFX;

    private AudioSource staticSource;
    private Material noiseMat;
    private Coroutine waitCor;
    private void OnEnable()
    {
        noiseMat = new Material(meshR.material);
        meshR.material = noiseMat;
        staticSource = GetComponent<AudioSource>();
        Shutdown();
        StartCoroutine(Frame());
        IEnumerator Frame()
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(RandomActivate());
        }
    }
    private IEnumerator RandomActivate()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            float rand = Random.value;
            if (rand < activationChance)
            {
                ActivateNoise();
                yield return new WaitForSeconds(Random.Range(minMaxTimeOfActivation.x, minMaxTimeOfActivation.y));
                Shutdown();
                yield return new WaitForSeconds(activationCD);
            }
        }
    }
    public void ActivateNoise()
    {
        if (isActive) return;
        isActive = true;
        areaLight.enabled = true;
        DOTween.To(
            () => 1f,
            x => noiseMat.SetFloat("_ShutdownProgress", x),
            0f,
            0.4f
        ).SetEase(easing);
        SFXManager.Instance.Play(enabledSFX,transform.position);
        StartCoroutine(WaitForClip());
        IEnumerator WaitForClip()
        {
            yield return new WaitForSeconds(enabledSFX.GetClip().length);
            staticSource.enabled = true;
        }
        waitCor = StartCoroutine(WaitForMaxTime());
        IEnumerator WaitForMaxTime()
        {
            yield return new WaitForSeconds(minMaxTimeOfActivation.y + 5f);
            Shutdown();
        }
    }
    public void Shutdown()
    {
        if(waitCor != null)StopCoroutine(waitCor);
        isActive = false;
        areaLight.enabled = false;
        DOTween.To(
            () => 0f,
            x => noiseMat.SetFloat("_ShutdownProgress", x),
            1f,
            0.4f
        ).SetEase(easing);
        staticSource.enabled = false;
        if(PlayerManager.Instance.playerObject != null)SFXManager.Instance.Play(disabledSFX, transform.position);
        StartCoroutine(WaitForClip());
        IEnumerator WaitForClip()
        {
            yield return new WaitForSeconds(enabledSFX.GetClip().length);
            staticSource.enabled = false;
        }
    }
}
