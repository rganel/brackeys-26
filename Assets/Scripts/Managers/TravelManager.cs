using System.Collections;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    [DefaultExecutionOrder(-1000)]
    public class TravelManager : MonoBehaviour
    {
        [SerializeField] private ActivitySO TravelActivity;
        [SerializeField] private float TravelTickInterval;

        [SerializeField] private Image TravelBackgroundImage;
        [SerializeField] private Sprite DaytimeSprite;
        [SerializeField] private Sprite NighttimeSprite;

        public static TravelManager Instance { get; private set; }

        private Coroutine m_tickCoroutine;

        private void Awake()
        {
            Debug.Assert(Instance == null);
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Debug.Assert(TravelActivity != null);
            Debug.Assert(TravelTickInterval > 0.0f);

            Debug.Assert(TravelBackgroundImage != null);
            Debug.Assert(DaytimeSprite != null);
            Debug.Assert(NighttimeSprite != null);
        }

        public void BeginTravel()
        {
            if (m_tickCoroutine != null)
            {
                StopCoroutine(m_tickCoroutine);
            }

            m_tickCoroutine = StartCoroutine(TickHandler());
        }

        public void PauseTravel()
        {
            if (m_tickCoroutine != null)
            {
                StopCoroutine(m_tickCoroutine);
                m_tickCoroutine = null;
            }
        }

        private IEnumerator TickHandler()
        {
            while (true)
            {
                yield return new WaitForSeconds(TravelTickInterval / 2);
                TravelBackgroundImage.sprite = NighttimeSprite;

                yield return new WaitForSeconds(TravelTickInterval / 2);
                TravelBackgroundImage.sprite = DaytimeSprite;

                // TODO: confirm activity can be done
                foreach (ActivitySO.ResourceChange resourceChange in TravelActivity.ResourceChanges)
                {
                    ResourceManager.Instance.AddResource(resourceChange.ResourceType, resourceChange.AmountChange);
                }
            }
        }
    }
}