using UnityEngine;

/// <summary>
/// all colors that are not defined in materials or shaders
/// </summary>
[CreateAssetMenu(fileName = "Colors", menuName = "ScriptableObjects/ColorsScriptableObject", order = 1)]
public class ColorsScriptableObject : ScriptableObject
{
    //important: change the values in unity inspector, not in this file

    [field: SerializeField] public Color DefaultNeuronGroup { get; private set; }
    [field: SerializeField] public Color[] NeuronGroups { get; private set; }
    [field: SerializeField] public Color ButtonErrorColor { get; private set; }
    [field: SerializeField] public Color ButtonDefaultColor { get; private set; }
    [field: SerializeField] public Color ButtonDefaultInvertColor { get; private set; }
    [field: SerializeField] public Color ButtonHoverColor { get; private set; }
    [field: SerializeField] public Color ButtonClickColor { get; private set; }
    [field: SerializeField] public Color ButtonSelectedNeutral { get; private set; }
    [field: SerializeField] public Color ButtonSelectedInvertNeutral { get; private set; }
    [field: SerializeField] public Color ButtonSelectedFirst { get; private set; }
    [field: SerializeField] public Color ButtonSelectedInvertFirst { get; private set; }
    [field: SerializeField] public Color ButtonSelectedSecond { get; private set; }
    [field: SerializeField] public Color ButtonSelectedInvertSecond { get; private set; }
    [field: SerializeField] public Color[] ButtonMarked { get; private set; }
    [field: SerializeField] public Color[] ButtonMarkedInvert { get; private set; }
    [field: SerializeField] public Color[] LineGraphColors { get; private set; }
}
