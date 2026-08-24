using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1000)]
public class ResourceManager : MonoBehaviour
{
    [SerializeField] private bool Resting;
    [SerializeField] private float TickRate;
    [SerializeField] private Resource[] Resources;

    public UnityEvent<ResourceType, float> ResourceUpdated;

    public static ResourceManager Instance { get; private set; }
    
    public enum ResourceType
    {
        Tick,
        Population,
        Morale,
        Herd,

        Count
    };

    [Serializable]
    private class Resource
    {
        public ResourceType Type;
        public float Amount;
        public float TravelDecayRate;
        public float RestRestoreRate;
        public string DisplayFormat;
        public TMP_Text DisplayLabel;
    }

    private void Awake()
    {
        Debug.Assert(Instance == null);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        
        Debug.Assert(Resources != null);
        Debug.Assert(Resources.Length == (int)ResourceType.Count);
        Debug.Assert(Resources.DistinctBy(resource => resource.Type).Count() == Resources.Length);
        Debug.Assert(Resources.All(resource => resource.Type != ResourceType.Count));

        foreach (Resource resource in Resources)
        {
            resource.DisplayLabel.text = string.Format(resource.DisplayFormat, Mathf.RoundToInt(resource.Amount));
        }
    }

    private void Update()
    {
        if (Time.time - Resources[(int)ResourceType.Tick].Amount >= TickRate)
        {
            Tick();
        }
    }

    private void Tick()
    {
        foreach (Resource resource in Resources)
        {
            if (Resting)
            {
                resource.Amount += resource.RestRestoreRate;
            }
            else
            {
                resource.Amount -= resource.TravelDecayRate;
            }
            
            resource.DisplayLabel.text = string.Format(resource.DisplayFormat, Mathf.RoundToInt(resource.Amount));
            ResourceUpdated?.Invoke(resource.Type, resource.Amount);
            
            // // Special handling to allow mid-tick updates in other systems (i.e. day/ night)
            // if ((resource.Type == ResourceType.Tick) && (resource.Amount % 1 != 0))
            // {
            //     break;
            // }
        }
    }
}