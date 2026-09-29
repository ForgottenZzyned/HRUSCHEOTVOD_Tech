using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;

public class OptionsManager : Singleton<OptionsManager>
{
    public GameObject optionsPanel;
    public TMP_Text currSensText;
    public TMP_Text sensText;
    private bool inOptions = false;
    private void Start()
    {
        CursorManager.SetCursor(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptions();
        }
    }
    public void ToggleOptions()
    {
        inOptions = !inOptions;
        Time.timeScale = inOptions ? 0.2f : 1f;
        PlayerManager.Instance.playerObject.GetComponentInChildren<CameraMovement>().enabled = !inOptions;
        optionsPanel.SetActive(inOptions);
        CursorManager.SetCursor(inOptions);
    }
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneController.Instance.LoadScene(0);
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
        }
    }
}
