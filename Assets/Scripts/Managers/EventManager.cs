using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Managers
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private int EventsPerMap;
        [SerializeField] private EventPanel EventPanel;
        [SerializeField] private EventButton[] EventPrefabs;
        [SerializeField] private RectTransform[] EventAnchors;

        private EventButton[] m_spawnedEvents;

        private void Awake()
        {
            Debug.Assert(EventPanel != null);
            Debug.Assert(EventPrefabs != null);
            Debug.Assert(EventPrefabs.Length > 0);
            Debug.Assert(EventAnchors != null);
            Debug.Assert(EventAnchors.Length > 0);
            Debug.Assert(EventAnchors.Distinct().Count() == EventAnchors.Length);
        }

        private void OnEnable()
        {
            TravelManager.Instance.NextLevelEvent.AddListener(SpawnEvents);
        }

        private void OnDisable()
        {
            TravelManager.Instance.NextLevelEvent.RemoveListener(SpawnEvents);
        }

        public void SpawnFirstSet()
        {
            if (m_spawnedEvents is { Length: > 0 })
            {
                return;
            }
            
            SpawnEvents();
        }
        
        private void SpawnEvents()
        {
            if (m_spawnedEvents != null)
            {
                foreach (EventButton eventButton in m_spawnedEvents)
                {
                    Destroy(eventButton.gameObject);
                }
            }

            m_spawnedEvents = new EventButton[EventsPerMap];
            
            List<RectTransform> unusedAnchors = EventAnchors.ToList();
            for (int i = 0; i < EventsPerMap; ++i)
            {
                int anchorIndex = Random.Range(0, unusedAnchors.Count);
                RectTransform anchor = unusedAnchors[anchorIndex];
                unusedAnchors.RemoveAt(anchorIndex);
                
                int buttonIndex = Random.Range(0, EventPrefabs.Length);
                m_spawnedEvents[i] = Instantiate(EventPrefabs[buttonIndex], anchor, false);
                m_spawnedEvents[i].EventPanel = EventPanel;
            }
        }
    }
}
