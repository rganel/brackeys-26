using UnityEngine;

public enum EResourceType
{
    Day,
    Population,
    Morale,
    Herd,
}

public static class EStatTypeExtensions
{
    public static string ToString(this EResourceType resourceType)
    {
        switch (resourceType)
        {
            case EResourceType.Day: return "Day";
            case EResourceType.Population: return "Caravan";
            case EResourceType.Herd: return "Herd";
            case EResourceType.Morale: return "Morale";

            default:
                Debug.Assert(false);
                return string.Empty;
        }
    }

}