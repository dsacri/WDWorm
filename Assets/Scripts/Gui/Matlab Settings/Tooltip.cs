using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// show content when hovered with mouse
/// </summary>
public class Tooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipContent;

    private void Awake()
    {
        tooltipContent.SetActive(false);

        EventTrigger eventTrigger = GetComponent<EventTrigger>();

        EventTrigger.Entry onEntry = new();
        onEntry.eventID = EventTriggerType.PointerEnter;
        onEntry.callback.AddListener((eventData) => { OnPointerEnter(); });
        eventTrigger.triggers.Add(onEntry);

        EventTrigger.Entry onExit = new();
        onExit.eventID = EventTriggerType.PointerExit;
        onExit.callback.AddListener((eventData) => { OnPointerExit(); });
        eventTrigger.triggers.Add(onExit);
    }

    private void OnPointerEnter()
    {
        tooltipContent.SetActive(true);
    }

    private void OnPointerExit()
    {
        tooltipContent.SetActive(false);
    }
}
