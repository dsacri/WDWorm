using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// graph to show activity of a single neuron
/// </summary>
public class Graph : MaskableGraphic
{
    /// <summary>
    /// size of the graph
    /// </summary>
    [SerializeField] private Image size;
    [SerializeField] private float lineWidth = 2f;
    [SerializeField] private TMP_Text title;

    /// <summary>
    /// lowest value
    /// </summary>
    public static float MinValue { get; set; }
    /// <summary>
    /// highest value
    /// </summary>
    public static float MaxValue { get; set; }
    public NeuronManager.DeserializedNeuron Neuron { get; private set; }

    /// <summary>
    /// line color
    /// </summary>
    private Color lineColor = Color.white;
    /// <summary>
    /// values to plot
    /// </summary>
    private List<float> values = new();
    /// <summary>
    /// maximum number of values to store
    /// </summary>
    private int maxValues;
    /// <summary>
    /// line or heatmap
    /// </summary>
    private bool isLine;
    private ScrollViewVisibilityChecker scrollViewVisibilityChecker;

    /// <summary>
    /// set multiple values
    /// </summary>
    public void SetValues(float[] newValues)
    {
        values = newValues.ToList();
    }

    /// <summary>
    /// add a single value to display
    /// </summary>
    public void AddValue(float value)
    {
        values.Add(value);

        //remove oldest value if the count exceeds the max
        if (values.Count > maxValues) 
            values.RemoveAt(0);
    }

    public void SetMaxValueCount(int number)
    {
        maxValues = number;
    }

    /// <summary>
    /// initialise
    /// </summary>
    public void SetData(Color color, NeuronManager.DeserializedNeuron neuron, bool isLine, ScrollViewVisibilityChecker scrollViewVisibilityChecker)
    {
        if (isLine)
        {
            title.color = color;
            lineColor = color;
        }

        Neuron = neuron;
        title.text = neuron.Name;
        this.isLine = isLine;
        gameObject.name = neuron.Name;
        this.scrollViewVisibilityChecker = scrollViewVisibilityChecker;
    }

    public void SetColor(Color color)
    {
        if (isLine)
        {
            title.color = color;
            lineColor = color;
        }
    }

    /// <summary>
    /// refresh
    /// </summary>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (!InitializationManager.Instance.LoadingFinished || values.Count < 2) 
            return;

        //set canvas renderer to visible
        canvasRenderer.SetAlpha(1);
        //clear previous mesh data
        vh.Clear();

        if (!scrollViewVisibilityChecker.VisibleChilds.Contains(gameObject))
        {
            return;
        }

        //graph size
        Rect graphRect = size.rectTransform.rect;
        //width of one value point
        float xStep = maxValues > 1 ? graphRect.width / (maxValues - 1) : 0;
        float translateX = -graphRect.width / 2 + size.transform.localPosition.x;
        float translateY = -graphRect.height / 2;

        //current position
        float x = translateX;

        //point of last loop
        Vector2 prevPoint = new(x, Mathf.InverseLerp(MinValue, MaxValue, values[0]) * graphRect.height + translateY);

        //draw all values
        for (int i = 1; i < values.Count; i++)
        {
            if (isLine)
            {
                Vector2 newPoint = new(x + xStep, Mathf.InverseLerp(MinValue, MaxValue, values[i]) * graphRect.height + translateY);
                DrawLine(vh, prevPoint, newPoint, lineWidth, lineColor);
                prevPoint = newPoint;
            }
            else
            {
                float normalizedValue = Mathf.InverseLerp(MinValue, MaxValue, values[i]);
                Color cellColor = Util.GetInterpolatedColor(normalizedValue);

                DrawHeatmap(vh, new Vector2(x, translateY), new Vector2(x + xStep, graphRect.height + translateY), cellColor);
            }

            x += xStep;
        }
    }

    /// <summary>
    /// draw line between two points
    /// </summary>
    private void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, float width, Color color)
    {
        Vector2 normal = (end - start).normalized;
        Vector2 perpendicular = new Vector2(-normal.y, normal.x) * (width * 0.5f);

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = start - perpendicular;
        vh.AddVert(vert);
        vert.position = start + perpendicular;
        vh.AddVert(vert);
        vert.position = end - perpendicular;
        vh.AddVert(vert);
        vert.position = end + perpendicular;
        vh.AddVert(vert);

        int index = vh.currentVertCount - 4;
        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index + 1, index + 2, index + 3);
    }

    /// <summary>
    /// create quad with a color
    /// </summary>
    private void DrawHeatmap(VertexHelper vh, Vector2 bottomLeft, Vector2 topRight, Color color)
    {
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = bottomLeft;
        vh.AddVert(vert);
        vert.position = new Vector2(bottomLeft.x, topRight.y);
        vh.AddVert(vert);
        vert.position = topRight;
        vh.AddVert(vert);
        vert.position = new Vector2(topRight.x, bottomLeft.y);
        vh.AddVert(vert);

        int index = vh.currentVertCount - 4;
        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index, index + 2, index + 3);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        canvasRenderer.SetAlpha(0);     //has to be transparent until the first value is drawn
    }

}
