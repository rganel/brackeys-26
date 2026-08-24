using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private StatTracker[] StatTrackers;
    [SerializeField] private Image TravelBackgroundImage;
    [SerializeField] private Sprite DaytimeSprite;
    [SerializeField] private Sprite NighttimeSprite;

    [SerializeField] private EStatType[] StatTypesToFloor;
    [SerializeField] private EStatType[] StatTypesToCeil;

    [Serializable]
    private class StatTracker
    {
        public string DisplayFormat;
        public TMP_Text DisplayLabel;

        public void UpdateLabel(int amount)
        {
            DisplayLabel.text = string.Format(DisplayFormat, amount);
        }
    }

    private void Awake()
    {
        Debug.Assert(StatTrackers != null);
        Debug.Assert(StatTrackers.Length == (int)EStatType.META_Count);
        Debug.Assert(TravelBackgroundImage != null);
        Debug.Assert(DaytimeSprite != null);
        Debug.Assert(NighttimeSprite != null);
        Debug.Assert(StatTypesToFloor != null);
        Debug.Assert(StatTypesToCeil != null);
        Debug.Assert(!StatTypesToFloor.Intersect(StatTypesToCeil).Any());
    }

    private void OnEnable()
    {
        TickManager.Instance.TickEvent.AddListener(OnTickEvent);
        ResourceManager.Instance.ResourceUpdated.AddListener(OnStatUpdated);
    }

    private void OnDisable()
    {
        TickManager.Instance.TickEvent.RemoveListener(OnTickEvent);
        ResourceManager.Instance.ResourceUpdated.RemoveListener(OnStatUpdated);
    }

    private void OnStatUpdated(EStatType statType, float value)
    {
        if (StatTypesToFloor.Contains(statType))
        {
            StatTrackers[(int)statType].UpdateLabel(Mathf.FloorToInt(value));
        }
        else if (StatTypesToCeil.Contains(statType))
        {
            StatTrackers[(int)statType].UpdateLabel(Mathf.CeilToInt(value));
        }
        else
        {
            StatTrackers[(int)statType].UpdateLabel(Mathf.RoundToInt(value));
        }
    }

    private void OnTickEvent(float tick)
    {
        TravelBackgroundImage.sprite = (tick % 1.0f <= 0.5f) ? DaytimeSprite : NighttimeSprite;

        OnStatUpdated(EStatType.Tick, Mathf.Floor(tick));
    }
}