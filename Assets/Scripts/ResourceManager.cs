using System;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-800)]
public class ResourceManager : MonoBehaviour
{
    [SerializeField] private Resource[] Resources;

    public UnityEvent<EStatType, float> ResourceUpdated;

    public static ResourceManager Instance { get; private set; }

    [Serializable]
    private class Resource
    {
        public float MinValue = float.MinValue;
        public float MaxValue = float.MaxValue;
        public float Amount;
        public float TravelDecayRate;
        public float RestRestoreRate;
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
        Debug.Assert(Resources.Length == (int)EStatType.META_LastResource - (int)EStatType.META_FirstResource + 1);
    }

    private void OnEnable()
    {
        TickManager.Instance.TickEvent.AddListener(OnTickEvent);
    }

    private void OnDisable()
    {
        TickManager.Instance.TickEvent.RemoveListener(OnTickEvent);
    }

    public void AddResource(EStatType statType, float amount)
    {
        Debug.Assert((statType >= EStatType.META_FirstResource) && (statType <= EStatType.META_LastResource));

        Resource resource = Resources[(int)statType];
        resource.Amount = Mathf.Clamp(resource.Amount + amount, resource.MinValue, resource.MaxValue);
        ResourceUpdated?.Invoke(statType, resource.Amount);
    }

    private void OnTickEvent(float tick)
    {
        for (EStatType statType = EStatType.META_FirstResource; statType <= EStatType.META_LastResource; statType++)
        {
            Resource resource = Resources[(int)statType];
            AddResource(statType, (GameStateManager.Instance.Traveling ? -resource.TravelDecayRate : resource.RestRestoreRate));
        }
    }
}