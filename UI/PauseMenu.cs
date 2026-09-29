using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    private bool inOptions = false;
    public GameObject optionsPanel;
    public CanvasGroup panelCanvGroup;
    public TMP_Text currSensText;
    public TMP_Text sensText;
    public static bool IsPaused = false;
    public static bool canPause = true;
    public MainMenuButton pauseText;
    private void Start()
    {
        currSensText.text = $"Current: {PlayerPrefs.GetFloat("MouseSensitivity",100f)}";
        optionsPanel.SetActive(true);
        panelCanvGroup = optionsPanel.GetComponent<CanvasGroup>();
        IsPaused = false;
        inOptions = false;
        panelCanvGroup.alpha = 0f;
        CursorManager.SetCursor(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !EndingManager.Instance.IsEnding())
        {
            ToggleOptions();
        }
    }
    public void ToggleOptions()
    {
        if (!canPause && !inOptions) return;
        inOptions = !inOptions;
        IsPaused = inOptions;
        Time.timeScale = inOptions ? 0.1f : 1f;
        PlayerManager.Instance.playerObject.GetComponentInChildren<CameraMovement>().enabled = !inOptions;
        pauseText.ToggleVisibility();
        SetPanelAlpha(inOptions);
        CursorManager.SetCursor(inOptions);
    }
    private void SetPanelAlpha(bool active)
    {
        DOTween.Kill("PanelAlpha");
        panelCanvGroup.DOFade(active ? 1f : 0f, active ? 0.4f : 0.3f)
            .SetEase(active ? Ease.InQuad : Ease.OutQuad)
            .SetId("PanelAlpha").OnUpdate(() =>
            {
                float timeScale = Mathf.Lerp(1f, 0f, panelCanvGroup.alpha);

                Time.timeScale = timeScale;
            }).
            SetUpdate(UpdateType.Normal, true);
    }
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }
    public void OnSensButtonPress()
    {
        string raw = sensText.text;

        StringBuilder cleaned = new StringBuilder();

        foreach (char c in raw)
        {
            if (char.IsDigit(c) || c == '.' || c == ',')
                cleaned.Append(c);
        }
        string input = cleaned.ToString().Replace(',', '.');
        bool success = float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result);
        if (success)
        {
            CameraMovement.Instance.mouseSensitivity = result;
            currSensText.text = $"Current: {result}";
            PlayerPrefs.SetFloat("MouseSensitivity", result);
            PlayerPrefs.Save();
        }
    }
}