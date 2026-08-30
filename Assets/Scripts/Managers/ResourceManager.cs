using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    [DefaultExecutionOrder(-800)]
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private Resource[] Resources = Enum.GetValues(typeof(EResourceType)).Cast<EResourceType>().Select(statType => new Resource(statType)).ToArray();

        public UnityEvent<EResourceType, float> ResourceAmountUpdated;

        public static ResourceManager Instance { get; private set; }

        [Serializable]
        private class Resource
        {
            [field: SerializeField] public EResourceType Type { get; private set; }
            public float MinValue = float.MinValue;
            public float MaxValue = float.MaxValue;

            public float Amount;
            public string DisplayFormat;
            public TMP_Text DisplayLabel;

            public Resource(EResourceType type)
            {
                Type = type;
            }

            public void UpdateLabel()
            {
                DisplayLabel.text = string.Format(DisplayFormat, Amount, Type);
            }
        }

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

        private void OnEnable()
        {
            foreach (Resource resource in Resources)
            {
                resource.UpdateLabel();
                ResourceAmountUpdated?.Invoke(resource.Type, resource.Amount);
            }
        }

        public void AddResource(EResourceType resourceType, float amount)
        {
            Resource resource = Resources[(int)resourceType];
            resource.Amount = Mathf.Clamp(resource.Amount + amount, resource.MinValue, resource.MaxValue);
            resource.UpdateLabel();

            ResourceAmountUpdated?.Invoke(resourceType, amount);
        }

        public float GetAmount(EResourceType resourceType)
        {
            return Resources[(int)resourceType].Amount;
        }
    }
}