using System;
using System.Collections.Generic;

using UnityEngine;

namespace IGI.StateMachine
{
    public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
    {
        protected Dictionary<EState, BaseState<EState>> states { get; set; } = new();
        protected BaseState<EState> currentState { get; set; }

        protected bool isTransitioningState = false;

        private void Start()
        {
            currentState.EnterState();
        }

        private void Update()
        {
            if (currentState == null)
            {
                Debug.LogError("BaseState is null");
                return;
            }

            Tick();

            EState nextState = currentState.GetNextState();

            if (!isTransitioningState && nextState.Equals(currentState.State))
            {
                currentState.UpdateState();
            }
            else if(!isTransitioningState)
            {
                TransitionToState(nextState);
            }
        }

        protected virtual void Tick() { }

        public virtual void TransitionToState(EState nextState)
        {
            if (states.ContainsKey(nextState) == false)
            {
                Debug.LogError("Dictionary States doesnt contains the " + nextState);
                return;
            }

            isTransitioningState = true;
            currentState.ExitState();
            currentState = states[nextState];
            currentState.EnterState();
            isTransitioningState = false;
        }
    }
}