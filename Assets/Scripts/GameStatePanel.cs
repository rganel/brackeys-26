using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStatePanel : MonoBehaviour
{
    [SerializeField] private UnityEvent StateEnterEvent;
    [SerializeField] private UnityEvent StateExitEvent;

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

        StateEnterEvent?.Invoke();
    }

    private void OnDisable()
    {
        StateExitEvent?.Invoke();
    }
}