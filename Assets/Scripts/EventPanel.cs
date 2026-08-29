using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text DescriptionLabel;
    [SerializeField] private TMP_Text ResultLabel;
    [SerializeField] private Button TrustButton;
    [SerializeField] private Button RefuseButton;
    [SerializeField] private Button ContinueButton;

    private string m_trustText;
    private string m_refuseText;

    private void OnEnable()
    {
        // TODO: temp dummy data
        SetEvent("The wildlife and flora here are strange but plentiful.", "More hands make for a bountiful harvest. Your group has scavenged some fruits, nuts, and what looks like rabbits.",
            "You've never seen such plants and animals before. Who knows what they could do to your group?");
    }

    public void SetEvent(string description, string trustText, string refuseText)
    {
        TrustButton.gameObject.SetActive(true);
        RefuseButton.gameObject.SetActive(true);
        ContinueButton.gameObject.SetActive(false);
        ResultLabel.gameObject.SetActive(false);

        DescriptionLabel.text = description;

        m_trustText = trustText;
        m_refuseText = refuseText;
    }

    public void OnTrust()
    {
        TrustButton.gameObject.SetActive(false);
        RefuseButton.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(true);
        ResultLabel.gameObject.SetActive(true);

        DescriptionLabel.text = m_trustText;
    }

    public void OnRefuse()
    {
        TrustButton.gameObject.SetActive(false);
        RefuseButton.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(true);
        ResultLabel.gameObject.SetActive(true);

        DescriptionLabel.text = m_refuseText;
    }
}