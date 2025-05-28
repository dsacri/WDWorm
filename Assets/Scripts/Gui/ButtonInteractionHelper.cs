using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// change gui colors
/// </summary>
[RequireComponent(typeof(EventTrigger))]
public class ButtonInteractionHelper : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private States state;
    /// <summary>
    /// invert primary and secondary colors
    /// </summary>
    [SerializeField] private Graphic[] invertGraphics;
    /// <summary>
    /// can button be turned off by clicking on it again?
    /// </summary>
    [SerializeField] private bool isDeselectable;

    /// <summary>
    /// is button selected
    /// </summary>
    public bool IsSelected { get; private set; }
    /// <summary>
    /// second selection mode
    /// </summary>
    public bool IsSelectedSecond { get; private set; }
    public bool[] IsMarked { get; set; }
    public ButtonPressedEvent ButtonLeftPressedEvent { get; } = new();
    public ButtonPressedEvent ButtonRightPressedEvent { get; } = new();

    private Graphic[] graphics;
    private bool isHover;
    private bool isDown;

    /// <summary>
    /// button selection modes
    /// </summary>
    public enum States
    {
        Simple,
        NeutralSelection,
        SingleSelection,
        DoubleSelection,
    }

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

        graphics = transform.GetComponentsInChildren<Graphic>();

        //exception
        graphics = graphics.Where(x => x.name != "Hitbox" && !invertGraphics.Contains(x)).ToArray();

        ButtonLeftPressedEvent.Button = GetComponent<Button>();
        ButtonLeftPressedEvent.ButtonInteractionHelper = this;

        ButtonRightPressedEvent.Button = GetComponent<Button>();
        ButtonRightPressedEvent.ButtonInteractionHelper = this;
    }

    private void OnPointerEnter()
    {
        isHover = true;

        if (!isDown)
        {
            SetHoverColor();
        }
    }

    private void OnPointerExit()
    {
        isHover = false;
        isDown = false;

        SetDefaultColor();
    }

    private void OnPointerDown()
    {
        isDown = true;

        SetDownColor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Middle)
        {
            OnPointerLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (state == States.DoubleSelection)
            {
                OnPointerRightClick();
            }
            else
            {
                OnPointerLeftClick();
            }
        }
    }

    private void OnPointerLeftClick()
    {
        isDown = false;

        if (isHover)
        {
            if (state == States.NeutralSelection || state == States.SingleSelection || state == States.DoubleSelection)
            {
                if (isDeselectable || !IsSelected && !isDeselectable)
                {
                    IsSelected = !IsSelected;
                    SetDefaultColor();
                    SetHoverColor();
                    ButtonLeftPressedEvent.Invoke(ButtonLeftPressedEvent);
                }
            }
            else
            {
                SetHoverColor();
            }
        }
        else
        {
            SetDefaultColor();
        }
    }

    private void OnPointerRightClick()
    {
        isDown = false;

        if (isHover)
        {
            if (isDeselectable || !IsSelectedSecond && !isDeselectable)
            {
                IsSelectedSecond = !IsSelectedSecond;
                SetDefaultColor();
                SetHoverColor();
                ButtonRightPressedEvent.Invoke(ButtonRightPressedEvent);
            }
        }
        else
        {
            SetDefaultColor();
        }
    }

    public void SetSelect(bool select)
    {
        IsSelected = select;

        if (!isHover && !isDown)
            SetDefaultColor();

        ButtonLeftPressedEvent.Invoke(ButtonLeftPressedEvent);
    }

    public void SetSelectSecond(bool select)
    {
        IsSelectedSecond = select;

        if (!isHover && !isDown)
            SetDefaultColor();

        ButtonRightPressedEvent.Invoke(ButtonRightPressedEvent);
    }

    public void SetMarked(bool marked, int layer)
    {
        IsMarked[layer] = marked;

        if (!isHover && !isDown)
            SetDefaultColor();
    }

    public void SetText(string name)
    {
        GetComponentInChildren<TMP_Text>().text = name;
    }

    private void OnDisable()
    {
        isHover = false;
        isDown = false;
        SetDefaultColor();
    }

    private void SetDefaultColor()
    {
        foreach (Graphic graphic in graphics)
        {
            if (IsSelected)
            {
                if (state == States.NeutralSelection)
                {
                    graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonSelectedNeutral;
                }
                else if (state == States.SingleSelection || state == States.DoubleSelection)
                {
                    graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonSelectedFirst;
                }
            }
            else if (IsSelectedSecond)
            {
                graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonSelectedSecond;
            }
            else if (IsMarked != null && IsMarked.Any(x => x == true))
            {
                for (int i = 0; i < IsMarked.Length; i++)
                {
                    if (IsMarked[i])
                    {
                        graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonMarked[i];
                        break;
                    }
                }
            }
            else
            {
                graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonDefaultColor;
            }
        }

        foreach (Graphic graphic in invertGraphics)
        {
            if (IsSelected)
            {
                if (state == States.NeutralSelection)
                {
                    graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonSelectedInvertNeutral;
                }
                else if (state == States.SingleSelection || state == States.DoubleSelection)
                {
                    graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonSelectedInvertFirst;
                }
            }
            else if (IsSelectedSecond)
            {
                graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonSelectedInvertSecond;
            }
            else if (IsMarked != null && IsMarked.Any(x => x == true))
            {
                for (int i = 0; i < IsMarked.Length; i++)
                {
                    if (IsMarked[i])
                    {
                        graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonMarkedInvert[i];
                        break;
                    }
                }
            }
            else
            {
                graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonDefaultInvertColor;
            }
        }
    }

    private void SetHoverColor()
    {
        foreach (Graphic graphic in graphics)
        {
            graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonHoverColor;
        }
    }

    private void SetDownColor()
    {
        foreach (Graphic graphic in graphics)
        {
            graphic.color = ScriptableObjectUtil.ColorsScriptableObject.ButtonClickColor;
        }
    }

}
