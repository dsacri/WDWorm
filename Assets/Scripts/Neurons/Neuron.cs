using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// single neuron that is visible within celegans
/// </summary>
public class Neuron : MonoBehaviour
{
    [SerializeField] protected Mapping mapping;
    [SerializeField] protected MeshRenderer meshRenderer;

    public string NeuronName { get; set; }
    public Color MaterialColor { get => materialColor; set => materialColor = value; }

    private Color materialColor;

    /// <summary>
    /// next color can be set outside the main thread
    /// </summary>
    private Color nextAppearenceColor;
    /// <summary>
    /// next scale can be set outside the main thread
    /// </summary>
    private Vector3 nextAppearenceScale;

    private void Awake()
    {
        MaterialColor = meshRenderer.material.color;
    }

    /// <summary>
    /// efficient way of changing appearence of multiple neurons at once
    /// values = all neurons at a single time (t)
    /// </summary>
    public static void ChangeToAppearence(float[] values, Dictionary<int, Neuron> indexToNeuronDictionary, Neuron[] neuronArray)   //
    {
        Parallel.For(0, values.Length, i => {
            CalculateNextAppearence(i, values[i], indexToNeuronDictionary);
        });

        foreach (Neuron neuron in neuronArray)
        {
            if (neuron != null)
                neuron.ShowAppearence();
        }
    }

    /// <summary>
    /// can be called outside the main thread
    /// </summary>
    public static void CalculateNextAppearence(int index, float value, Dictionary<int, Neuron> indexToNeuronDictionary)
    {
        Neuron neuron = indexToNeuronDictionary[index];

        if (neuron == null)
            return;
        
        if (value > 1)
            value = 1;

        if (value < 0.5f)
            neuron.nextAppearenceColor = neuron.MaterialColor * (value + 0.5f);
        else
            neuron.nextAppearenceColor = neuron.MaterialColor;

        if (value < 0.1f)
            value = 0.1f;

        neuron.nextAppearenceScale = Vector3.one * value / 30;
    }

    /// <summary>
    /// apply saved color and scale
    /// has to be called in main thread
    /// </summary>
    public void ShowAppearence()
    {
        nextAppearenceColor.a = meshRenderer.material.color.a;
        meshRenderer.material.color = nextAppearenceColor;
        transform.localScale = nextAppearenceScale;
    }

    /// <summary>
    /// change appearence of single neuron
    /// value should be between 0 - 1
    /// has to be called in main thread
    /// </summary>
    public void ChangeToAppearence(float value)
    {
        if (value > 1)
            value = 1;

        materialColor.a = meshRenderer.material.color.a;

        if (value < 0.5f)
            meshRenderer.material.color = Util.ColorMultiply(MaterialColor, value + 0.5f);
        else
            meshRenderer.material.color = MaterialColor;

        if (value < 0.1f)
            value = 0.1f;

        transform.localScale = Vector3.one * value / 30;
    }

}
