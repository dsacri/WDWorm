using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// change which neurons are enabled and disabled
/// </summary>
public class MatlabToggleNeuron : MonoBehaviour, ISaveable
{
    [SerializeField] private TMP_Text selectedNeuronName;
    [SerializeField] private Toggle neuronEnabled;

    [SerializeField] private Toggle motor;
    [SerializeField] private Toggle inter;
    [SerializeField] private Toggle sensory;
    [SerializeField] private Toggle allNeurons;
    [SerializeField] private Toggle locomotion;
    [SerializeField] private Toggle chemosensory;
    [SerializeField] private Toggle custom;

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
            neuronEnabled.isOn = false;
            neuronEnabled.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            neuronEnabled.interactable = false;
        }
        else if (neuronGrid.GetSelectedNeuronCount(0) == 1)
        {
            neuronEnabled.isOn = !neuronGrid.IsFirstSelectedNeuronMarked(0);
            neuronEnabled.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            neuronEnabled.interactable = true;
        }
        else
        {
            if (neuronGrid.AreAllSelectedNeuronMarked(0))
            {
                neuronEnabled.isOn = false;
                neuronEnabled.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            }
            else if (neuronGrid.AreAllSelectedNeuronNotMarked(0))
            {
                neuronEnabled.isOn = true;
                neuronEnabled.GetComponent<ToggleInteractionHelper>().HideCheckmark2();
            }
            else
            {
                neuronEnabled.isOn = false;
                neuronEnabled.GetComponent<ToggleInteractionHelper>().ShowCheckmark2();
            }

            neuronEnabled.interactable = true;
        }

        blockToggleStimulatedNeuron = false;
    }

    public void TogglePressed()         //gui
    {
        if (blockToggleStimulatedNeuron)
            return;

        neuronGrid.MarkAllSelectedNeurons(!neuronEnabled.isOn, 0);

        custom.isOn = true;
    }

    public void CircuitryTogglePressed()        //gui
    {
        if (custom.isOn)
            return;

        neuronGrid.DeselectAllNeurons();
        neuronGrid.MarkAllNeurons(true, 0);

        if (motor.isOn)
            MarkCircuitry(RunMe.motorNSet);
        else if (inter.isOn)
            MarkCircuitry(RunMe.interNSet);
        else if (sensory.isOn)
            MarkCircuitry(RunMe.sensoryNSet);
        else if (allNeurons.isOn)
            MarkCircuitry(RunMe.allNeuronsSet);
        else if (locomotion.isOn)
            MarkCircuitry(RunMe.locomotionSet);
        else if (chemosensory.isOn)
            MarkCircuitry(RunMe.chemosensorySet);
    }

    /// <summary>
    /// mark all neurons of this circuitry
    /// </summary>
    private void MarkCircuitry(MatlabMatrix matlabMatrix)
    {
        for (int i = 0; i < matlabMatrix.Size(1); i++)
        {
            neuronGrid.MarkNeuron((int)matlabMatrix[i], false, 0);
        }
    }

    public void EnableAllNeurons()      //gui
    {
        neuronGrid.MarkAllNeurons(false, 0);
        neuronGrid.DeselectAllNeurons();
    }

    public void Save(MatlabSerializedData matlabSerializedData)
    {
        matlabSerializedData.motor = motor.isOn;
        matlabSerializedData.inter = inter.isOn;
        matlabSerializedData.sensory = sensory.isOn;
        matlabSerializedData.allNeurons = allNeurons.isOn;
        matlabSerializedData.locomotion = locomotion.isOn;
        matlabSerializedData.chemosensory = chemosensory.isOn;
        matlabSerializedData.custom = custom.isOn;

        matlabSerializedData.idxNeurons = Util.InvertBoolArray((bool[])neuronGrid.GetMarkedNeurons(0).Clone());
        
    }

    public void Load(MatlabSerializedData matlabSerializedData)
    {
        neuronGrid.DeselectAllNeurons();

        if (matlabSerializedData.custom)
        {
            custom.isOn = true;

            for (int i = 0; i < matlabSerializedData.idxNeurons.Length; i++)
            {
                neuronGrid.MarkNeuron(i, !matlabSerializedData.idxNeurons[i], 0);
            }
        }
        else
        {
            motor.isOn = matlabSerializedData.motor;
            inter.isOn = matlabSerializedData.inter;
            sensory.isOn = matlabSerializedData.sensory;
            allNeurons.isOn = matlabSerializedData.allNeurons;
            locomotion.isOn = matlabSerializedData.locomotion;
            chemosensory.isOn = matlabSerializedData.chemosensory;
        }
    }
}
