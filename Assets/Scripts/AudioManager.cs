using FMOD.Studio;
using FMODUnity;
using Managers;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private EventReference MenuMusicEvent;
    [SerializeField] private EventReference GameMusicEvent;

    [SerializeField][ParamRef] private string MoraleParameter;

    private EventInstance m_eventInstance;
    private PARAMETER_DESCRIPTION m_moraleParameter;

    private void Awake()
    {
        Debug.Assert(!MenuMusicEvent.IsNull);
        Debug.Assert(!GameMusicEvent.IsNull);
        
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
        m_eventInstance.stop(STOP_MODE.IMMEDIATE);
        m_eventInstance.release();
    }

    public void PlayMenuMusic()
    {
        Play(MenuMusicEvent);
    }
    
    public void PlayGameMusic()
    {
        Play(GameMusicEvent);
    }
    
    private void Play(EventReference eventReference)
    {
        if (IsInstanceBoundTo(eventReference))
        {
            // Already playing this event
            return;
        }
        
        m_eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        m_eventInstance.release();
        
        m_eventInstance = RuntimeManager.CreateInstance(eventReference);
        m_eventInstance.start();
    }
    
    private void HandleResourceUpdated(EResourceType resourceType, float amount)
    {
        if (resourceType != EResourceType.Morale)
        {
            return;
        }
        
        RuntimeManager.StudioSystem.setParameterByID(m_moraleParameter.id, amount);
    }

    private bool IsInstanceBoundTo(EventReference eventReference)
    {
        m_eventInstance.getDescription(out EventDescription eventInstanceDescription);
        eventInstanceDescription.getID(out FMOD.GUID eventInstanceGuid);

        return (eventReference.Guid == eventInstanceGuid);
    }
}