using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// change settings how the graphs are shown
/// </summary>
public class MatlabGraphSettings : MonoBehaviour, ISaveable
{
    [SerializeField] private TMP_Text selectedNeuronName;
    [SerializeField] private Toggle showNeuron;

    [SerializeField] private Toggle lineGraph;
    [SerializeField] private Toggle heatmap;
    [SerializeField] private Toggle membranePotential;
    [SerializeField] private Toggle calciumConcentration;
    [SerializeField] private Toggle alphanumeric;
    [SerializeField] private Toggle byType;

    private bool blockToggleStimulatedNeuron;
    private NeuronGrid neuronGrid;

    private void Start()
    {
        neuronGrid = GetComponent<NeuronGrid>();
        neuronGrid.Initialize();
        //neuron selection changed
        neuronGrid.SelectionChangedEvent.AddListener(SelectionChangedListener);
        
        RefreshSelectedDetails();
    }

    private void SelectionChangedListener()
    {
        RefreshSelectedDetails();
    }

    /// <summary>
    /// refresh ui of selected neuron
    /// </summary>
    private void RefreshSelectedDetails()
    {
        blockToggleStimulatedNeuron = true;

        selectedNeuronName.text = neuronGrid.GetSelectedNeuronName(0);

        if (neuronGrid.GetSelectedNeuronCount(0) == 0)
        {
            showNeuron.isOn = false;
            showNeuron.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            showNeuron.interactable = false;
        }
        else if (neuronGrid.GetSelectedNeuronCount(0) == 1)
        {
            showNeuron.isOn = !neuronGrid.IsFirstSelectedNeuronMarked(0);
            showNeuron.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            showNeuron.interactable = true;
        }
        else
        {

            if (neuronGrid.AreAllSelectedNeuronMarked(0))
            {
                showNeuron.isOn = false;
                showNeuron.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            }
            else if (neuronGrid.AreAllSelectedNeuronNotMarked(0))
            {
                showNeuron.isOn = true;
                showNeuron.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            }
            else
            {
                showNeuron.isOn = false;
                showNeuron.GetComponent<ToggleInteractionHelper>().ShowCheckmark2();
            }

            showNeuron.interactable = true;
        }

        blockToggleStimulatedNeuron = false;
    }

    public void ShowNeuron()            //gui
    {
        if (blockToggleStimulatedNeuron)
            return;

        neuronGrid.MarkAllSelectedNeurons(!showNeuron.isOn, 0);
    }

    public void ShowAllNeurons()        //gui
    {
        neuronGrid.MarkAllNeurons(false, 0);
    }

    public void ShowNoNeurons()         //gui
    {
        neuronGrid.MarkAllNeurons(true, 0);
    }

    public void Save(MatlabSerializedData matlabSerializedData)
    {
        matlabSerializedData.lineGraph = lineGraph.isOn;
        matlabSerializedData.heatmap = heatmap.isOn;
        matlabSerializedData.membranePotential = membranePotential.isOn;
        matlabSerializedData.calciumConcentration = calciumConcentration.isOn;
        matlabSerializedData.alphanumeric = alphanumeric.isOn;
        matlabSerializedData.byType = byType.isOn;

        matlabSerializedData.graphNeurons = Util.InvertBoolArray((bool[])neuronGrid.GetMarkedNeurons(0).Clone());
    }

    public void Load(MatlabSerializedData matlabSerializedData)
    {
        neuronGrid.DeselectAllNeurons();

        lineGraph.isOn = matlabSerializedData.lineGraph;
        heatmap.isOn = matlabSerializedData.heatmap;
        membranePotential.isOn = matlabSerializedData.membranePotential;
        calciumConcentration.isOn = matlabSerializedData.calciumConcentration;
        alphanumeric.isOn = matlabSerializedData.alphanumeric;
        byType.isOn = matlabSerializedData.byType;

        for (int i = 0; i < matlabSerializedData.graphNeurons.Length; i++)
        {
            neuronGrid.MarkNeuron(i, !matlabSerializedData.graphNeurons[i], 0);
        }
    }

}
