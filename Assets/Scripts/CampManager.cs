using UnityEngine;

public class CampManager : MonoBehaviour
{
    public void DoRest()
    {
        TickManager.Instance.TickOnce();
    }

    public void DoSacrifice()
    {
        ResourceManager.Instance.AddResource(EStatType.Herd, -1);
        ResourceManager.Instance.AddResource(EStatType.Morale, 5);
    }

    public void DoFeast()
    {
        ResourceManager.Instance.AddResource(EStatType.Herd, -5);
        ResourceManager.Instance.AddResource(EStatType.Morale, 15);
    }
}
