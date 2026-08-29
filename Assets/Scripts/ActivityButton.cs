using System.Linq;
using Managers;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActivityButton : Button
{
    [SerializeField] protected ActivitySO ActivityConfig;
    [SerializeField] protected TMP_Text DisplayLabel;
    [SerializeField] protected RectTransform DetailsParent;
    [SerializeField] protected TMP_Text DetailsLabel;

    protected override void Awake()
    {
        base.Awake();

        Debug.Assert(ActivityConfig != null);
        Debug.Assert(DisplayLabel != null);

        DisplayLabel.text = ActivityConfig.DisplayName;
        if (DetailsLabel != null)
        {
            DetailsLabel.text = string.Join("\n", ActivityConfig.ResourceChanges.OrderBy(activity => activity.ResourceType));
        }
    }

    protected override void OnDisable()
    {
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
        interactable = true;

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

        foreach (ActivitySO.ResourceChange resourceChange in ActivityConfig.ResourceChanges)
        {
            ResourceManager.Instance.AddResource(resourceChange.ResourceType, resourceChange.AmountChange);
        }
    }
}