using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroSequenceManager : Singleton<IntroSequenceManager>
{
    [SerializeField] TextMeshProUGUI controlsText;
    [SerializeField] TextMeshProUGUI phraseText;
    [SerializeField] TextMeshProUGUI pressAnyText;

    [TextArea]
    [SerializeField] private string controls;
    [TextArea]
    [SerializeField] private string pressAny;
    [TextArea]
    [SerializeField] private string[] phrases;

    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float textTypeDuration = 2f;
    [SerializeField] private float textFadeDuration = 0.5f;

    private ScreenFader screenFader;
    private TypewriterText typewriter;

    public void OnEnable()
    {
        typewriter = TypewriterText.Instance;
        screenFader = ScreenFader.Instance;
        StartIntro();
    }

    public void StartIntro()
    {
        VolumeManager.Instance.SetMasterVolume(0, 0f);
        StartCoroutine(IntroRoutine());
        PauseMenu.canPause = false;
    }

    private IEnumerator IntroRoutine()
    {
        yield return screenFader.FadeToBlack(fadeDuration).WaitForCompletion();

        yield return ShowText(controls,controlsText);
        yield return WaitForInput();
        typewriter.Clear(controlsText);
        typewriter.Clear(phraseText);
        
        foreach (string phrase in phrases)
        {
            yield return ShowText(phrase, phraseText);
            yield return new WaitForSeconds(1f);
        }
        yield return typewriter.FadeOut(1f,phraseText).WaitForCompletion();
        typewriter.Clear(phraseText);
        VolumeManager.Instance.SetMasterVolume(1f, 2f);
        VolumeManager.Instance.SetMusicVolume(1f, 2f);
        yield return screenFader.FadeFromBlack(fadeDuration).WaitForCompletion();
        PauseMenu.canPause = true;
    }
    public void ShowPhrases(List<string> phrases)
    {
        StartCoroutine(TypePhrases(phrases));
    }

    private IEnumerator TypePhrases(List<string> phrases)
    {
        typewriter.FadeIn(1f, phraseText);
        foreach (string phrase in phrases)
        {
            yield return ShowText(phrase, phraseText, false);
            yield return new WaitForSeconds(1f);
        }
        typewriter.FadeOut(1f, phraseText);
    }

    private IEnumerator ShowText(string text, TextMeshProUGUI textField, bool canBeSkipped = true)
    {
        typewriter.Type(text, textField);
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (Input.anyKeyDown && canBeSkipped)
            {
                if (typewriter.IsTyping)
                {
                    typewriter.SkipTyping(textField);
                    break;
                }
            }
            if (!typewriter.IsTyping)
            {
                break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitForInput()
    {
        yield return ShowText(pressAny, pressAnyText);
        while (true)
        {
            if (Input.anyKeyDown)
            {
                typewriter.Clear(pressAnyText);
                break;
            }
            yield return null;
        }
    }
}
