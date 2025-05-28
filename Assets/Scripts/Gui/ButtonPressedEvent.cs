using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// event when button was pressed
/// </summary>
public class ButtonPressedEvent : UnityEvent<ButtonPressedEvent>
{
   public Button Button { get; set; }
   public ButtonInteractionHelper ButtonInteractionHelper { get; set; }
}
