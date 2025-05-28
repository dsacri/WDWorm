using TMPro;
using UnityEngine;

/// <summary>
/// show names of neuron at hovering
/// </summary>
public class HoverNeuron : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI text;
    [SerializeField] protected Vector3 offset;

    /// <summary>
    /// is it still hovering?
    /// </summary>
    private bool refreshedThisFrame;

    private void Awake()
    {
        text.gameObject.SetActive(false);
    }

    private void Start()
    {
        NeuronManager.Instance.NeuronHoverChangedEvent.AddListener(NeuronHoverChangedListener);
    }

    private void Update()
    {
        //refresh position
        text.transform.position = Input.mousePosition + offset * CanvasScale.Scale;

        //deactive if not refreshed anymore
        if (refreshedThisFrame)
        {
            refreshedThisFrame = false;
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }

    private void NeuronHoverChangedListener()
    {
        if (!text.gameObject.activeSelf)
        {
            text.gameObject.SetActive(true);
        }

        text.text = Util.ConvertText(NeuronManager.Instance.NeuronToDeserializedNeuronDictionary[NeuronManager.Instance.HoverNeuron].Name);
        refreshedThisFrame = true;
    }
}
