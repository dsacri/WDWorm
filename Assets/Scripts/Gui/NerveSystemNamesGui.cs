using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// creator and manager of nerve system (neuron) names
/// </summary>
public class NerveSystemNamesGui : MonoBehaviour
{
    /// <summary>
    /// name prefab
    /// </summary>
    [SerializeField] protected GameObject groupPrefab;
    [SerializeField] protected GameObject content;
    /// <summary>
    /// parent of all names
    /// </summary>
    [SerializeField] protected GameObject groupParent;
    /// <summary>
    /// game object above all names
    /// </summary>
    [SerializeField] protected GameObject topBox;
    /// <summary>
    /// game object below all names
    /// </summary>
    [SerializeField] protected GameObject bottomBox;

    private void Awake()
    {
        content.SetActive(false);
    }

    private void Start()
    {
        InitializationManager.Instance.LoadingFinishedEvent.AddListener(LoadingFinishedListener);
    }

    private void LoadingFinishedListener()
    {
        List<GroupGui> allNerveSystemNames = new();

        //create nerve system names
        foreach (NeuronManager.Group group in NeuronManager.Instance.GroupAbbrDictionary.Values)
        {
            if (group.Members.Count == 0 || !group.HasColor)
                continue;

            GroupGui groupGui = Instantiate(groupPrefab, groupParent.transform).GetComponent<GroupGui>();
            TextMeshProUGUI textMeshProUGUI = groupGui.Text;
            textMeshProUGUI.text = group.Name;
            textMeshProUGUI.color = group.Color;
            groupGui.Name = group.Name;

            allNerveSystemNames.Add(groupGui);

            content.SetActive(true);
        }

        //sort nerve system names
        GroupGui interNeuron = allNerveSystemNames.Find(x => x.Name.Equals(SerializationNeuronConnectome.StandardizeNormalName("interneuron")));
        GroupGui motorNeuron = allNerveSystemNames.Find(x => x.Name.Equals(SerializationNeuronConnectome.StandardizeNormalName("motor neuron")));
        GroupGui sensoryNeuron = allNerveSystemNames.Find(x => x.Name.Equals(SerializationNeuronConnectome.StandardizeNormalName("sensory neuron")));

        if (topBox != null)
        {
            topBox.transform.localPosition = Vector3.zero;
            topBox.transform.SetSiblingIndex(0);
        }
        if (bottomBox != null)
        {
            bottomBox.transform.localPosition = Vector3.zero;
            bottomBox.transform.SetSiblingIndex(groupParent.transform.childCount - 1);
        }

        if (sensoryNeuron != null)
            sensoryNeuron.transform.SetSiblingIndex(1);
        if (interNeuron != null)
            interNeuron.transform.SetSiblingIndex(2);
        if (motorNeuron != null)
            motorNeuron.transform.SetSiblingIndex(3);
    }
}
