using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Managers
{
    [DefaultExecutionOrder(-700)]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] [ParamRef] private string MoraleParameter;
        [SerializeField] private EventReference TrustSound;
        [SerializeField] private EventReference RejectSound;

        public static AudioManager Instance { get; private set; }

        private EventInstance m_eventInstance;
        private PARAMETER_DESCRIPTION m_moraleParameter;

        private void Awake()
        {
            Debug.Assert(Instance == null);
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            RuntimeManager.StudioSystem.getParameterDescriptionByName(MoraleParameter, out m_moraleParameter);
            Debug.Assert(m_moraleParameter.name == MoraleParameter);
        }

        private void OnEnable()
        {
            ResourceManager.Instance.ResourceAmountUpdated.AddListener(HandleResourceUpdated);
        }

        private void OnDisable()
        {
            ResourceManager.Instance.ResourceAmountUpdated.RemoveListener(HandleResourceUpdated);
        }

        private void OnDestroy()
        {
            if (!m_eventInstance.isValid())
            {
                return;
            }

            m_eventInstance.stop(STOP_MODE.IMMEDIATE);
            m_eventInstance.release();
        }

        public void PlayOneShot(EventReference eventReference)
        {
            RuntimeManager.PlayOneShot(eventReference);
        }

        public void PlayUntilReplaced(EventReference eventReference)
        {
            if (IsInstanceBoundTo(eventReference))
            {
                // Already playing this event
                return;
            }

            if (m_eventInstance.isValid())
            {
                Debug.LogWarning("Stop old music");
                m_eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
                m_eventInstance.release();
            }

            Debug.LogWarning("Start new music");
            m_eventInstance = RuntimeManager.CreateInstance(eventReference);
            m_eventInstance.start();
        }

        public void PlayTrustSound()
        {
            RuntimeManager.PlayOneShot(TrustSound);
        }

        public void PlayRejectSound()
        {
            RuntimeManager.PlayOneShot(RejectSound);
        }

        public UnityAction PlayUntilStopped(EventReference eventReference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.start();

            return () =>
            {
                eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
                eventInstance.release();
            };
        }

        private void HandleResourceUpdated(EResourceType resourceType, float amount)
        {
            if (resourceType != EResourceType.Morale)
            {
                return;
            }

            Debug.Log("audio set health to " + amount);

            RuntimeManager.StudioSystem.setParameterByID(m_moraleParameter.id, amount);
        }

        private bool IsInstanceBoundTo(EventReference eventReference)
        {
            if (!m_eventInstance.isValid())
            {
                return false;
            }

            m_eventInstance.getDescription(out EventDescription eventInstanceDescription);
            eventInstanceDescription.getID(out GUID eventInstanceGuid);

            return (eventReference.Guid == eventInstanceGuid);
        }
    }
}