using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class VolumeManager : Singleton<VolumeManager>
{
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    public AudioMixer mainMixer;

    private float checkTiming = 0.25f;
    private float lastChecked;

    private void Start()
    {
        if (masterSlider == null) SetValueToSliders();
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", masterSlider.value);
        masterSlider.value = masterVolume;
        SetMixerVolume(masterVolume, "MasterVolume");
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxSlider.value);
        sfxSlider.value = sfxVolume;
        SetMixerVolume(sfxVolume, "SFXVolume");
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicSlider.value);
        musicSlider.value = musicVolume;
        SetMixerVolume(musicVolume, "MusicVolume");
    }
    private void Update()
    {
        if (masterSlider == null) return;
        if(lastChecked +checkTiming < Time.time)
        {
            lastChecked = Time.time;
            SetMixerVolume(masterSlider.value,"MasterVolume");
            SetMixerVolume(sfxSlider.value, "SFXVolume");
            SetMixerVolume(musicSlider.value, "MusicVolume");
        }
    }
    private void SetValueToSliders()
    {
        SetSliderValue(masterSlider, "MasterVolume");
        SetSliderValue(sfxSlider, "SFXVolume");
        SetSliderValue(musicSlider, "MusicVolume");
    }
    public void SetMasterVolume(float percent,float dur)
    { 
        DOTween.Kill("MasterChange");
        mainMixer.GetFloat("MasterLowpass", out float currFreq);
        DOTween.To(
            () => currFreq,
            x =>
            {
                currFreq = x;
                mainMixer.SetFloat("MasterLowpass", x);
            },
            GetLowpassFreq(percent),
            dur)
        .SetId("MasterChange");
    }
    public void SetSFXVolume(float percent, float dur)
    {
        DOTween.Kill("SFXChange");
        mainMixer.GetFloat("SFXLowpass", out float currFreq);
        DOTween.To(
            () => currFreq,
            x =>
            {
                currFreq = x;
                mainMixer.SetFloat("SFXLowpass", x);
            },
            GetLowpassFreq(percent),
            dur)
        .SetId("SFXChange");
    }
    public void SetMusicVolume(float percent, float dur)
    {
        DOTween.Kill("MusicChange");
        mainMixer.GetFloat("MusicLowpass", out float currFreq);
        DOTween.To(
            () => currFreq,
            x =>
            {
                currFreq = x;
                mainMixer.SetFloat("MusicLowpass", x);
            },
            GetLowpassFreq(percent),
            dur)
        .SetId("MusicChange");
    }
    private void SetSliderValue(Slider slider, string mixerGroup)
    {
        if(slider.maxValue != 1)slider.maxValue = 1;
        mainMixer.GetFloat(mixerGroup, out float db);
        float value = Mathf.Pow(10f, (db - 5f) / 20f);
        slider.value = value;
    }
    private void SetMixerVolume(float sliderValue, string mixerGroup)
    {
        sliderValue = Mathf.Max(sliderValue, 0.0001f);
        float db = Mathf.Log10(sliderValue) * 20f + 5f;
        mainMixer.SetFloat(mixerGroup, db);
        PlayerPrefs.SetFloat(mixerGroup, sliderValue);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
    private float GetLowpassFreq(float percent)
    {
        return Mathf.Lerp(10f, 22000f, percent);
    }
}
