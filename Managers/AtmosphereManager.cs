using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class AtmosphereManager : Singleton<AtmosphereManager>
{
    public Light mainLight;
    public Volume volume;
    [Header("Fog Settings")]
    public Fog fog;
    public float startFogAttenuation = 30f;
    public float maxFogAttenuation = 100f;
    public float minFogAttenuation = 13f;
    public float lightDegrValue = 5000f;

    private void Start()
    {
        volume.profile.TryGet(out fog);
        fog.meanFreePath.value = startFogAttenuation;
    }

    private void OnEnable()
    {
        GameEvents.OnDoorOpened += HandleDoorOpened;
    }

    private void OnDisable()
    {
        GameEvents.OnDoorOpened -= HandleDoorOpened;
    }

    public void HandleDoorOpened(Door openedDoor)
    {
        OnDoorOpen();
    }

    private void OnDoorOpen()
    {
        CalculateSunIntensity();
        CalculateFog();
    }

    private void CalculateSunIntensity()
    {
        int currRoom = RoomChainManager.Instance.currentRoom;
        float decrease;

        if (currRoom < 50)
            decrease = 2400f;
        else if (currRoom < 70)
            decrease = 450f;
        else if (currRoom < 90)
            decrease = 300f;
        else
            decrease = 150f;
        mainLight.DOKill();
        mainLight.DOIntensity(mainLight.intensity - decrease, 1f);
    }
    private void CalculateFog()
    {
        int currRoom = RoomChainManager.Instance.currentRoom;
        if (currRoom < 45)
            fog.meanFreePath.value -= ((startFogAttenuation-minFogAttenuation) / (45));
        else if (currRoom > 50)
            fog.meanFreePath.value += ((maxFogAttenuation - minFogAttenuation) / (70));
    }

    public void SetSunIntensity(float intensity, float time)
    {
        DOTween.To(() => mainLight.intensity, x => mainLight.intensity = x, intensity, time)
                    .SetEase(Ease.OutQuad);
    }
}
