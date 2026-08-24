using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform PopupParent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PopupParent.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PopupParent.gameObject.SetActive(false);
    }
}
