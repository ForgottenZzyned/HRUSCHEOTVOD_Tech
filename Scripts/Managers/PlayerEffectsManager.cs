using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PlayerEffectsManager : Singleton<PlayerEffectsManager>
{
    public Volume volume;
    public CustomPassVolume customPass;
    public AudioSource source;
    public Image screamerImage;
    public static bool isScreamer = false;
    private Coroutine glitchCoroutine;
    private Sequence seq;
    private Vignette vignette;
    private float baseVignetteIntensity = 0.35f;
    [Header("Fracture Screamer Settings")]
    public Material glitchMat;
    public Sprite fractureBase;
    public Sprite fractureScream;
    public Color fStartCol;
    public Color fEndCol;
    [Header("Ephemer Screamer Settings")]
    public Sprite ephemerScream;
    public Color eStartCol;
    public Color eSecondaryCol;
    [Header("Glitch Effect Settings")]
    public AudioClip glitchSound;
    public Color mainCol;
    [Header("Watcher Settings")]
    public Sprite notMoveSprite;
    [Header("Ressurection Settings")]
    public bool isRessurection = false;
    public SFXBank ressurectionSFX;
    public SFXBank ressurectionSFX2;
    [Header("Walker Glitch")]
    public Material glitchWalkerMat;
    public SFXBank walkerGlitchSFX;
    private Sequence walkerSeq;
    private void Start()
    {
        volume.profile.TryGet(out vignette);
        baseVignetteIntensity = vignette.intensity.value;
        FullScreenCustomPass glitchPass = customPass.customPasses[2] as FullScreenCustomPass;
        FullScreenCustomPass walkerGlitchPass = customPass.customPasses[3] as FullScreenCustomPass;
        glitchMat = new Material(glitchPass.fullscreenPassMaterial);
        glitchPass.fullscreenPassMaterial = glitchMat;
        glitchWalkerMat = new Material(walkerGlitchPass.fullscreenPassMaterial);
        walkerGlitchPass.fullscreenPassMaterial = glitchWalkerMat;
        screamerImage.enabled = false;
    }
    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.G))
        {
            TriggerFractureScream();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            TriggerEphemerScream();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            TriggerGlitch(0.05f, 0.05f, -0.1f, 1f, 0.2f);
        }
        #endif
    }
    public void TriggerVignette(float fadeInTime, float fadeOutTime, Color col, float maxIntense = 0.6f)
    {
        volume.profile.TryGet(out vignette);

        vignette.intensity.overrideState = true;
        vignette.color.overrideState = true;

        float baseIntensity = baseVignetteIntensity;
        float maxIntensity = maxIntense;

        Color baseColor = vignette.color.value;

        seq?.Kill();
        seq = DOTween.Sequence();

        seq.Append(
            DOTween.To(
                () => vignette.intensity.value,
                x => vignette.intensity.value = x,
                maxIntensity,
                fadeInTime
            ).SetEase(Ease.OutQuad)
        );
        seq.Join( 
            DOTween.To(
                () => vignette.color.value,
                x => vignette.color.value = x,
                col,
                fadeInTime
            ).SetEase(Ease.OutQuad)
        );
        seq.Append(
            DOTween.To(
                () => vignette.intensity.value,
                x => vignette.intensity.value = x,
                baseIntensity,
                fadeOutTime
            ).SetEase(Ease.InQuad)
        );
        seq.Join(
            DOTween.To(
                () => vignette.color.value,
                x => vignette.color.value = x,
                baseColor,
                fadeOutTime
            ).SetEase(Ease.InQuad)
        );
    }
    public void TriggerGlitch(float fadeIn = 0, float fadeOut = 0,float endValue = 1f,float dur = 0.2f, float staticVol = 0.2f)
    {
        if (isScreamer || isRessurection) return;
        if (glitchCoroutine != null)StopCoroutine(glitchCoroutine);
        source.volume = staticVol;
        glitchCoroutine = StartCoroutine(GlitchFullscreen(fadeIn, fadeOut, endValue, dur));
    }
    public void TriggerWalkerGlitch(float fadeIn = 0, float fadeOut = 0, float endValue = 1f, float dur = 0.2f)
    {
        if (isScreamer || isRessurection) return;
        if (glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
            ClearGlitch();
        }
            
        WalkerGlitchFullscreen(fadeIn, fadeOut, endValue, dur);
    }
    private void WalkerGlitchFullscreen(float fadeIn, float fadeOut, float endValue, float dur)
    {
        if (isRessurection) return;
        SFXManager.Instance.Play(walkerGlitchSFX,Vector3.zero, 0,int.MaxValue);
        SFXManager.Instance.Play(walkerGlitchSFX,Vector3.zero, 0,int.MaxValue);
        DOTween.Kill("GlitchEffect");
        walkerSeq?.Kill();
        walkerSeq = DOTween.Sequence()/*.SetId("GlitchEffect")*/;
        walkerSeq.Append(DOTween.To(
            () => 0f,
            x => glitchWalkerMat.SetFloat("_Alpha", x),
            endValue,
            fadeIn
        )/*.SetId("GlitchEffect")*/);
        walkerSeq.AppendInterval(dur);
        walkerSeq.Append(DOTween.To(
            () => endValue,
            x => glitchWalkerMat.SetFloat("_Alpha", x),
            0f,
            fadeOut
        )/*.SetId("GlitchEffect")*/);
        seq.OnKill(() =>
        {
            glitchWalkerMat.SetFloat("_Alpha", 0f);
        });
    }
    private IEnumerator GlitchFullscreen(float fadeIn, float fadeOut, float endValue, float dur)
    {
        if (isRessurection) yield break;
        source.clip = glitchSound;
        source.Play();
        DOTween.Kill("GlitchEffect");
        Sequence seq = DOTween.Sequence().SetId("GlitchEffect");
        seq.Append(DOTween.To(
            () => 0f,
            x => glitchMat.SetFloat("_Alpha", x),
            endValue,
            fadeIn
        )).SetId("GlitchEffect");
        seq.AppendInterval(dur);
        seq.Append(DOTween.To(
            () => endValue,
            x => glitchMat.SetFloat("_Alpha", x),
            0f,
            fadeOut
        )).SetId("GlitchEffect");
        seq.OnKill(() =>
        {
            glitchMat.SetFloat("_Alpha", 0f);
            source.clip = null;
        });
        source.clip = null;
    }
    public void TriggerNotMoveImage()
    {
        if (isScreamer || isRessurection) return;
        if (glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
            ClearGlitch();
        }
        StartCoroutine(NotMove());
    }
    public void TriggerFractureScream()
    {
        //if (PauseMenu.IsPaused) return;
        if (isScreamer || isRessurection) return;
        if (!PlayerManager.Instance.IsPlayerAlive()) return;
        if (glitchCoroutine != null) StopCoroutine(glitchCoroutine);
        StartCoroutine(FractureScreamer());
    }
    public void TriggerEphemerScream()
    {
        //if (PauseMenu.IsPaused) return;
        if (isScreamer || isRessurection) return;
        if (!PlayerManager.Instance.IsPlayerAlive()) return;
        if (glitchCoroutine != null) StopCoroutine(glitchCoroutine);
        VolumeManager.Instance.SetMusicVolume(0f,0f);
        StartCoroutine(EphemerScreamer());
    }
    public void TriggerRessurect()
    {
        Ressurect();
    }
    private IEnumerator EphemerScreamer()
    {
        PauseMenu.canPause = false;
        isScreamer = true;
        glitchMat.SetFloat("_Alpha", 1f);
        glitchMat.SetColor("_NoiseColor", eStartCol);
        DOTween.Kill("GlitchEffect");
        screamerImage.sprite = ephemerScream;
        screamerImage.enabled = true;
        RectTransform rt = screamerImage.GetComponent<RectTransform>();
        yield return new WaitForSeconds(3f);
        rt.DOShakeAnchorPos(
            duration: 0.25f,
            strength: 20,
            vibrato: 600,
            randomness: 100,    
            fadeOut: false
        ).SetId("GlitchEffect");
        rt.DOScale(rt.localScale * 2.5f,0.2f).SetEase(Ease.OutBack).SetId("GlitchEffect");
        SFXBank screamSfx = Resources.Load<SFXBank>("Audio/Entities/Ephemer/Scream/Scream");
        SFXManager.Instance.PlayScream(screamSfx, Vector3.zero, 0);
        glitchMat.SetColor("_NoiseColor", eSecondaryCol * 4f);
        yield return new WaitForSeconds(0.1f);
        glitchMat.SetColor("_NoiseColor", eStartCol * 4f);
        yield return new WaitForSeconds(0.05f);
        glitchMat.SetColor("_NoiseColor", eSecondaryCol * 4f);
        
        yield return new WaitForSeconds(0.1f);
        screamerImage.enabled = false; 
        rt.localScale = rt.localScale / 2.5f;
        glitchMat.SetColor("_NoiseColor", mainCol);
        glitchMat.SetFloat("_Alpha", 0f);
        PlayerManager.Instance.HitPlayer(2000, "Ephemer");
        isScreamer = false;
        if (PlayerManager.Instance.IsPlayerAlive())
        {
            PauseMenu.canPause = true;
            VolumeManager.Instance.SetMusicVolume(1f, 1f);
        }
        
    }
    private IEnumerator FractureScreamer()
    {
        DOTween.Kill("GlitchEffect");
        PauseMenu.canPause = false;
        isScreamer = true;
        source.clip = glitchSound;
        source.Play();
        glitchMat.SetColor("_NoiseColor", fStartCol);
        DOTween.To(
            () => 0f,
            x => glitchMat.SetFloat("_Alpha", x),
            1f,
            0f
        ).SetId("GlitchEffect");
        yield return new WaitForSeconds(0.5f);
        screamerImage.sprite = fractureBase;
        SFXBank preSfx = Resources.Load<SFXBank>("Audio/Entities/Fracture/PreScream/PreScrim");
        SFXManager.Instance.PlayScream(preSfx, Vector3.zero, 0);
        screamerImage.enabled = true;
        RectTransform rt = screamerImage.GetComponent<RectTransform>();
        rt.DOShakeAnchorPos(
            duration: 0.5f,
            strength: 5,
            vibrato: 400,
            randomness: 100,
            fadeOut: false
        );
        glitchMat.SetColor("_NoiseColor", fStartCol * 4f);
        yield return new WaitForSeconds(0.1f);
        glitchMat.SetColor("_NoiseColor", fEndCol * 4f);
        yield return new WaitForSeconds(0.1f);
        glitchMat.SetColor("_NoiseColor", fStartCol);
        DOTween.To(
            () => fStartCol,
            x => glitchMat.SetColor("_NoiseColor", x),
            fEndCol,
            0.05f
        ).SetEase(Ease.OutQuad)
         .SetId("GlitchEffect");
        yield return new WaitForSeconds(0.3f);
        screamerImage.sprite = fractureScream;
        rt.DOShakeAnchorPos(
            duration: 1,
            strength: 12,
            vibrato: 600,
            randomness: 100,
            fadeOut: false
        ).SetId("GlitchEffect");
        SFXBank screamSfx = Resources.Load<SFXBank>("Audio/Entities/Fracture/Scream/FractureScream");
        SFXManager.Instance.PlayScream(screamSfx, Vector3.zero, 0);
        yield return new WaitForSeconds(1f);
        screamerImage.enabled = false;
        yield return new WaitForSeconds(0.05f);
        SFXBank afterSfx = Resources.Load<SFXBank>("Audio/Entities/Fracture/AfterScream/AfterScream");
        SFXManager.Instance.PlayScream(afterSfx, Vector3.zero, 0);
        glitchMat.SetColor("_NoiseColor", fEndCol * 4f);
        yield return new WaitForSeconds(0.01f);
        glitchMat.SetColor("_NoiseColor", fStartCol * 4f);
        yield return new WaitForSeconds(0.1f);
        source.clip = null;
        DOTween.To(
            () => 1f,
            x => glitchMat.SetFloat("_Alpha", x),
            0f,
            0.05f
        ).SetEase(Ease.OutQuad)
        .SetId("GlitchEffect");
        PlayerManager.Instance.HitPlayer(35, "Fracture");
        glitchMat.SetColor("_NoiseColor", mainCol);
        isScreamer = false;
        if(PlayerManager.Instance.IsPlayerAlive())PauseMenu.canPause = true;
    }
    private IEnumerator NotMove()
    {
        DOTween.Kill("GlitchEffect");
        screamerImage.sprite = notMoveSprite;
        screamerImage.enabled = true;
        RectTransform rt = screamerImage.GetComponent<RectTransform>();
        rt.DOShakeAnchorPos(
            duration: 0.25f,
            strength: 50,
            vibrato: 1000,
            randomness: 100,
            fadeOut: false
        ).SetId("GlitchEffect");
        yield return new WaitForSeconds(0.45f);
        screamerImage.enabled = false;
    }
    private void Ressurect()
    {
        DOTween.Kill("GlitchEffect");
        isRessurection = true;
        source.clip = glitchSound;
        source.volume = 0.4f;
        source.Play();
        SFXManager.Instance.Play(ressurectionSFX,Vector3.zero,0,int.MaxValue,true);
        SFXManager.Instance.Play(ressurectionSFX2, Vector3.zero,0,int.MaxValue,true);
        glitchMat.SetFloat("_IsRessurectionGlitch", 1f);
        glitchMat.SetFloat("_RessurectionProgress", 50f);
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(
            () => 0f,
            x => glitchMat.SetFloat("_Alpha", x),
            1,
            0f
        ));
        seq.Append(DOTween.To(
            () => glitchMat.GetFloat("_RessurectionProgress"),
            x => glitchMat.SetFloat("_RessurectionProgress", x),
            1,
            0.25f
        ));
        seq.AppendInterval(1.5f);
        seq.Append(DOTween.To(
            () => 0f,
            x => glitchMat.SetFloat("_RessurectionProgress", x),
            500,
            1.5f
        ).SetEase(Ease.InQuad)
        ).OnComplete(() =>
        {
            glitchMat.SetFloat("_RessurectionProgress", 1f);
            glitchMat.SetFloat("_IsRessurectionGlitch", 0f);
            glitchMat.SetFloat("_Alpha", 0);
            source.clip = null;
            source.volume = 0.2f;
            isRessurection = false;
        });
    }
    private void ClearGlitch()
    {
        glitchMat.SetFloat("_Alpha", 0f);
        glitchWalkerMat.SetFloat("_Alpha", 0f);
    }
}
