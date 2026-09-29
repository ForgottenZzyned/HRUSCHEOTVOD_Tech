using DG.Tweening;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TypewriterText : Singleton<TypewriterText>
{
    [Header("References")]
    [SerializeField] private SFXBank typeBank;

    [Header("Typing Settings")]
    [SerializeField] private float characterDelay = 0.03f;
    [SerializeField] private float punctuationPause = 0.5f;
    [SerializeField] private int soundEveryNCharacters = 2;

    private Coroutine typingRoutine;
    private string currentFullText;

    public bool IsTyping { get; private set; }

    public void Type(string text, TextMeshProUGUI textField)
    {
        StopCurrentTyping();

        currentFullText = text;
        typingRoutine = StartCoroutine(TypeRoutine(text, textField));
    }

    public void SkipTyping(TextMeshProUGUI textField)
    {
        if (!IsTyping) return;

        StopCurrentTyping();

        textField.text = currentFullText;
        textField.maxVisibleCharacters = currentFullText.Length;
        IsTyping = false;
    }

    public void Clear(TextMeshProUGUI textField)
    {
        textField.text = "";
    }
    public Tween FadeOut(float duration, TextMeshProUGUI textField)
    {
        return textField.DOFade(0f, duration);
    }
    public Tween FadeIn(float duration, TextMeshProUGUI textField)
    {
        return textField.DOFade(1f, duration);
    }


    private void StopCurrentTyping()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);
    }

    private IEnumerator TypeRoutine(string fullText, TextMeshProUGUI textField)
    {
        IsTyping = true;

        textField.text = fullText;
        textField.maxVisibleCharacters = 0;
        int visibleCharacterCount = 0;

        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];
            if (c == '<')
            {
                int closingTagIndex = fullText.IndexOf('>', i);

                if (closingTagIndex != -1)
                {
                    i = closingTagIndex;
                    continue;
                }
            }

            visibleCharacterCount++;
            textField.maxVisibleCharacters = visibleCharacterCount;

            if (visibleCharacterCount % soundEveryNCharacters == 0)
            {
                PlayTypeSound();
            }

            float delay = characterDelay;

            if (IsPunctuation(c))
            {
                delay += punctuationPause;
            }
            yield return new WaitForSeconds(delay);
        }
        IsTyping = false;
    }

    private bool IsPunctuation(char c)
    {
        return c == '.' || c == ',' || c == '!' || c == '?' || c == ':';
    }

    private void PlayTypeSound()
    {
        if (typeBank != null)
        {
            SFXManager.Instance.Play(typeBank, Vector3.zero, 0);
        }
    }
}