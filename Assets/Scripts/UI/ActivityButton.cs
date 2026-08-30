using System.Linq;
using Managers;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ActivityButton : Button
    {
        [SerializeField] protected ActivitySO ActivityConfig;
        [SerializeField] protected TMP_Text DisplayLabel;
        [SerializeField] protected RectTransform DetailsParent;
        [SerializeField] protected TMP_Text DetailsLabel;

        protected override void OnEnable()
        {
            base.OnEnable();

            ResourceManager.Instance?.ResourceAmountUpdated.AddListener(HandleResourceChange);

            Debug.Assert(DisplayLabel != null);

            if (ActivityConfig == null)
            {
                return;
            }

            DisplayLabel.text = ActivityConfig.DisplayName;

            if (ActivityManager.Instance != null)
            {
                if (DetailsLabel != null)
                {
                    DetailsLabel.text = string.Join("\n", ActivityManager.Instance.GetChangeAmounts(ActivityConfig).OrderBy(kvp => kvp.Key).Select(kvp => Format(kvp.Key, kvp.Value)));
                }
            }
        }

        protected override void OnDisable()
        {
            ResourceManager.Instance?.ResourceAmountUpdated.RemoveListener(HandleResourceChange);

            // Trigger this manually to reset the button because OnPointerExit won't fire if the button press caused it to become disabled
            OnPointerExit(null);

            base.OnDisable();
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
            if (ActivityManager.Instance != null)
            {
                interactable &= ActivityManager.Instance.CanPerform(ActivityConfig);
            }

            base.OnPointerExit(eventData);

            if (DetailsParent != null)
            {
                DetailsParent.gameObject.SetActive(false);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable) return;

            // Do this before making the button not interactable or the events won't fire
            base.OnPointerClick(eventData);

            interactable = false;
            if (DetailsParent != null)
            {
                DetailsParent.gameObject.SetActive(false);
            }

            ActivityManager.Instance.ApplyResourceChanges(ActivityConfig);
        }

        public void SetActivity(ActivitySO activityDefinition)
        {
            ActivityConfig = activityDefinition;
            DisplayLabel.text = ActivityConfig.DisplayName;
            if (DetailsLabel != null)
            {
                DetailsLabel.text = string.Join("\n", ActivityManager.Instance.GetChangeAmounts(ActivityConfig).OrderBy(kvp => kvp.Key).Select(kvp => Format(kvp.Key, kvp.Value)));
            }
        }

        private void HandleResourceChange(EResourceType resourceType, float amount)
        {
            interactable &= ActivityManager.Instance.CanPerform(ActivityConfig);
        }

        private string Format(EResourceType resourceType, float amount)
        {
            return $"{((amount >= 0) ? "+" : string.Empty)}{amount} {resourceType}";
        }
    }
}