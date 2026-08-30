using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EventPanel : MonoBehaviour
    {
        [SerializeField] private Image EventImage;
        [SerializeField] private TMP_Text DescriptionLabel;
        [SerializeField] private TMP_Text TrustResultLabel;
        [SerializeField] private TMP_Text RefuseResultLabel;
        [SerializeField] private ActivityButton TrustButton;
        [SerializeField] private ActivityButton RefuseButton;
        [SerializeField] private Button ContinueButton;

        private EventSO m_eventDefinition;
        
        public void SetEvent(EventSO eventDefinition)
        {
            gameObject.SetActive(true);
            
            TrustButton.gameObject.SetActive(true);
            RefuseButton.gameObject.SetActive(true);
            ContinueButton.gameObject.SetActive(false);
            TrustResultLabel.gameObject.SetActive(false);
            RefuseResultLabel.gameObject.SetActive(false);

            TrustButton.interactable = true;
            RefuseButton.interactable = true;
            
            EventImage.sprite = eventDefinition.HoverSprite;
            DescriptionLabel.text = eventDefinition.Description;

            TrustButton.SetActivity(eventDefinition.TrustActivity);
            RefuseButton.SetActivity(eventDefinition.RejectActivity);
            
            m_eventDefinition = eventDefinition;
        }

        private void Progress()
        {
            TrustButton.gameObject.SetActive(false);
            RefuseButton.gameObject.SetActive(false);
            ContinueButton.gameObject.SetActive(true);
        }

        public void OnTrust()
        {
            Progress();

            DescriptionLabel.text = m_eventDefinition.TrustDescription;
            TrustResultLabel.gameObject.SetActive(true);
        }

        public void OnRefuse()
        {
            Progress();

            DescriptionLabel.text = m_eventDefinition.RejectDescription;
            RefuseResultLabel.gameObject.SetActive(true);
        }
    }
}