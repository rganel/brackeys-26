using System;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-900)]
public class GameStateManager : MonoBehaviour
{
    [SerializeField] private GameState[] GameStates = new GameState[(int)EStateType.META_Count];
 
    public bool Traveling => GameStates[(int)EStateType.Travel].PanelParent.gameObject.activeSelf;

    public static GameStateManager Instance { get; private set; }
    
    [Serializable]
    private class GameState
    {
        public RectTransform PanelParent;
        public UnityEvent InitiateStateAction;
    }
    
    [Serializable]
    public enum EStateType
    {
        Menu,
        Travel,
        Camp,
        
        META_Count
    };

    public void Awake()
    {
        Debug.Assert(Instance == null);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        
        SetState(EStateType.Menu);
    }

    public void SetState(EStateType newState)
    {
        for (int i = 0; i < GameStates.Length; i++)
        {
            GameState state = GameStates[i];

            if (i == (int)newState)
            {
                state.PanelParent.gameObject.SetActive(true);
                state.InitiateStateAction?.Invoke();
            }
            else
            {
                state.PanelParent.gameObject.SetActive(false);
            }
        }
    }

    public void SetStateMenu()
    {
        SetState(EStateType.Menu);
    }

    public void SetStateTravel()
    {
        SetState(EStateType.Travel);
    }

    public void SetStateCamp()
    {
        SetState(EStateType.Camp);
    }
}
