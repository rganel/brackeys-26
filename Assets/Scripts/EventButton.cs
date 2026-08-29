using Scriptable_Objects;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventButton : ActivityButton
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;

        // Do this before making the button not interactable or the events won't fire
        base.OnPointerClick(eventData);

        targetGraphic.enabled = false;
    }
}
