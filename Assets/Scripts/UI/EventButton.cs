using Scriptable_Objects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class EventButton : Button
    {
        [SerializeField] private RectTransform DetailsParent;
        [SerializeField] private EventSO EventDefinition;
        
        public EventPanel EventPanel;
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable) return;

            // Do this before making the button not interactable or the events won't fire
            base.OnPointerClick(eventData);

            targetGraphic.enabled = false;
            interactable = false;
            
            EventPanel.SetEvent(EventDefinition);
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (DetailsParent != null)
            {
                DetailsParent.gameObject.SetActive(true);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (DetailsParent != null)
            {
                DetailsParent.gameObject.SetActive(false);
            }
        }
    }
}
