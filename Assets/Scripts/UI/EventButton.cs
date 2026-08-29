using UnityEngine.EventSystems;

namespace UI
{
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
}
