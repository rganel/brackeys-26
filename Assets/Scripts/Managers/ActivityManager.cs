using System;
using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-700)]
    public class ActivityManager : MonoBehaviour
    {
        public static ActivityManager Instance { get; private set; }

        private void Awake()
        {
            Debug.Assert(Instance == null);
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        public bool CanPerform(ActivitySO activityDefinition)
        {
            foreach (EResourceType resourceType in Enum.GetValues(typeof(EResourceType)))
            {
                float changeAmount = GetChangeAmount(activityDefinition, resourceType, out bool isRequiredCost);
                if (isRequiredCost && (ResourceManager.Instance.GetAmount(resourceType) < -changeAmount))
                {
                    // Debug.Log($"Can't perform {activityDefinition.name} because {ResourceManager.Instance.GetAmount(resourceType)} {resourceType} is less than required cost {-changeAmount}");
                    return false;
                }
            }

            // Debug.Log($"Can perform {activityDefinition.name}");
            return true;
        }

        public bool ApplyResourceChanges(ActivitySO activityDefinition)
        {
            if (!CanPerform(activityDefinition))
            {
                return false;
            }

            foreach (ActivitySO.ResourceChange resourceChange in activityDefinition.ResourceChanges)
            {
                float changeAmount = GetChangeAmount(activityDefinition, resourceChange.ResourceType, out bool isRequiredCost);
                ResourceManager.Instance.AddResource(resourceChange.ResourceType, changeAmount);
            }

            return true;
        }

        public Dictionary<EResourceType, float> GetChangeAmounts(ActivitySO activityDefinition)
        {
            List<EResourceType> resourceTypes = Enum.GetValues(typeof(EResourceType)).Cast<EResourceType>().ToList();
            return resourceTypes.ToDictionary(resourceType => resourceType,
                                              resourceType => GetChangeAmount(activityDefinition, resourceType, out bool isRequiredCost));
        }

        public float GetChangeAmount(ActivitySO activityDefinition, EResourceType resourceType, out bool isRequiredCost)
        {
            float change = 0;
            isRequiredCost = false;

            foreach (ActivitySO.ResourceChange resourceChange in activityDefinition.ResourceChanges)
            {
                if (resourceChange.ResourceType != resourceType)
                {
                    continue;
                }

                float baseChange = resourceChange.BaseAmountChange;
                float multiplier = 0.0f;
                if ((resourceChange.ResourceDependencies != null) && (resourceChange.ResourceDependencies.Length > 0))
                {
                    foreach (ActivitySO.ResourceDependency resourceDependency in resourceChange.ResourceDependencies)
                    {
                        multiplier += GetResourceMultiplier(resourceDependency);
                    }

                    multiplier /= resourceChange.ResourceDependencies.Length;
                }
                else
                {
                    multiplier = 1.0f;
                }

                // Debug.Log($"{activityDefinition.name}: {resourceType} cost is {baseChange} * {multiplier}");

                change += baseChange * multiplier;
                isRequiredCost |= resourceChange.IsRequiredCost;
            }

            // Debug.Log($"{activityDefinition.name}: {resourceType} total cost is {change} ({(isRequiredCost ? "required" : "not required")})");

            return Mathf.RoundToInt(change * 10) / 10.0f;
        }

        private float GetResourceMultiplier(ActivitySO.ResourceDependency resourceDependency)
        {
            if (resourceDependency == null)
            {
                return 1.0f;
            }

            float currentAmount = ResourceManager.Instance.GetAmount(resourceDependency.ResourceType);

            // Debug.Log($"Resource multiplier for {currentAmount} {resourceDependency.ResourceType} is {resourceDependency.MultiplierCurve.Evaluate(currentAmount)}");
            return resourceDependency.MultiplierCurve.Evaluate(currentAmount);
        }
    }
}