using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameEndManager : MonoBehaviour
    {
        [SerializeField] private GameStatePanel GameEndPanel;
        [SerializeField] private TMP_Text ResultLabel;
        [SerializeField] private TMP_Text DescriptionLabel;

        private static readonly EResourceType[] s_loseConditions = { EResourceType.Population, EResourceType.Morale, EResourceType.Herd};
        
        private void OnEnable()
        {
            ResourceManager.Instance.ResourceAmountUpdated.AddListener(HandleResourceChanged);
            TravelManager.Instance.ReachedTowerEvent.AddListener(WinGame);
        }

        private void OnDisable()
        {
            ResourceManager.Instance.ResourceAmountUpdated.RemoveListener(HandleResourceChanged);
            TravelManager.Instance.ReachedTowerEvent.RemoveListener(WinGame);
        }
        
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void WinGame()
        {
            GameEndPanel.gameObject.SetActive(true);
            ResultLabel.text = "Journey's End";
            DescriptionLabel.text = $"You've reached the Tower in {ResourceManager.Instance.GetAmount(EResourceType.Day):N0} days with {ResourceManager.Instance.GetAmount(EResourceType.Population):N0} followers";
        }

        private void LoseGame(EResourceType loseReason)
        {
            GameEndPanel.gameObject.SetActive(true);
            
            switch (loseReason)
            {
                case EResourceType.Population:
                    ResultLabel.text = "Journey Failed";
                    DescriptionLabel.text = "You've lost your caravan and are unable to finish the journey on your own";
                    break;

                case EResourceType.Morale:
                    ResultLabel.text = "Faith Broken";
                    DescriptionLabel.text = "You've lost the trust of your followers and have been abandoned to fend for yourself";
                    break;
            
                case EResourceType.Herd:
                    ResultLabel.text = "Journey Failed";
                    DescriptionLabel.text = "Your caravan ran out of supplies and was unable to complete its journey";
                    break;
            
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private void HandleResourceChanged(EResourceType resourceType, float amount)
        {
            if (s_loseConditions.Any(loseCondition => (loseCondition == resourceType)) && (amount <= 0.0f))
            {
                LoseGame(resourceType);
            }
        }
    }
}