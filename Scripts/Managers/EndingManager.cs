using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
[RequireComponent(typeof(AudioSource))]
public class EndingManager : Singleton<EndingManager>
{
    public List<string> phrases = new();

    [Header("References")]
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera vcam;
    public TMP_Text phrasesText;
    [HideInInspector] public AudioSource audioSource;

    [Header("Targets")]
    public Transform window;
    public Vector3 inRoomOffset = new Vector3(0, 1, -10);
    public Vector3 approachOffset = new Vector3(0, 1, -5);
    public Vector3 insideOffset = new Vector3(0, 1, 2);
    public Vector3 outsideOffset = new Vector3(0, 1, 20);

    [Header("Timings (seconds)")]
    public float startTime = 45f;
    public float enterTime = 60f;
    public float exitTime = 75f;
    public float endTime = 90f;

    public static bool endingStarted = false;
    public bool endingEnded = false;

    private Sequence cutsceneSequence;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        endingEnded = false;
        endingStarted = false;
    }
    private void Update()
    {
        CheckCutscene();
    }
    private bool CheckCutscene()
    {
        float t = audioSource.time;

        if (t >= startTime && !endingStarted)
        {
            StartCutscene();
        }
        return endingStarted;
    }
    public void StartCutscene()
    {
        SetStarted(true);
        PlayerManager.Instance.playerObject
            .GetComponentInChildren<CameraMovement>()
            .enabled = false;
        Flashlight light = PlayerManager.Instance.playerObject
            .GetComponentInChildren<Flashlight>();
        if (light.UsingFlashLight()) light.SwitchFlashlight();
        light.enabled = false;

        vcam.transform.position = mainCamera.transform.position;
        vcam.transform.rotation = mainCamera.transform.rotation;

        vcam.PreviousStateIsValid = false;
        vcam.Follow = null;
        vcam.LookAt = null;
        vcam.Priority = 20;

        HUDManager.Instance.SetAlphaTo(0, 1.5f);

        CreateCutsceneSequence();
    }
    private void CreateCutsceneSequence()
    {
        cutsceneSequence?.Kill();
        Vector3 approachPosition = window.position + approachOffset;
        Vector3 insidePosition = window.position + insideOffset;
        Vector3 outsidePosition = window.position + outsideOffset;
        Vector3 directionToWindow = window.position - approachPosition;
        directionToWindow.y = 0;

        Quaternion approachRotation = Quaternion.LookRotation(directionToWindow);
        Quaternion outsideRotation = Quaternion.LookRotation(-window.forward);

        cutsceneSequence = DOTween.Sequence();
        // 45 -> 60
        cutsceneSequence.Append(
            vcam.transform.DOMove(approachPosition, enterTime - startTime)
                .SetEase(Ease.InOutSine));
        cutsceneSequence.Join(
            vcam.transform.DORotateQuaternion(
                approachRotation,
                enterTime - startTime)
            .SetEase(Ease.InOutSine));
        // 60 -> 75
        cutsceneSequence.Append(
            vcam.transform.DOMove(
                insidePosition,
                exitTime - enterTime
            )
            .SetEase(Ease.InOutSine)
        );
        cutsceneSequence.Join(
            vcam.transform.DORotateQuaternion(
                outsideRotation,
                endTime - exitTime)
            .SetEase(Ease.InOutSine));
        // 75 -> 90
        cutsceneSequence.Append(
            vcam.transform.DOMove(
                outsidePosition,
                endTime - exitTime - 1f
            )
            .SetEase(Ease.InOutSine)
        );
        cutsceneSequence.AppendCallback(() =>
        {
            EndCutscene();
        });
    }
    private void EndCutscene()
    {
        if (endingEnded)
            return;
        endingEnded = true;
        AnalyticsManager.RunCompleted();
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            ScreenFader.Instance.FadeToBlack(0.5f);
        });
        seq.AppendInterval(1.5f);
        seq.AppendCallback(() =>
        {
            IntroSequenceManager.Instance.ShowPhrases(phrases);
        });
        seq.AppendInterval(17.5f);
        seq.AppendCallback(() =>
        {
            endingStarted = false;
            LoadMenuScene();
        });
    }
    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    public void SetStarted(bool value) => endingStarted = value;
    public bool IsEnding()
    {
        return audioSource.time >= 10;
    }
    private void OnDisable()
    {
        cutsceneSequence?.Kill();
    }
}