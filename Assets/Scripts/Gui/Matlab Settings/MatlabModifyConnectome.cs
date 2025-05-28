using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// create new connections between neurons and disable transmission types
/// </summary>
public class MatlabModifyConnectome : MonoBehaviour, ISaveable
{
    [SerializeField] private TMP_Text[] transmittingNeuronNames;
    [SerializeField] private TMP_Text[] receivingNeuronNameNeuronNames;
    [SerializeField] private Toggle additionalConnection;
    [SerializeField] private TMP_Text additionalConnectionText;
    [SerializeField] private TMP_Text additionalConnectionText2;

    [SerializeField] private Toggle glutamat;
    [SerializeField] private Toggle acetylcholine;
    [SerializeField] private Toggle gammaAminobutyricAcid;
    [SerializeField] private Toggle monoamines;
    [SerializeField] private Toggle shortRangedNeuropeptides;
    [SerializeField] private Toggle gapJunctions;

    [SerializeField] private TMP_InputField[] inputFields;

    private bool blockToggleAdditionalConnection;
    private bool blockSelectionChanged;
    private bool blockInputFieldInWindowChanged;
    private NeuronGrid neuronGrid;
    private List<MatlabSerializedData.NeuronConnection> connections;
    private string[] defaultValues;
    private string originalAdditionalConnectionText;
    private string originalAdditionalConnectionText2;
    private readonly string noNeuronsSelectedText = "Please select 2 different neurons";

    private void Start()
    {
        originalAdditionalConnectionText = additionalConnectionText.text;
        originalAdditionalConnectionText2 = additionalConnectionText2.text;

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
        connections = new();
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
        if (blockSelectionChanged)
            return;

        blockToggleAdditionalConnection = true;
        blockSelectionChanged = true;
        blockInputFieldInWindowChanged = true;

        neuronGrid.MarkAllNeurons(false, 1);

        transmittingNeuronNames[0].text = neuronGrid.GetSelectedNeuronName(0);
        receivingNeuronNameNeuronNames[0].text = neuronGrid.GetSelectedNeuronName(1);

        int? neuronFrom = neuronGrid.GetFirstSelectedNeuronIndex(0);
        int? neuronTo = neuronGrid.GetFirstSelectedNeuronIndex(1);

        //is selection valid for a connection?
        if (neuronFrom.HasValue && neuronTo.HasValue && neuronFrom.Value != neuronTo.Value)
        {
            transmittingNeuronNames[1].text = neuronGrid.GetSelectedNeuronName(0);
            receivingNeuronNameNeuronNames[1].text = neuronGrid.GetSelectedNeuronName(1);

            additionalConnectionText.text = originalAdditionalConnectionText;
            additionalConnectionText2.text = originalAdditionalConnectionText2;

            MatlabSerializedData.NeuronConnection neuronConnection = GetConnection(neuronFrom.Value, neuronTo.Value);

            additionalConnection.interactable = true;
            additionalConnection.isOn = neuronConnection != null;

            if (neuronConnection == null)
            {
                for (int i = 0; i < inputFields.Length; i++)
                {
                    inputFields[i].text = "";
                    inputFields[i].enabled = false;
                }
            }
            else
            {
                SetInputFieldValues(neuronConnection);
            }
        }
        else
        {
            transmittingNeuronNames[1].text = "";
            receivingNeuronNameNeuronNames[1].text = "";

            additionalConnectionText.text = noNeuronsSelectedText;
            additionalConnectionText2.text = "";

            additionalConnection.interactable = false;
            additionalConnection.isOn = false;

            for (int i = 0; i < inputFields.Length; i++)
            {
                inputFields[i].text = "";
                inputFields[i].enabled = false;
            }
        }
        
        if (neuronFrom != null)
        {
            List<int> connections = GetConnection(neuronFrom.Value);

            for (int i = 0; i < connections.Count; i++)
            {
                neuronGrid.MarkNeuron(connections[i], true, 1);
            }
        }

        blockSelectionChanged = false;
        blockToggleAdditionalConnection = false;
        blockInputFieldInWindowChanged = false;
    }

    private void CreateConnection(int from, int to)
    {
        MatlabSerializedData.NeuronConnection neuronConnection = new();
        neuronConnection.connected = true;
        neuronConnection.fromNeuron = from;
        neuronConnection.toNeuron = to;
        SetDefaultValues(neuronConnection);
        connections.Add(neuronConnection);
    }

    private void RemoveConnection(int from, int to)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].fromNeuron == from && connections[i].toNeuron == to)
            {
                connections.RemoveAt(i);
                break;
            }
        }
    }

    private List<int> GetConnection(int from)
    {
        List<int> connections = new();

        for (int i = 0; i < this.connections.Count; i++)
        {
            if (this.connections[i].fromNeuron == from)
            {
                connections.Add(this.connections[i].toNeuron);
            }
        }
        return connections;
    }

    private MatlabSerializedData.NeuronConnection GetConnection(int from, int to)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].fromNeuron == from && connections[i].toNeuron == to)
            {
                return connections[i];
            }
        }

        return null;
    }

    private bool IsConnected(int from, int to)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].fromNeuron == from && connections[i].toNeuron == to)
            {
                return true;
            }
        }

        return false;
    }

    public void ToggleAdditionalConnectionPressed()         //gui
    {
        if (blockToggleAdditionalConnection)
            return;

        int? neuron1 = neuronGrid.GetFirstSelectedNeuronIndex(0);
        int? neuron2 = neuronGrid.GetFirstSelectedNeuronIndex(1);

        if (neuron1.HasValue && neuron2.HasValue && neuron1.Value != neuron2.Value)
        {
            if (IsConnected(neuron1.Value, neuron2.Value))
            {
                RemoveConnection(neuron1.Value, neuron2.Value);
            }
            else
            {
                CreateConnection(neuron1.Value, neuron2.Value);
            }
        }

        RefreshSelectedDetails();
    }

    public void ButtonResetConnectionsPressed()         //gui
    {
        ResetValues();
        neuronGrid.DeselectAllNeurons();
    }

    public void InputFieldChanged()          //gui
    {
        if (blockInputFieldInWindowChanged)
            return;

        int? neuron1 = neuronGrid.GetFirstSelectedNeuronIndex(0);
        int? neuron2 = neuronGrid.GetFirstSelectedNeuronIndex(1);

        if (neuron1.HasValue && neuron2.HasValue && neuron1.Value != neuron2.Value)
        {
            MatlabSerializedData.NeuronConnection neuronConnection = GetConnection(neuron1.Value, neuron2.Value);

            if (neuronConnection != null)
            {
                SetNeuronConnectionValues(neuronConnection);
            }
        }
    }

    private void SetDefaultValues(MatlabSerializedData.NeuronConnection neuronConnection)
    {
        neuronConnection.gGlu = defaultValues[0];
        neuronConnection.gACh = defaultValues[1];
        neuronConnection.gGABA = defaultValues[2];
        neuronConnection.gMo = defaultValues[3];
        neuronConnection.gExtra = defaultValues[4];
        neuronConnection.Eexc = defaultValues[5];
        neuronConnection.Einh = defaultValues[6];
        neuronConnection.EMo = defaultValues[7];
        neuronConnection.Eextra = defaultValues[8];
        neuronConnection.Us1 = defaultValues[9];
        neuronConnection.UMo1 = defaultValues[10];
        neuronConnection.Uex1 = defaultValues[11];
        neuronConnection.Us2 = defaultValues[12];
        neuronConnection.UMo2 = defaultValues[13];
        neuronConnection.Uex2 = defaultValues[14];
    }

    /// <summary>
    /// show values of this connection
    /// </summary>
    private void SetInputFieldValues(MatlabSerializedData.NeuronConnection neuronConnection)
    {
        blockInputFieldInWindowChanged = true;

        for (int i = 0; i < inputFields.Length; i++)
        {
            inputFields[i].enabled = transform;
        }

        inputFields[0].text = neuronConnection.gGlu;
        inputFields[1].text = neuronConnection.gACh;
        inputFields[2].text = neuronConnection.gGABA;
        inputFields[3].text = neuronConnection.gMo;
        inputFields[4].text = neuronConnection.gExtra;
        inputFields[5].text = neuronConnection.Eexc;
        inputFields[6].text = neuronConnection.Einh;
        inputFields[7].text = neuronConnection.EMo;
        inputFields[8].text = neuronConnection.Eextra;
        inputFields[9].text = neuronConnection.Us1;
        inputFields[10].text = neuronConnection.UMo1;
        inputFields[11].text = neuronConnection.Uex1;
        inputFields[12].text = neuronConnection.Us2;
        inputFields[13].text = neuronConnection.UMo2;
        inputFields[14].text = neuronConnection.Uex2;

        blockInputFieldInWindowChanged = false;
    }

    private void SetNeuronConnectionValues(MatlabSerializedData.NeuronConnection neuronConnection)
    {
        neuronConnection.gGlu = inputFields[0].text;
        neuronConnection.gACh = inputFields[1].text;
        neuronConnection.gGABA = inputFields[2].text;
        neuronConnection.gMo = inputFields[3].text;
        neuronConnection.gExtra = inputFields[4].text;
        neuronConnection.Eexc = inputFields[5].text;
        neuronConnection.Einh = inputFields[6].text;
        neuronConnection.EMo = inputFields[7].text;
        neuronConnection.Eextra = inputFields[8].text;
        neuronConnection.Us1 = inputFields[9].text;
        neuronConnection.UMo1 = inputFields[10].text;
        neuronConnection.Uex1 = inputFields[11].text;
        neuronConnection.Us2 = inputFields[12].text;
        neuronConnection.UMo2 = inputFields[13].text;
        neuronConnection.Uex2 = inputFields[14].text;
    }

    public void Load(MatlabSerializedData matlabSerializedData)
    {
        glutamat.isOn = matlabSerializedData.ActivateA_Glu;
        acetylcholine.isOn = matlabSerializedData.ActivateA_ACh;
        gammaAminobutyricAcid.isOn = matlabSerializedData.ActivateA_GABA;
        monoamines.isOn = matlabSerializedData.ActivateA_mon;
        shortRangedNeuropeptides.isOn = matlabSerializedData.ActivateA_np;
        gapJunctions.isOn = matlabSerializedData.ActivateA_el;


        ResetValues();
        neuronGrid.DeselectAllNeurons();

        connections = matlabSerializedData.connections.ToList();

        RefreshSelectedDetails();
    }

    public void Save(MatlabSerializedData matlabSerializedData)
    {
        matlabSerializedData.ActivateA_Glu = glutamat.isOn;
        matlabSerializedData.ActivateA_ACh = acetylcholine.isOn;
        matlabSerializedData.ActivateA_GABA = gammaAminobutyricAcid.isOn;
        matlabSerializedData.ActivateA_mon = monoamines.isOn;
        matlabSerializedData.ActivateA_np = shortRangedNeuropeptides.isOn;
        matlabSerializedData.ActivateA_el = gapJunctions.isOn;

        matlabSerializedData.connections = connections.ToArray();
    }

}
