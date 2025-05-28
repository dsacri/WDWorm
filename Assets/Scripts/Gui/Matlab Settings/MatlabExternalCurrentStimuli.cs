using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// select neurons that get stimulated and stimulation values
/// </summary>
public class MatlabExternalCurrentStimuli : MonoBehaviour, ISaveable
{
    [SerializeField] private TMP_InputField currentAmplitude;
    [SerializeField] private TMP_InputField pulseWidth;
    [SerializeField] private TMP_Text selectedNeuronName;
    [SerializeField] private Toggle stimulatedNeuron;

    private bool blockToggleStimulatedNeuron;
    private NeuronGrid neuronGrid;

    private void Start()
    {
        neuronGrid = GetComponent<NeuronGrid>();
        neuronGrid.Initialize();
        //neuron selection changed
        neuronGrid.SelectionChangedEvent.AddListener(SelectionChangedListener);

        //defalt start neurons
        neuronGrid.MarkAllNeurons(true, 0);
        neuronGrid.MarkNeuron(145, false, 0);
        neuronGrid.MarkNeuron(146, false, 0);

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
            stimulatedNeuron.isOn = false;
            stimulatedNeuron.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            stimulatedNeuron.interactable = false;
        }
        else if (neuronGrid.GetSelectedNeuronCount(0) == 1)
        {
            stimulatedNeuron.isOn = !neuronGrid.IsFirstSelectedNeuronMarked(0);
            stimulatedNeuron.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            stimulatedNeuron.interactable = true;
        }
        else
        {

            if (neuronGrid.AreAllSelectedNeuronMarked(0))
            {
                stimulatedNeuron.isOn = false;
                stimulatedNeuron.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            }
            else if (neuronGrid.AreAllSelectedNeuronNotMarked(0))
            {
                stimulatedNeuron.isOn = true;
                stimulatedNeuron.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            }
            else
            {
                stimulatedNeuron.isOn = false;
                stimulatedNeuron.GetComponent<ToggleInteractionHelper>().ShowCheckmark2();
            }

            stimulatedNeuron.interactable = true;
        }

        blockToggleStimulatedNeuron = false;
    }

    public void TogglePressed()         //gui
    {
        if (blockToggleStimulatedNeuron)
            return;

        neuronGrid.MarkAllSelectedNeurons(!stimulatedNeuron.isOn, 0);
    }

    public void Save(MatlabSerializedData matlabSerializedData)
    {
        matlabSerializedData.currentAmplitude = currentAmplitude.text;
        matlabSerializedData.pulseWidth = pulseWidth.text;
        matlabSerializedData.idxInputNeurons = Util.InvertBoolArray((bool[])neuronGrid.GetMarkedNeurons(0).Clone());
    }

    public void Load(MatlabSerializedData matlabSerializedData)
    {
        currentAmplitude.text = matlabSerializedData.currentAmplitude;
        pulseWidth.text = matlabSerializedData.pulseWidth;

        neuronGrid.DeselectAllNeurons();

        for (int i = 0; i < matlabSerializedData.idxInputNeurons.Length; i++)
        {
            neuronGrid.MarkNeuron(i, !matlabSerializedData.idxInputNeurons[i], 0);
        }
    }
}
