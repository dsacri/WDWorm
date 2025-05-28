using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// window where all graphs are shown
/// </summary>
public class AllGraphsWindow : MonoBehaviour
{
    [SerializeField] protected GameObject lineGraphPrefab;
    [SerializeField] protected GameObject heatmapGraphPrefab;
    [SerializeField] protected Transform graphParent;
    /// <summary>
    /// min distance the window should have to the screen border
    /// </summary>
    [SerializeField] protected Vector2 borderDistance;
    [SerializeField] protected ButtonInteractionHelper openCloseButton;
    [SerializeField] protected GraphicInteractionHelper windowCloseButton;
    [SerializeField] protected ScrollViewVisibilityChecker scrollViewVisibilityChecker;
    [SerializeField] protected TMP_Text title;
    [SerializeField] protected int drawEveryXFrames = 5;

    /// <summary>
    /// singleton
    /// </summary>
    public static AllGraphsWindow Instance { get; private set; }

    /// <summary>
    /// is user hovering over the window
    /// </summary>
    private bool isHovering;
    /// <summary>
    /// is user hovering over the scrollbar
    /// </summary>
    private bool isHoveringScrollbar;
    /// <summary>
    /// is user dragging the window
    /// </summary>
    private bool isDragging;
    /// <summary>
    /// mouse pointer offset at start of dragging
    /// </summary>
    private Vector3 draggingOffset;
    /// <summary>
    /// size of window
    /// </summary>
    private Vector2 size;
    /// <summary>
    /// size of window / 2
    /// </summary>
    private Vector2 sizeHalf;
    private Graph[] allGraphs;
    /// <summary>
    /// last timestemp values were set to graphs
    /// </summary>
    private int? lastSetValueTime;
    /// <summary>
    /// frames until next draw
    /// </summary>
    private int framesTillDraw;

    private void Awake()
    {
        Instance = this;

        CalculateSize();

        Close();
        openCloseButton.ButtonLeftPressedEvent.AddListener(CloseButtonPressedListener);
    }

    private void Update()
    {
        //drag start
        if (isHovering && !isHoveringScrollbar || isDragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                draggingOffset = transform.position - Input.mousePosition;
            }
        }

        Vector3 refreshedPosition = transform.position;

        //dragging
        if (isDragging)
        {
            refreshedPosition = Input.mousePosition + draggingOffset;

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }

        //correct window position
        transform.position = CorrectPosition(refreshedPosition);

        SetValues(false);
    }

    private void SetValues(bool forceDraw)
    {
        if (!InitializationManager.Instance.LoadingFinished)
            return;

        MatlabSerializedData matlabSerializedData = InitializationManager.Instance.MatlabSerializedData;

        if (lastSetValueTime.HasValue && lastSetValueTime == TimingManager.Instance.Time)
        {
            //only draw

            if (forceDraw)
                Draw(forceDraw);
        }
        else if (lastSetValueTime.HasValue && lastSetValueTime == TimingManager.Instance.Time - 1)
        {
            //add a single frame

            int graphIndex = 0;

            for (int i = 0; i < matlabSerializedData.graphNeurons.Length; i++)
            {
                float value = matlabSerializedData.membranePotential ?
                    InitializationManager.Instance.RunMe.GetSingleNeuronActivityOfANeuron(i, TimingManager.Instance.Time) :
                    InitializationManager.Instance.RunMe.GetSingleEtaOfANeuron(i, TimingManager.Instance.Time);

                Graph.MinValue = Mathf.Min(Graph.MinValue, value);
                Graph.MaxValue = Mathf.Max(Graph.MaxValue, value);

                if (!matlabSerializedData.graphNeurons[i])
                    continue;

                allGraphs[graphIndex].SetMaxValueCount(TimingManager.Instance.MaxTime);
                allGraphs[graphIndex].AddValue(value);

                graphIndex++;
            }

            Draw(forceDraw);
        }
        else
        {
            //calculate everything again

            Graph.MinValue = float.MaxValue;
            Graph.MaxValue = float.MinValue;

            int graphIndex = 0;

            for (int i = 0; i < matlabSerializedData.graphNeurons.Length; i++)
            {
                float[] values = matlabSerializedData.membranePotential ?
                    InitializationManager.Instance.RunMe.GetAllNeuronActivityOfANeuron(i, TimingManager.Instance.Time) :
                    InitializationManager.Instance.RunMe.GetAllEtaOfANeuron(i, TimingManager.Instance.Time);

                if (values.Length > 0)
                {
                    Graph.MinValue = Mathf.Min(Graph.MinValue, values.Min());
                    Graph.MaxValue = Mathf.Max(Graph.MaxValue, values.Max());
                }

                if (!matlabSerializedData.graphNeurons[i])
                    continue;

                allGraphs[graphIndex].SetMaxValueCount(TimingManager.Instance.MaxTime);
                allGraphs[graphIndex].SetValues(values);

                graphIndex++;
            }

            Draw(forceDraw);
        }

        lastSetValueTime = TimingManager.Instance.Time;
    }

    /// <summary>
    /// draw all graphs
    /// </summary>
    /// <param name="force">force a new draw (otherwise the draw can be skipped)</param>
    private void Draw(bool force)
    {
        if (!InitializationManager.Instance.LoadingFinished)
            return;

        if (framesTillDraw > 0 && !force)
        {
            framesTillDraw--;
            return;
        }

        framesTillDraw = drawEveryXFrames;

        for (int i = 0; i < allGraphs.Length; i++)
        {
            allGraphs[i].SetVerticesDirty();
        }
    }

    /// <summary>
    /// apply the graph settings from the settings menu
    /// </summary>
    public void ApplySettings()
    {
        if (allGraphs != null)
        {
            for (int i = 0; i < allGraphs.Length; i++)
            {
                Destroy(allGraphs[i].gameObject);
            }
        }

        MatlabSerializedData matlabSerializedData = InitializationManager.Instance.MatlabSerializedData;
        title.text = matlabSerializedData.membranePotential ? "Membrane Potential" : "Calcium Concentration";

        int arraySize = 0;

        for (int i = 0; i < matlabSerializedData.graphNeurons.Length; i++)
        {
            if (matlabSerializedData.graphNeurons[i])
            {
                arraySize++;
            }
        }

        //all graphs that are shown
        allGraphs = new Graph[arraySize];
        Color[] colors = ScriptableObjectUtil.ColorsScriptableObject.LineGraphColors;

        int graphIndex = 0;
        int colorIndex = 0;

        //create graphs
        for (int i = 0; i < matlabSerializedData.graphNeurons.Length; i++)
        {
            if (!matlabSerializedData.graphNeurons[i])
                continue;

            GameObject prefab = matlabSerializedData.lineGraph ? lineGraphPrefab : heatmapGraphPrefab;
            allGraphs[graphIndex] = Instantiate(prefab, graphParent).GetComponent<Graph>();
            allGraphs[graphIndex].SetData(colors[colorIndex], NeuronManager.Instance.DeserializedNeurons[i], matlabSerializedData.lineGraph, scrollViewVisibilityChecker);

            graphIndex++;
            colorIndex++;

            if (colorIndex >= colors.Length)
                colorIndex = 0;
        }

        //sort graphs
        if (matlabSerializedData.byType)
        {
            string[] groupNames = new string[3];
            groupNames[0] = "sensory neuron";
            groupNames[1] = "interneuron";
            groupNames[2] = "motor neuron";

            List<(Graph Graph, int Index)> sortedGraphs = allGraphs
                .Where(g => g != null)
                .Select((g, i) => (Graph: g, Index: i))
                .OrderBy(x =>
                {
                    string nameLower = x.Graph.Neuron.Group.Name.ToLower();
                    int index = Array.FindIndex(groupNames, gn => gn.Equals(nameLower));
                    return index >= 0 ? index : int.MaxValue;
                })
                .ToList();

            colorIndex = 0;

            for (int i = 0; i < sortedGraphs.Count; i++)
            {
                Graph graph = sortedGraphs[i].Graph;
                graph.transform.SetSiblingIndex(i);

                graph.SetColor(colors[colorIndex]);
                colorIndex++;

                if (colorIndex >= colors.Length)
                    colorIndex = 0;
            }
        }

        lastSetValueTime = null;
        SetValues(true);

        if (TimingManager.Instance.IsPaused && gameObject.activeSelf)
        {
            StartCoroutine(DrawIEnumerator());

            IEnumerator DrawIEnumerator()
            {
                yield return null;
                Draw(true);
            }
        }
    }

    /// <summary>
    /// calculate size of the window
    /// </summary>
    private void CalculateSize()
    {
        size = GetComponent<RectTransform>().sizeDelta;
        size += borderDistance;
        size *= CanvasScale.Scale;
        sizeHalf = size / 2;
    }

    /// <summary>
    /// correct position that window doesn't leave screen
    /// </summary>
    private Vector3 CorrectPosition(Vector3 position)
    {
        float x = position.x;
        if (x - sizeHalf.x < 0)
            x = sizeHalf.x;
        else if (x + sizeHalf.x > Screen.width)
            x = Screen.width - sizeHalf.x;

        float y = position.y;
        if (y - sizeHalf.y < 0)
            y = sizeHalf.y;
        else if (y + sizeHalf.y > Screen.height)
            y = Screen.height - sizeHalf.y;

        return new Vector3(x, y, 0);
    }

    private void OpenClose()
    {
        if (gameObject.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Open()
    {
        gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void CloseButtonPressedListener(ButtonPressedEvent buttonPressedEvent)
    {
        OpenClose();
    }

    public void WindowClose()       //gui
    {
        openCloseButton.SetSelect(false);
        isDragging = false;
        windowCloseButton.OnPointerExit();
    }

    public void PointerEnter()      //gui
    {
        isHovering = true;
    }

    public void PointerExit()      //gui
    {
        isHovering = false;
    }

    public void PointerEnterScrollbar()      //gui
    {
        isHoveringScrollbar = true;
    }

    public void PointerExitScrollbar()      //gui
    {
        isHoveringScrollbar = false;
    }

    public void Scrolled()          //gui
    {
        Draw(true);
    }

}
