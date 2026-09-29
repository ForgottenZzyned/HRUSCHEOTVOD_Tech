using TMPro;
using UnityEngine;

public class FloatInputValidator : MonoBehaviour
{
    public TMP_InputField inputField;
    void Awake()
    {
        inputField.onValidateInput += Validate;
    }
    private char Validate(string text, int charIndex, char addedChar)
    {
        if (char.IsDigit(addedChar))
            return addedChar;
        if ((addedChar == '.' || addedChar == ',') && !text.Contains(".") && !text.Contains(","))
            return addedChar;
        return '\0';
    }
}