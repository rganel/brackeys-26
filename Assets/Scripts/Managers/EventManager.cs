using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-400)]
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private int EventsPerCycle;
        [SerializeField] private EventPanel EventPanel;
        [SerializeField] private EventButton[] EventPrefabs;
        [SerializeField] private RectTransform[] EventAnchors;

        public static EventManager Instance { get; private set; }

        private readonly List<EventButton> m_spawnedEvents = new();

        private void Awake()
        {
            Debug.Assert(Instance == null);
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

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
            if (m_spawnedEvents.Count > 0)
            {
                return;
            }

            SpawnEvents();
        }

        public void HideEvents()
        {
            foreach (EventButton eventButton in m_spawnedEvents)
            {
                if (eventButton != null)
                {
                    eventButton.gameObject.SetActive(false);
                }
            }
        }

        public void ShowEvents()
        {
            foreach (EventButton eventButton in m_spawnedEvents)
            {
                if ((eventButton != null) && (eventButton.interactable))
                {
                    eventButton.gameObject.SetActive(true);
                }
            }
        }

        public void SpawnEvents()
        {
            List<EventButton> destroyedButtons = new();
            foreach (EventButton eventButton in m_spawnedEvents.Where(eventButton => (eventButton != null)))
            {
                Destroy(eventButton.gameObject);
                destroyedButtons.Add(eventButton);
            }

            m_spawnedEvents.RemoveAll(destroyedButtons.Contains);

            Dictionary<EntityId, List<RectTransform>> eventAnchorsByParent = EventAnchors.GroupBy(eventAnchor => eventAnchor.parent.GetEntityId()).ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());
            int eventsPerGroup = ((EventsPerCycle - m_spawnedEvents.Count) / eventAnchorsByParent.Count);
            foreach (List<RectTransform> unusedAnchors in eventAnchorsByParent.Select(kvp => kvp.Value))
            {
                for (int i = 0; i < eventsPerGroup; ++i)
                {
                    int anchorIndex = Random.Range(0, unusedAnchors.Count);
                    RectTransform anchor = unusedAnchors[anchorIndex];
                    unusedAnchors.RemoveAt(anchorIndex);

                    int buttonIndex = Random.Range(0, EventPrefabs.Length);
                    EventButton eventButton = Instantiate(EventPrefabs[buttonIndex], anchor, false);
                    eventButton.EventPanel = EventPanel;
                    m_spawnedEvents.Add(eventButton);
                }
            }
        }
    }
}