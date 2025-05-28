using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// checks if text is parsable to double and changes to error colors if not
/// </summary>
public class ParseToDoubleCheck : MonoBehaviour
{
    [SerializeField] private Graphic[] graphics;
    [SerializeField] private TMP_InputField inputField;
    
    public void OnValueChanged()            //gui
    {
        if (double.TryParse(inputField.text, out double d))
        {
            ColorGraphics(true);
        }
        else
        {
            ColorGraphics(false);
        }
    }

    private void ColorGraphics(bool normal)
    {
        foreach (Graphic graphic in graphics)
        {
            graphic.color = normal ? ScriptableObjectUtil.ColorsScriptableObject.ButtonDefaultColor : ScriptableObjectUtil.ColorsScriptableObject.ButtonErrorColor;
        }
    }

}
