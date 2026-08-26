using System;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField] private RectTransform StatGroup;
        [SerializeField] private GameState[] GameStates = new GameState[3];

        [Serializable]
        private class GameState
        {
            public RectTransform PanelParent;
            public RectTransform StatParent;
            public UnityEvent InitiateStateAction;
        }

        [Serializable]
        public enum EStateType
        {
            Menu,
            Travel,
            Camp
        }

        public void Awake()
        {
            Debug.Assert(StatGroup != null);
            
            SetState(EStateType.Menu);
        }

        public void SetState(EStateType newState)
        {
            for (int i = 0; i < GameStates.Length; i++)
            {
                GameState state = GameStates[i];

                if (i == (int)newState)
                {
                    if (state.StatParent != null)
                    {
                        StatGroup.SetParent(state.StatParent, false);
                    }
                    
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
}