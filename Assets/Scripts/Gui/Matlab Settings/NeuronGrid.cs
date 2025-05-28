using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// interactable grid of all neurons
/// </summary>
public class NeuronGrid : MonoBehaviour
{
    /// <summary>
    /// parent of titles and neuron layouts
    /// </summary>
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject titlePrefab;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject neuronLayoutPrefab;
    /// <summary>
    /// how many different selections are possible
    /// </summary>
    [SerializeField] private int numberOfSelectionLayers;
    /// <summary>
    /// how many different markings are possible
    /// </summary>
    [SerializeField] private int numberOfMarkedLayers;

    public UnityEvent SelectionChangedEvent { get; } = new();

    /// <summary>
    /// buttons for all neurons
    /// </summary>
    private ButtonInteractionHelper[] buttons;
    /// <summary>
    /// all neurons
    /// </summary>
    private NeuronManager.DeserializedNeuron[] neurons;
    /// <summary>
    /// is a neuron marked? (marked layer, index)
    /// </summary>
    private bool[][] markedNeurons;
    /// <summary>
    /// selected neurons (selection layer, index)
    /// </summary>
    private List<int>[] selectedNeurons;
    /// <summary>
    /// neuron that was selected the latest
    /// </summary>
    private int lastSelectedNeuron = -1;
    private bool blockNeuronSelectionListener;
    private int[] buttonOrderToRealOrder;
    private int[] realOrderToButtonOrder;

    public void Initialize()
    {
        //initialize collections
        Dictionary<NeuronManager.Group, GameObject> groups = new();

        int neuronCount = NeuronManager.Instance.DeserializedNeurons.Count;
        buttons = new ButtonInteractionHelper[neuronCount];
        neurons = new NeuronManager.DeserializedNeuron[neuronCount];
        buttonOrderToRealOrder = new int[neuronCount];
        realOrderToButtonOrder = new int[neuronCount];
        List<NeuronManager.Group> neuronGroups = new();

        markedNeurons = new bool[numberOfMarkedLayers][];
        selectedNeurons = new List<int>[numberOfSelectionLayers];

        for (int i = 0; i < numberOfMarkedLayers; i++)
        {
            markedNeurons[i] = new bool[neuronCount];
        }

        for (int i = 0; i < numberOfSelectionLayers; i++)
        {
            selectedNeurons[i] = new();
        }

        List<NeuronManager.DeserializedNeuron> deserializedNeurons = new(NeuronManager.Instance.DeserializedNeurons);

        //order of neuron groups
        string[] groupNames = new string[3];
        groupNames[0] = "sensory neuron";
        groupNames[1] = "interneuron";
        groupNames[2] = "motor neuron";

        //create all gameobjects
        for (int i = 0; i < neuronCount; i++)
        {
            NeuronManager.DeserializedNeuron deserializedNeuron = deserializedNeurons[i];

            if (!groups.ContainsKey(deserializedNeuron.Group))
            {
                TMP_Text title = Instantiate(titlePrefab, parent.transform).GetComponent<TMP_Text>();
                title.text = deserializedNeuron.Group.Name;

                GameObject group = Instantiate(neuronLayoutPrefab, parent.transform);
                groups.Add(deserializedNeuron.Group, group);
                neuronGroups.Add(deserializedNeuron.Group);

                string nameLower = deserializedNeuron.Group.Name.ToLower();
                if (groupNames.Any(x => x.Equals(nameLower)))
                {
                    int index = Array.FindIndex(groupNames, x => x.Equals(nameLower));

                    title.transform.SetSiblingIndex(index * 2);
                    group.transform.SetSiblingIndex(index * 2 + 1);
                }
                else
                {
                    Debug.LogWarning("Group not found");
                }
            }

            GameObject neuron = Instantiate(buttonPrefab, groups[deserializedNeuron.Group].transform);
            ButtonInteractionHelper buttonInteractionHelper = neuron.GetComponent<ButtonInteractionHelper>();
            buttonInteractionHelper.SetText(deserializedNeuron.Name);
            buttonInteractionHelper.IsMarked = new bool[markedNeurons.Length];
            buttonInteractionHelper.ButtonLeftPressedEvent.AddListener(ButtonLeftPressedListener);
            buttonInteractionHelper.ButtonRightPressedEvent.AddListener(ButtonRightPressedListener);

            buttons[i] = buttonInteractionHelper;
            neurons[i] = deserializedNeuron;
        }

        //sort neuron groups
        neuronGroups = groupNames
            .Select(name => neuronGroups.FirstOrDefault(x => x.Name.ToLower().Equals(name)))
            .ToList();

        //fill arrays to convert from button index to neuron index and back
        int arrayCounter = 0;

        for (int i = 0; i < neuronGroups.Count; i++)
        {
            NeuronManager.Group group = neuronGroups[i];

            for (int j = 0; j < groups[group].transform.childCount; j++)
            {
                ButtonInteractionHelper child = groups[group].transform.GetChild(j).GetComponent<ButtonInteractionHelper>();
                buttonOrderToRealOrder[arrayCounter] = Array.IndexOf(buttons, child);
                realOrderToButtonOrder[Array.IndexOf(buttons, child)] = arrayCounter;

                arrayCounter++;
            }
        }
    }

    /// <summary>
    /// left mouse click
    /// </summary>
    private void ButtonLeftPressedListener(ButtonPressedEvent buttonPressedEvent)
    {
        if (blockNeuronSelectionListener)
            return;

        int index = Array.IndexOf(buttons, buttonPressedEvent.ButtonInteractionHelper);
        bool dontSelect = selectedNeurons[0].Contains(index);      //true if already selected neuron was clicked

        //select multiple neurons
        if (numberOfSelectionLayers == 1 && Input.GetKey(KeyCode.LeftControl))
        {
            if (dontSelect)
            {
                DeselectNeuron(index, 0);
                return;
            }

            SelectNeuron(index, 0);
        }
        //select all neurons in list between last selected and new selection
        else if (numberOfSelectionLayers == 1 && Input.GetKey(KeyCode.LeftShift) && lastSelectedNeuron != -1)
        {
            int lastSelectedNeuronAtClick = lastSelectedNeuron;

            int correctedIndex = realOrderToButtonOrder[index];
            int correctedLastSelectedNeuronAtClick = realOrderToButtonOrder[lastSelectedNeuronAtClick];

            if (correctedLastSelectedNeuronAtClick < correctedIndex)
            {
                for (int i = correctedLastSelectedNeuronAtClick; i <= correctedIndex; i++)
                {
                    SelectNeuron(buttonOrderToRealOrder[i], 0);
                }
            }
            else
            {
                for (int i = correctedIndex; i <= correctedLastSelectedNeuronAtClick; i++)
                {
                    SelectNeuron(buttonOrderToRealOrder[i], 0);
                }
            }
        }
        else
        {
            DeselectAllNeurons(0);

            if (dontSelect)
            {
                return;
            }

            SelectNeuron(index, 0);
        }
    }

    /// <summary>
    /// right mouse click
    /// </summary>
    private void ButtonRightPressedListener(ButtonPressedEvent buttonPressedEvent)
    {
        if (blockNeuronSelectionListener)
            return;

        int index = Array.IndexOf(buttons, buttonPressedEvent.ButtonInteractionHelper);
        bool dontSelect = selectedNeurons[1].Contains(index);      //true if already selected neuron was clicked

        DeselectAllNeurons(1);

        if (dontSelect)
        {
            return;
        }

        SelectNeuron(index, 1);
    }

    private void SelectNeuron(int index, int layer)
    {
        if (!selectedNeurons[layer].Contains(index))
            selectedNeurons[layer].Add(index);

        lastSelectedNeuron = index;

        blockNeuronSelectionListener = true;
        
        if (layer == 0)
            buttons[index].SetSelect(true);
        else if (layer == 1)
            buttons[index].SetSelectSecond(true);

        blockNeuronSelectionListener = false;

        SelectionChangedEvent.Invoke();
    }

    public void DeselectAllNeurons()
    {
        for (int i = 0; i < numberOfSelectionLayers; i++)
        {
            DeselectAllNeurons(i);
        }
    }

    public void DeselectAllNeurons(int layer)
    {
        for (int i = selectedNeurons[layer].Count - 1; i >= 0; i--)
        {
            DeselectNeuron(selectedNeurons[layer][i], layer);
        }
    }

    private void DeselectNeuron(int index, int layer)
    {
        selectedNeurons[layer].Remove(index);
        lastSelectedNeuron = -1;

        blockNeuronSelectionListener = true;

        if (layer == 0)
            buttons[index].SetSelect(false);
        else if (layer == 1)
            buttons[index].SetSelectSecond(false);

        blockNeuronSelectionListener = false;

        SelectionChangedEvent.Invoke();
    }

    public void MarkNeuron(int index, bool mark, int layer)
    {
        buttons[index].SetMarked(mark, layer);
        markedNeurons[layer][index] = mark;
    }

    public void MarkAllNeurons(bool mark, int layer)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            MarkNeuron(i, mark, layer);
        }

        SelectionChangedEvent.Invoke();
    }

    public void MarkAllSelectedNeurons(bool mark, int layer)
    {
        foreach (int index in selectedNeurons[0])
        {
            MarkNeuron(index, mark, layer);
        }

        SelectionChangedEvent.Invoke();
    }

    public bool[] GetMarkedNeurons(int layer)
    {
        return markedNeurons[layer];
    }

    public bool AreAllSelectedNeuronMarked(int layer)
    {
        foreach (int index in selectedNeurons[0])
        {
            if (!markedNeurons[layer][index])
            {
                return false;
            }
        }

        return true;
    }

    public bool AreAllSelectedNeuronNotMarked(int layer)
    {
        foreach (int index in selectedNeurons[0])
        {
            if (markedNeurons[layer][index])
            {
                return false;
            }
        }

        return true;
    }

    public bool IsFirstSelectedNeuronMarked(int layer)
    {
        return markedNeurons[layer][selectedNeurons[0][0]];
    }

    public int? GetFirstSelectedNeuronIndex(int layer)
    {
        if (selectedNeurons[layer].Count > 0)
            return selectedNeurons[layer][0];
        else
            return null;
    }

    public List<int> GetSelectedNeuronIndicies(int layer)
    {
        return selectedNeurons[layer];
    }

    public int GetSelectedNeuronCount(int layer)
    {
        return selectedNeurons[layer].Count;
    }

    public int GetNeuronCount()
    {
        return buttons.Length;
    }

    public string GetSelectedNeuronName(int layer)
    {
        if (selectedNeurons[layer].Count == 0)
        {
            return "";
        }
        else if (selectedNeurons[layer].Count == 1)
        {
            return neurons[selectedNeurons[layer][0]].Name;
        }
        else
        {
            return "Multiple";
        }
    }

}
