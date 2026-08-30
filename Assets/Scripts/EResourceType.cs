using UnityEngine;

public enum EResourceType
{
    Day,
    Population,
    Morale,
    Herd
}

public static class EResourceTypeExtensions
{
    public static string ToString(this EResourceType resourceType)
    {
        switch (resourceType)
        {
            case EResourceType.Day: return "Day";
            case EResourceType.Population: return "Followers";
            case EResourceType.Herd: return "Supplies";
            case EResourceType.Morale: return "Devotion";

            default:
                Debug.Assert(false);
                return string.Empty;
        }
    }

}