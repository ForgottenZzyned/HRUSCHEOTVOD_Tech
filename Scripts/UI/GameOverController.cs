using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GameOverController : Singleton<GameOverController>
{
    public TextMeshProUGUI killedText;
    public List<MainMenuButton> buttons;
    public string killedString;
    public bool isGameOver = false;
    private void Update()
    {
        if(isGameOver && !Cursor.visible) CursorManager.SetCursor(true);
    }
    public void GameOver()
    {
        VolumeManager.Instance.SetMasterVolume(0f,1f);
        StartCoroutine(ShowPanel());
    }
    public void MainMenuButton()
    {
        PlayerPrefs.Save();
        ShowButtons();
        DOVirtual.DelayedCall(0.5f, () =>
        {
            SceneController.Instance.LoadScene(0);
        });
    }
    public void RestartButton()
    {
        PlayerPrefs.Save();
        ShowButtons();
        DOVirtual.DelayedCall(0.5f, () =>
        {
            SceneController.Instance.ReloadCurrentScene();
        });
    }
    private IEnumerator ShowPanel()
    {
        ScreenFader.Instance.FadeToBlack(1f);
        yield return new WaitForSeconds(3f);
        SetText();
        CursorManager.SetCursor(true);
        yield return new WaitForSeconds(4f);
        ShowButtons();
        isGameOver = true;
    }
    private void ShowButtons()
    {
        foreach (var button in buttons)
        {
            button.ToggleVisibility();
        }
    }
    private void SetText()
    {
        TypewriterText.Instance.FadeIn(0f, killedText);
        TypewriterText.Instance.Type(killedString,killedText);
    }
}
