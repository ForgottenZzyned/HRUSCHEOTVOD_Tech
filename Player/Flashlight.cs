using System.Collections;
using UnityEngine;

public class Flashlight : Singleton<Flashlight>
{
    public SFXBank onEnabledSFX;
    public SFXBank offEnabledSFX;
    private Light spotLight;
    private float baseIntensity;

    private Coroutine flickerCor;
    private void Start()
    {
        spotLight = GetComponent<Light>();
        spotLight.enabled = false;
        baseIntensity = spotLight.intensity;
    }
    private void Update()
    {
        if (spotLight.enabled && EnergyManager.Instance.CheckEnergy()) 
            EnergyManager.Instance.DrainEnergy();
        if (!EnergyManager.Instance.CheckEnergy() && spotLight.enabled) 
            SwitchFlashlight();
        if (Input.GetKeyDown(KeyCode.F) && !PauseMenu.IsPaused) 
            SwitchFlashlight();
    }
    public void ForceBreakFlashlight()
    {
        StartCoroutine(BreakFlashlight());
    }
    public void ForceFlickerFlashlight()
    {
        if(flickerCor != null) StopCoroutine(flickerCor);
        flickerCor = StartCoroutine(FlickerFlashlight());
    }
    public bool UsingFlashLight()
    {
        return spotLight.enabled;
    }
    private IEnumerator FlickerFlashlight()
    {
        float intensity;
        for (int i = 0; i < Random.Range(8, 14); i++)
        {
            intensity = Random.Range(0.4f, 1f);
            spotLight.intensity *= intensity;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.25f));
        }
        spotLight.intensity = baseIntensity;
    }
    private IEnumerator BreakFlashlight()
    {
        float intensity = 0;
        for (int i = 0; i < Random.Range(10, 15); i++)
        {
            intensity = Random.Range(0.2f, 1f);
            spotLight.intensity *= intensity;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        }
        spotLight.intensity *= 0f;
        spotLight.enabled = false;
    }
    public void SwitchFlashlight()
    {
        spotLight.enabled = !spotLight.enabled;
        if (spotLight.enabled) SFXManager.Instance.Play(onEnabledSFX,transform.position);
        else SFXManager.Instance.Play(offEnabledSFX, transform.position);
    }
}
