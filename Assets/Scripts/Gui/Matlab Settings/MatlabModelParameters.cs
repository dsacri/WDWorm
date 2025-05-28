using TMPro;
using UnityEngine;

/// <summary>
/// change parameters of individual neurons
/// </summary>
public class MatlabModelParameters : MonoBehaviour, ISaveable
{
    [SerializeField] private TMP_Text selectedNeuronName;
    [SerializeField] private TMP_InputField[] inputFields;

    private NeuronGrid neuronGrid;
    /// <summary>
    /// all values of all neurons
    /// </summary>
    private string[][] values;      //1 different values, 2 different neurons
    /// <summary>
    /// start/reset values
    /// </summary>
    private string[] defaultValues;
    private bool blockInputChanged = true;

    private void Start()
    {
        neuronGrid = GetComponent<NeuronGrid>();
        neuronGrid.Initialize();
        //neuron selection changed
        neuronGrid.SelectionChangedEvent.AddListener(SelectionChangedListener);

        defaultValues = new string[inputFields.Length];

        for (int i = 0; i < inputFields.Length; i++)
        {
            defaultValues[i] = inputFields[i].text;
        }

        ResetValues();

        RefreshSelectedDetails();
    }

    private void ResetValues()
    {
        values = new string[inputFields.Length][];

        for (int i = 0; i < inputFields.Length; i++)
        {
            values[i] = new string[neuronGrid.GetNeuronCount()];

            for (int j = 0; j < neuronGrid.GetNeuronCount(); j++)
            {
                values[i][j] = defaultValues[i];
            }
        }
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
        blockInputChanged = true;

        selectedNeuronName.text = neuronGrid.GetSelectedNeuronName(0);
        
        if (neuronGrid.GetSelectedNeuronCount(0) == 0)
        {
            for (int i = 0; i < inputFields.Length; i++)
            {
                inputFields[i].interactable = false;
                inputFields[i].text = "";
            }
        }
        else if (neuronGrid.GetSelectedNeuronCount(0) == 1)
        {
            for (int i = 0; i < inputFields.Length; i++)
            {
                inputFields[i].interactable = true;
                inputFields[i].text = values[i][neuronGrid.GetFirstSelectedNeuronIndex(0).Value];
            }
        }
        else
        {
            for (int i = 0; i < inputFields.Length; i++)
            {
                inputFields[i].interactable = true;

                string text = GetNonEqualText(i);
                inputFields[i].text = text;
            }

        }

        blockInputChanged = false;
    }

    /// <summary>
    /// if multiple neurons with different values are selected, what should be displayed
    /// </summary>
    private string GetNonEqualText(int valueIndex)
    {
        string text = values[valueIndex][neuronGrid.GetFirstSelectedNeuronIndex(0).Value];

        for (int j = 0; j < neuronGrid.GetSelectedNeuronCount(0); j++)
        {
            if (!values[valueIndex][neuronGrid.GetSelectedNeuronIndicies(0)[j]].Equals(text))
            {
                return "?";
            }
        }

        return text;
    }

    public void InputChanged()          //gui
    {
        if (blockInputChanged)
            return;

        for (int i = 0; i < inputFields.Length; i++)
        {
            for (int j = 0; j < neuronGrid.GetSelectedNeuronCount(0); j++)
            {
                int selectedNeuron = neuronGrid.GetSelectedNeuronIndicies(0)[j];
                values[i][selectedNeuron] = inputFields[i].text;
            }
        }
    }

    public void ButtonResetToDefaultValues()            //gui
    {
        neuronGrid.DeselectAllNeurons();
        ResetValues();
    }

    public void Load(MatlabSerializedData matlabSerializedData)
    {
        neuronGrid.DeselectAllNeurons();
        ResetValues();

        values[0] = matlabSerializedData.C;
        values[1] = matlabSerializedData.EL;
        values[2] = matlabSerializedData.GL;
        values[3] = matlabSerializedData.GCa1;
        values[4] = matlabSerializedData.ECa;
        values[5] = matlabSerializedData.UCa1;
        values[6] = matlabSerializedData.UCa2;
        values[7] = matlabSerializedData.GK1;
        values[8] = matlabSerializedData.EK;
        values[9] = matlabSerializedData.UK1;
        values[10] = matlabSerializedData.UK2;
        values[11] = matlabSerializedData.FK;
    }

    public void Save(MatlabSerializedData matlabSerializedData)
    {
        matlabSerializedData.C = (string[])values[0].Clone();
        matlabSerializedData.EL = (string[])values[1].Clone();
        matlabSerializedData.GL = (string[])values[2].Clone();
        matlabSerializedData.GCa1 = (string[])values[3].Clone();
        matlabSerializedData.ECa = (string[])values[4].Clone();
        matlabSerializedData.UCa1 = (string[])values[5].Clone();
        matlabSerializedData.UCa2 = (string[])values[6].Clone();
        matlabSerializedData.GK1 = (string[])values[7].Clone();
        matlabSerializedData.EK = (string[])values[8].Clone();
        matlabSerializedData.UK1 = (string[])values[9].Clone();
        matlabSerializedData.UK2 = (string[])values[10].Clone();
        matlabSerializedData.FK = (string[])values[11].Clone();
    }

}
