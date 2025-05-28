using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// change gui colors
/// </summary>
[RequireComponent(typeof(EventTrigger))]
public class GraphicInteractionHelper : MonoBehaviour
{
    private Graphic[] graphics;
    private bool hover;
    private bool down;

    private void Awake()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();

        EventTrigger.Entry onEntry = new();
        onEntry.eventID = EventTriggerType.PointerEnter;
        onEntry.callback.AddListener((eventData) => { OnPointerEnter(); });
        eventTrigger.triggers.Add(onEntry);

        EventTrigger.Entry onExit = new();
        onExit.eventID = EventTriggerType.PointerExit;
        onExit.callback.AddListener((eventData) => { OnPointerExit(); });
        eventTrigger.triggers.Add(onExit);

        EventTrigger.Entry onDown = new();
        onDown.eventID = EventTriggerType.PointerDown;
        onDown.callback.AddListener((eventData) => { OnPointerDown(); });
        eventTrigger.triggers.Add(onDown);

        EventTrigger.Entry onUp = new();
        onUp.eventID = EventTriggerType.PointerUp;
        onUp.callback.AddListener((eventData) => { OnPointerUp(); });
        eventTrigger.triggers.Add(onUp);
        
        graphics = transform.GetComponentsInChildren<Graphic>();

        //exception
        graphics = graphics.Where(go => go.name != "Hitbox").ToArray();

    }

    private void OnPointerEnter()
    {
        hover = true;

        if (!down)
        {
            foreach (Graphic graphic in graphics)
            {
                graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonHoverColor;
            }
        }
    }

    public void OnPointerExit()
    {
        hover = false;

        if (!down)
        {
            foreach (Graphic graphic in graphics)
            {
                graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonDefaultColor;
            }
        }
    }

    private void OnPointerDown()
    {
        down = true;

        foreach (Graphic graphic in graphics)
        {
            graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonClickColor;
        }
    }

    private void OnPointerUp()
    {
        down = false;

        foreach (Graphic graphic in graphics)
        {
            if (hover)
                graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonHoverColor;
            else
                graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonDefaultColor;
        }
    }

}
