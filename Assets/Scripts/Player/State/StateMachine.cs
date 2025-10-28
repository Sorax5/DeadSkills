using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class StateMachine
{
    private IState currentState;
    public IState CurrentState => currentState;

    private readonly List<IState> states = new List<IState>();
    private readonly List<Transition> transitions = new List<Transition>();

    public Animator animator;

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

        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void AddState(IState state)
    {
        if (!states.Contains(state))
        {
            states.Add(state);
            if (state is InputState) ((InputState)state).animator = animator;
        }
    }

    public void AddTransition(string fromStateName, string toStateName, Func<bool> condition)
    {
        transitions.Add(new Transition { FromName = fromStateName, ToName = toStateName, Condition = condition });
    }

    private Transition GetTransition()
    {
        if (currentState != null)
        {
            foreach (var t in transitions)
            {
                if (!string.IsNullOrEmpty(t.FromName) && t.FromName == currentState.Name && t.Condition())
                {
                    return t;
                }
            }
        }

        foreach (var t in transitions)
        {
            if (string.IsNullOrEmpty(t.FromName) && t.Condition())
            {
                return t;
            }
        }

        return null;
    }

    public IState GetStateByName(string name)
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
