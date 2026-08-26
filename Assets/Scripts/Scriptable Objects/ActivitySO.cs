using System;
using UnityEngine;
using Utils;

namespace Scriptable_Objects
{
    [SmartConfig("Assets/Backups/Activity")]
    [CreateAssetMenu(fileName = "Activity", menuName = "Scriptable Objects/Activity")]
    public class ActivitySO : ScriptableObject
    {
        public string DisplayName;
        public ResourceChange[] ResourceChanges;
        
        [Serializable]
        public class ResourceChange
        {
            public EResourceType ResourceType;
            public float AmountChange;

            public override string ToString()
            {
                return $"{((AmountChange >= 0) ? "+" : string.Empty)}{AmountChange} {ResourceType}";
            }
        }
    }
}
