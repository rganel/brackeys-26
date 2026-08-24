using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HoverOutline : MonoBehaviour
{
    [SerializeField] private float MaxDistance;
    [SerializeField] private float MaxAngle;
    
    [SerializeField] private LayerMask PlayerLayerMask;
    [SerializeField] private RenderingLayerMask OutlineLayerMask;

    private Renderer m_renderer;
    private uint m_defaultLayerMask;

    private void Awake()
    {
        m_renderer = GetComponent<Renderer>();
        m_defaultLayerMask = m_renderer.renderingLayerMask;

        Debug.Assert(MaxDistance > 0.0f);
        Debug.Assert(MaxAngle > 0.0f);
        Debug.Assert(m_renderer != null);
    }

    private void Update()
    {
        m_renderer.renderingLayerMask = IsInteractable() ? OutlineLayerMask : m_defaultLayerMask;
    }

    private bool IsInteractable()
    {
        foreach (Collider playerCollider in Physics.OverlapSphere(transform.position, MaxDistance, PlayerLayerMask))
        {
            Vector3 distanceVector = (transform.position - playerCollider.transform.position);
            return Mathf.Abs(Vector3.Angle(playerCollider.transform.forward, new Vector3(distanceVector.x, 0.0f, distanceVector.z))) <= MaxAngle;
        }

        return false;
    }
}