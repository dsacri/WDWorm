using TMPro;
using UnityEngine;

/// <summary>
/// gui prefab for a single neuron group
/// </summary>
public class GroupGui : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
    public string Name { get; set; }

}
