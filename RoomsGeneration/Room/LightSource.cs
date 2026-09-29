using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    [Header("Light")]
    public List<Light> sources = new();
    public float maxIntensity = 1f;
    [Header("Ambient SFX Banks")]
    public SFXBank ambientBank;
    public SFXBank flickerBank;
    public SFXBank shuttingOffBank;
    public SFXBank forceShuttingOffBank;
    [Header("Noise Flicker")]
    public float noiseAmount = 0.1f;
    public float noiseSpeed = 15f;

    [Header("Random Flicker")]
    public Vector2 flickerInterval = new Vector2(5f, 15f);

    [Header("Lamp Condition")]
    [Range(0, 1)]
    public float lampHealth = 1f;
    public float burnoutChance = 0.02f;

    [SerializeField] private List<Renderer> emissionRenderer;
    private List<Material> emmisionMaterial = new();

    private Coroutine flickerCor;
    private AudioSource ambientSource;
    private float baseIntensity;
    private Color baseEmission;
    private bool playingPattern;
    private bool stopLoop = false;

    private void OnEnable()
    {
        
        GetEmmisionMat();   
        baseEmission = emmisionMaterial[0].GetColor("_EmissiveColor");
        SetEmission(0f);
        baseIntensity = sources[0].intensity;
        ambientSource = gameObject.AddComponent<AudioSource>();
        SetSourcesState(false);
    }

    private void Update()
    {
        if (!playingPattern && sources[0].enabled)
        {
            ApplyNoise();
        }
    }

    private void ApplyNoise()
    {
        float noise = Mathf.PerlinNoise(Time.time * noiseSpeed, 0f);
        noise = (noise - 0.5f) * noiseAmount;

        ApplyIntensity(baseIntensity + noise, false);
    }

    public IEnumerator RandomFlickerLoop()
    {
        while (true)
        {
            if (stopLoop) break;
            yield return new WaitForSeconds(Random.Range(flickerInterval.x, flickerInterval.y));
            float chance = Random.value;
            if (chance < 0.3f)
            {
                yield return ShortFlicker();
            }
            if (chance < burnoutChance * (1f - lampHealth))
            {
                yield return Burnout();
                break;
            }
            lampHealth -= Random.Range(0.01f, 0.03f);
        }
    }

    private IEnumerator ShortFlicker()
    {
        playingPattern = true;

        float intensity = 0;
        SFXManager.Instance.Play(flickerBank, transform.position,1,10);
        for (int i = 0; i < Random.Range(2, 5); i++)
        {
            intensity = Random.Range(0.2f, maxIntensity);
            ApplyIntensity(baseIntensity * intensity);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));
        }
        ApplyIntensity(baseIntensity * 1f);

        playingPattern = false;
    }

    private IEnumerator Burnout(SFXBank customSFX = null)
    {
        playingPattern = true;

        float intensity = 0;
        SFXBank sfx = customSFX == null ? shuttingOffBank : customSFX;
        
        ambientSource.Stop();
        SFXManager.Instance.Play(flickerBank, transform.position, 1, 10);
        for (int i = 0; i < 10; i++)
        {
            intensity = Random.Range(0f, maxIntensity);
            ApplyIntensity(baseIntensity * intensity);
            
            yield return new WaitForSeconds(Random.Range(0.03f, 0.1f));
        }
        ApplyIntensity(baseIntensity * 0f);
        SFXManager.Instance.Play(sfx, transform.position, 1, 10);
        SetSourcesState(false);
        playingPattern = false;
        if(flickerCor != null)
            StopCoroutine(flickerCor);
        stopLoop = true;
    }

    private void ApplyIntensity(float intensity, bool isEmmision = true)
    {
        foreach (var light in sources)
        {
            light.intensity = intensity;
        }
        if(intensity < 10)SetEmission(intensity);
    }

    private void SetSourcesState(bool state)
    {
        foreach (var light in sources)
        {
            light.enabled = state;
        }
    }

    private void GetEmmisionMat()
    {
        foreach(var r in emissionRenderer)
        {
            emmisionMaterial.Add(r.material);
        }
    } 

    private void SetEmission(float value)
    {
        foreach (var m in emmisionMaterial)
        {
            m.SetColor("_EmissiveColor", baseEmission * value);
        }
    }

    public void ActivateSource()
    {
        if (AtmosphereManager.Instance.mainLight.intensity > 1000f) return;
        if(SFXManager.Instance != null)SFXManager.Instance.InitializeLoop(ambientBank, ambientSource);
        SetEmission(1f);
        SetSourcesState(true);
        flickerCor = StartCoroutine(RandomFlickerLoop());
    }

    public void ForceFlicker()
    {
        StartCoroutine(ShortFlicker());
    }

    public void ForceBurnout()
    {
        StartCoroutine(Burnout(forceShuttingOffBank));
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }
}
