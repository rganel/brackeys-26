using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-800)]
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private Resource[] Resources = Enum.GetValues(typeof(EResourceType)).Cast<EResourceType>().Select(statType => new Resource(statType)).ToArray();

        public static ResourceManager Instance { get; private set; }

        [Serializable]
        private class Resource
        {
            [SerializeField] private EResourceType Type;
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

            foreach (Resource resource in Resources)
            {
                resource.UpdateLabel();
            }
        }

        public void AddResource(EResourceType resourceType, float amount)
        {
            Resource resource = Resources[(int)resourceType];
            resource.Amount = Mathf.Clamp(resource.Amount + amount, resource.MinValue, resource.MaxValue);
            resource.UpdateLabel();
        }
    }
}