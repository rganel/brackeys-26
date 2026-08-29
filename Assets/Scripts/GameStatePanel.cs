using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Managers;
using UnityEngine;
using UnityEngine.Events;

public class GameStatePanel : MonoBehaviour
{
    [SerializeField] private UnityEvent StateEnterEvent;
    [SerializeField] private UnityEvent StateExitEvent;
    [SerializeField] private EventReference[] AmbientSounds;
    [SerializeField] private EventReference BackgroundMusicEvent;
    [SerializeField] private EventReference[] StateEnterOneShots;
    [SerializeField] private EventReference[] StateExitOneShots;

    private UnityAction[] m_stopAmbientSoundsActions;
    
    private static List<GameStatePanel> s_gameStatePanels;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        s_gameStatePanels = new List<GameStatePanel>();
    }

    private void Awake()
    {
        Debug.Assert(!s_gameStatePanels.Contains(this));
        s_gameStatePanels.Add(this);
    }

    private void OnEnable()
    {
        foreach (GameStatePanel gameStatePanel in s_gameStatePanels)
        {
            if (gameStatePanel != this)
            {
                gameStatePanel.gameObject.SetActive(false);
            }
        }

        if (!BackgroundMusicEvent.IsNull)
        {
            AudioManager.Instance.PlayUntilReplaced(BackgroundMusicEvent);
        }

        m_stopAmbientSoundsActions = AmbientSounds.Select(ambientSound => AudioManager.Instance.PlayUntilStopped(ambientSound)).ToArray();
        StateEnterOneShots?.ToList().ForEach(oneShot => AudioManager.Instance.PlayOneShot(oneShot));

        StateEnterEvent?.Invoke();
    }

    private void OnDisable()
    {
        m_stopAmbientSoundsActions?.ToList().ForEach(action => action?.Invoke());
        m_stopAmbientSoundsActions = null;
        
        StateExitOneShots?.ToList().ForEach(oneShot => AudioManager.Instance.PlayOneShot(oneShot));
        
        StateExitEvent?.Invoke();
    }
}