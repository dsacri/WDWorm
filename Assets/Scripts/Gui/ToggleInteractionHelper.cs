using UnityEngine;

/// <summary>
/// adds a secondary checkmark for toggles
/// </summary>
public class ToggleInteractionHelper : MonoBehaviour
{
    [SerializeField] private GameObject checkmark2;

    public void ShowCheckmark2()
    {
        checkmark2.SetActive(true);
    }

    public void HideCheckmark2()
    {
        checkmark2.SetActive(false);
    }
}
