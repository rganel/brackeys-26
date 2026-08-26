using UnityEngine;

public class AnchorToParent : MonoBehaviour
{
    public void Reparent(Transform newParent)
    {
        transform.SetParent(newParent, false);
    }
}
