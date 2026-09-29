using Cinemachine;
using DG.Tweening;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
[System.Serializable]
public class StartingMusicProfile
{
    public AudioClip startClip;
    public AudioClip mainClip;
}
public class MainMenuManager : Singleton<MainMenuManager>
{
    public CinemachineVirtualCamera vcam;
    public StartingMusicProfile musicProfile;
    public AudioMixerGroup musicMixerGroup;
    public AudioSource startSource;
    public AudioSource mainSource;
    public DepthText gameNameText;
    public string gameName;
    public List<MainMenuButton> buttons = new();
    public Disclaimer disclaimer;
    public bool isHard = false;
    public static bool isWarned = false;
    private bool transiting = false;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLoad;
    }
    private void Start()
    {
        gameNameText.SetText(gameName);
    }
    public void OnLoad(Scene scene, LoadSceneMode loadMode)
    {
        HardmodeManager.SetHard(false);
        StartCoroutine(FadeRoutine());
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            AnalyticsManager.FirstLaunch();
            PlayerPrefs.SetInt("FirstLaunch", 1);
        }
    }
    private IEnumerator FadeRoutine()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(1f);
        if (!isWarned)
            yield return disclaimer.ShowDisclaimer().WaitForCompletion();
        yield return new WaitForSeconds(1.5f);
        if (!isWarned)
        {
            yield return disclaimer.HideDisclaimer().WaitForCompletion();
            isWarned = true;
        }
        yield return new WaitForSeconds(0.5f);
        ScreenFader.Instance.FadeFromBlack(5f);
        StartMusic();
        yield return new WaitForSeconds(4f);
        buttons.ForEach(button => button.ToggleVisibility());
    }
    public void TransitToNextScene()
    {
        if (transiting) return;
        transiting = true;
        AnalyticsManager.RunStarted();
        buttons.ForEach(button => button.ToggleVisibility(1.5f));
        ChangeFOV(10f, 5f);
        ScreenFader.Instance.FadeToBlack(4.5f);
        startSource.DOFade(0f,4.5f);
        mainSource.DOFade(0f,4.5f);
    }
    public void StartMusic()
    {
        StartCoroutine(PlayStartingMusic());
    }
    private IEnumerator PlayStartingMusic()
    {
        InitSource(startSource,musicProfile.startClip, 0.2f,4f);
        yield return new WaitForSeconds(musicProfile.startClip.length-15.5f);
        InitSource(mainSource, musicProfile.mainClip, 0.2f,0.2f);
        mainSource.loop = true;
    }
    private void InitSource(AudioSource source,AudioClip clip, float targetVolume, float time)
    {
        source.spatialBlend = 0f;
        source.clip = clip;
        source.volume = 0;
        source.DOFade(targetVolume, time);
        source.Play();
        source.outputAudioMixerGroup = musicMixerGroup;
    }
    public void ChangeFOV(float targetFOV, float duration)
    {
        DOTween.To(
            () => vcam.m_Lens.FieldOfView,
            x => {
                var lens = vcam.m_Lens;
                lens.FieldOfView = x;
                vcam.m_Lens = lens;
            },
            targetFOV,
            duration
        ).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            if(isHard)SceneController.Instance.LoadScene(2);
            else SceneController.Instance.LoadScene(1);
        }); 
    }
    public void ChangeMode()
    {
        isHard = !isHard;
        HardmodeManager.SetHard(isHard);
    }
    public void ExitButton()
    {
        Application.Quit();
    }

}
