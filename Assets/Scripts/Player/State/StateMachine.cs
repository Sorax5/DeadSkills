using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState currentState;
    public IState CurrentState => currentState;

    private readonly List<IState> states = new List<IState>();
    private readonly List<Transition> transitions = new List<Transition>();

    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
        {
            var toState = GetStateByName(transition.ToName);
            if (toState != null)
            {
                SetState(toState);
            }
            else
            {
                Debug.LogWarning($"StateMachine: Transition target state '{transition.ToName}' not found");
            }
        }

        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void SetState(IState state)
    {
        if (state == currentState)
        {
            return;
        }

        Debug.Log($"StateMachine: Transition {currentState?.Name ?? "null"} -> {state?.Name}");

        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void AddState(IState state)
    {
        if (!states.Contains(state))
        {
            states.Add(state);
        }
    }

    public void AddTransition(string fromStateName, string toStateName, Func<bool> condition)
    {
        transitions.Add(new Transition { FromName = fromStateName, ToName = toStateName, Condition = condition });
    }

    private Transition GetTransition()
    {
        foreach (var t in transitions)
        {
            bool fromMatches = false;
            if (string.IsNullOrEmpty(t.FromName))
            {
                fromMatches = true; // global transition
            }
            else if (currentState != null && t.FromName == currentState.Name)
            {
                fromMatches = true;
            }

            if (fromMatches && t.Condition())
            {
                return t;
            }
        }
        return null;
    }

    private IState GetStateByName(string name)
    {
        foreach (var s in states)
        {
            if (s.Name == name)
            {
                return s;
            }
        }
        return null;
    }
}
