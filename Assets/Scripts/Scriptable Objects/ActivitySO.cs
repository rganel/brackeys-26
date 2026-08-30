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
            public float BaseAmountChange;
            public ResourceDependency[] ResourceDependencies;
            public bool IsRequiredCost;

            public override string ToString()
            {
                return $"{((BaseAmountChange >= 0) ? "+" : string.Empty)}{BaseAmountChange} {ResourceType}";
            }
        }

        [Serializable]
        public class ResourceDependency
        {
            public EResourceType ResourceType;
            public AnimationCurve MultiplierCurve;
        }
    }
}
