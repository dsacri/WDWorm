using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// creation and management of neurons, their behaviour and their connections
/// </summary>
public class NeuronManager : MonoBehaviour
{
    [SerializeField] protected GameObject neuronPrefab;
    [SerializeField] protected Transform neuronParent;
    /// <summary>
    /// offset to spawn neuron
    /// </summary>
    [SerializeField] protected Vector3 positionOffset;
    /// <summary>
    /// SO for all colors
    /// </summary>
    [SerializeField] protected ColorsScriptableObject colors;

    /// <summary>
    /// singleton
    /// </summary>
    public static NeuronManager Instance { get; private set; }
    /// <summary>
    /// neurons that are selected by the user
    /// </summary>
    public Neuron HoverNeuron { get; private set; }
    /// <summary>
    /// group name to group dictionary
    /// </summary>
    public Dictionary<string, Group> GroupAbbrDictionary { get; } = new();
    public UnityEvent NeuronHoverChangedEvent { get; } = new();
    public UnityEvent ShowAllNeuronsChangedEvent { get; } = new();
    /// <summary>
    /// all deserialized neurons
    /// </summary>
    public List<DeserializedNeuron> DeserializedNeurons { get; private set; } = new();
    public Dictionary<Neuron, DeserializedNeuron> NeuronToDeserializedNeuronDictionary { get; private set; } = null;
    /// <summary>
    /// show all neurons at maximum size
    /// </summary>
    public bool ShowAllNeurons { get => showAllNeurons; set { 

            foreach (Neuron neuron in neuronArray)
            {
                neuron.ChangeToAppearence(1);
            }

            showAllNeurons = value;
            ShowAllNeuronsChangedEvent.Invoke();
            TimingManager.Instance.NeedsSoftUpdate = true;
        } }

    /// <summary>
    /// actual time of replay (can be different then the replay time of the muscles due to different framerates)
    /// </summary>
    private float time;
    private Dictionary<int, Neuron> indexToNeuronDictionary = null;
    private Dictionary<Neuron, int> neuronToIndexDictionary = null;
    /// <summary>
    /// all neurons
    /// </summary>
    private Neuron[] neuronArray = null;
    private bool showAllNeurons;

    private void Awake()
    {
        Instance = this;

        LoadSerializedNeurons();
    }

    public void ManualUpdate()      //time and activity are controlled by timing manager
    {
        //set time
        time = TimingManager.Instance.Time;

        int intTime = GetIntTime();

        //set activity (size, color) of neurons
        float[] activity = InitializationManager.Instance.RunMe.GetNeuronActivityOfAllNeurons(intTime);

        if (!ShowAllNeurons)
        {
            Neuron.ChangeToAppearence(activity, indexToNeuronDictionary, neuronArray);
        }
    }

    private void Update()
    {
        HoverNeuron = null;
        
        //user interaction with neurons
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit) && raycastHit.transform != null && raycastHit.transform.TryGetComponent(out Neuron neuron))
            {
                HoverNeuron = neuron;
                NeuronHoverChangedEvent.Invoke();
            }
        }
    }

    private void LoadSerializedNeurons()
    {
        SerializationNeuronConnectome.AddNames(DeserializedNeurons);
        SerializationNeuronConnectome.AddPositions(DeserializedNeurons);
        SerializationNeuronConnectome.AddNeuronGroups(DeserializedNeurons, GroupAbbrDictionary);

        SetGroupColors();
    }

    /// <summary>
    /// create all neurons and connections and get all neuron related data ready
    /// </summary>
    public void StartNeuronCreation(Vector3 position, Quaternion rotation, SplineUtil.SingleSpline singleSpline)
    {
        CreateNeurons(position, rotation, singleSpline);
        FillNeuronEnumerables();
    }

    /// <summary>
    /// convert replay time to int
    /// </summary>
    public int GetIntTime()
    {
        return (int)time;
    }

    private void CreateNeurons(Vector3 position, Quaternion rotation, SplineUtil.SingleSpline singleSpline)
    {
        foreach (DeserializedNeuron deserializedNeuron in DeserializedNeurons)
        {
            if (deserializedNeuron.Color == null)
                deserializedNeuron.Color = colors.DefaultNeuronGroup;

            Vector3 neuronPosition = position + (Vector3)Util.RotateVectorY(deserializedNeuron.Position / 110f, rotation.eulerAngles.y) + positionOffset;

            deserializedNeuron.Neuron = Instantiate(neuronPrefab, neuronPosition, Quaternion.Euler(new()), neuronParent).GetComponent<Neuron>();
            deserializedNeuron.Neuron.ChangeToAppearence(1);
            deserializedNeuron.Neuron.NeuronName = deserializedNeuron.Name;
            deserializedNeuron.Neuron.gameObject.name = deserializedNeuron.Name;
            deserializedNeuron.Neuron.MaterialColor = deserializedNeuron.Color.Value;

            singleSpline.Mappings.Add(deserializedNeuron.Neuron.GetComponent<Mapping>());
        }

        Mapping.MapToSpline(Data.Instance.NeuronSpline);
    }

    private void SetGroupColors()
    {
        List<Group> groups = GroupAbbrDictionary.Values.ToList();

        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].Members.Count == 0)
            {
                groups.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < groups.Count; i++)
        {
            if (colors.NeuronGroups.Length > i)
            {
                groups[i].Color = colors.NeuronGroups[i];
            }
            else
            {
                Debug.LogError("Error: not enough colors for neuron groups defined in Settings/Colors (ScriptableOject). Number of colors found: " + colors.NeuronGroups.Length + ", colors needed: " + groups.Count);
                groups[i].Color = Color.HSVToRGB(Random.value, 1, 1);
            }

            groups[i].Color = groups[i].Color;
            groups[i].HasColor = true;
        }

        foreach (DeserializedNeuron deserializedNeuron in DeserializedNeurons)
        {
            if (deserializedNeuron.Group == null)
                continue;

            deserializedNeuron.Color = deserializedNeuron.Group.Color;
        }
    }

    /// <summary>
    /// initialize all arrays, lists and dictionaries
    /// </summary>
    private void FillNeuronEnumerables()
    {
        indexToNeuronDictionary = new();
        neuronToIndexDictionary = new();
        NeuronToDeserializedNeuronDictionary = new();

        foreach (DeserializedNeuron deserializedNeuron in DeserializedNeurons)
        {
            if (deserializedNeuron.Index != -1)
            {
                indexToNeuronDictionary.Add(deserializedNeuron.Index, deserializedNeuron.Neuron);
                neuronToIndexDictionary.Add(deserializedNeuron.Neuron, deserializedNeuron.Index);
            }
            
            NeuronToDeserializedNeuronDictionary.Add(deserializedNeuron.Neuron, deserializedNeuron);
        }

        neuronArray = DeserializedNeurons.Select(x => x.Neuron).ToArray();
    }

    /// <summary>
    /// non mono behaviour neuron
    /// </summary>
    public class DeserializedNeuron
    {
        public int Index { get; set; } = -1;
        public string Name { get; set; } = null;
        public Group Group { get; set; } = null;
        public Color? Color { get; set; } = null;
        public Neuron Neuron { get; set; } = null;
        public Vector3 Position { get; set; } = Vector3.down * 100;
        public float[] Activity { get; set; } = null;
        public bool HasActivity { get; set; }
    }

    /// <summary>
    /// scientific group of neurons
    /// </summary>
    public class Group
    {
        public string Name { get; set; } = null;
        public Color Color { get; set; } = Color.gray;
        public bool HasColor { get; set; } 
        public List<DeserializedNeuron> Members { get; } = new();
    }

}
