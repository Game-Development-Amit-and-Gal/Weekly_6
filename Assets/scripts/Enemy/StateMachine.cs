using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


using State = UnityEngine.MonoBehaviour;
using Transition = System.Tuple<UnityEngine.MonoBehaviour, System.Func<bool>, UnityEngine.MonoBehaviour>;

/**
 * This component represents a generic state-machine.
 * Each state is represented by a component installed on the same object as the StateMachine component.
 * At each transition, the machine enables the active state and disables all other states.
 * The first state added is the first active state.
 */
public class StateMachine : MonoBehaviour
{
    //[SerializeField]
    private List<State> states = new();     // List of states in the state machine

    //[SerializeField]
    private List<Transition> transitions = new();   // List of transitions between states

    private State activeState = null;               // Currently active state




    public void GoToState(State newActiveState)             // Method to transition to a new state
    {
        if (activeState == newActiveState) return;          // No transition if already in the desired state
        if (activeState != null) activeState.enabled = false;   // Disable the current active state

        activeState = newActiveState;                           // Update the active state
        activeState.enabled = true;                             // Enable the new active state
        Debug.Log("Going to state " + activeState);             // Log the state transition
    }

    public StateMachine AddState(State newState)        // Method to add a new state to the state machine
    {
        states.Add(newState);                           // Add the new state to the list of states
        return this;                                    // Return the state machine for method chaining
    }

    public StateMachine AddTransition(State fromState, Func<bool> condition, State toState)     // Method to add a transition between states
    {
        transitions.Add(new Transition(fromState, condition, toState));                         // Add the new transition to the list of transitions
        return this;                                                                            // Return the state machine for method chaining
    }

    public void Initialize()                                // Method to initialize the state machine
    {
        foreach (State state in states)                     // Disable all states initially
        {
            state.enabled = false;                          // Disable the state
        }

        int zero = 0;                                       // To avoid magic number
        int first = 0;
        if (states.Count > zero)                               // If there are states added
            GoToState(states[first]);
        else
            Debug.LogError("StateMachine: No states were added before Initialize() was called.");
    }


    private void Update() // Unity's update method called once per frame
    {
        foreach (Transition transition in transitions)          // Iterate through all transitions
        {
            if (transition.Item1 == activeState)                // Check if the transition is from the active state
            {
                if (transition.Item2() == true)                 // Check if the transition condition is met
                {
                    GoToState(transition.Item3);                // Transition to the new state
                    break;
                }
            }
        }
    }
}
