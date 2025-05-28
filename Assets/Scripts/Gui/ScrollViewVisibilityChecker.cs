using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// which childs are visible in a scroll view
/// </summary>
public class ScrollViewVisibilityChecker : MonoBehaviour
{
    /// <summary>
    /// scroll view
    /// </summary>
    public ScrollRect scrollRect;
    /// <summary>
    /// content within acroll view (its childs gets checked)
    /// </summary>
    public RectTransform content;

    public HashSet<GameObject> VisibleChilds { get; private set; }

    private void Update()
    {
        VisibleChilds = new();
        CheckVisibleItems();
    }

    private void CheckVisibleItems()
    {
        //scroll view size
        Rect visibleRect = scrollRect.viewport.rect;

        //loop though all childs of content
        foreach (RectTransform child in content)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(scrollRect.viewport, child.position, null, out Vector2 localPoint);
            float elementTop = localPoint.y + child.rect.height / 2;
            float elementBottom = localPoint.y - child.rect.height / 2;

            if (elementTop > visibleRect.yMin && elementBottom < visibleRect.yMax)
            {
                VisibleChilds.Add(child.gameObject);
            }
        }
    }
}
