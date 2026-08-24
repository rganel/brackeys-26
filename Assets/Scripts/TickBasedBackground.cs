using UnityEngine;
using UnityEngine.UI;
using ResourceType = ResourceManager.ResourceType;

[RequireComponent(typeof(Image))]
public class TickBasedBackground : MonoBehaviour
{
    [SerializeField] private Sprite DaytimeSprite;
    [SerializeField] private Sprite NighttimeSprite;
    
    private Image m_image;

    private void Awake()
    {
        m_image = GetComponent<Image>();
        
        Debug.Assert(m_image != null);
    }

    private void OnEnable()
    {
        ResourceManager.Instance.ResourceUpdated.AddListener(OnResourceUpdated);
    }

    private void OnDisable()
    {
        ResourceManager.Instance.ResourceUpdated.RemoveListener(OnResourceUpdated);
    }

    private void OnResourceUpdated(ResourceType resourceType, float amount)
    {
        if (resourceType == ResourceType.Tick)
        {
            m_image.sprite = (amount % 2 == 0) ? DaytimeSprite : NighttimeSprite;
        }
    }
}
