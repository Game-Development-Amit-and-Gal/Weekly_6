using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [Header("States")]
    [SerializeField] private WalkState walkState;
    [SerializeField] private BoatState boatState;

    [Header("Interaction")]
    [SerializeField] private PlayerBoatInteraction boatInteraction;

    private void Awake()
    {
        this.AddState(walkState)
            .AddState(boatState);

        AddTransition(
            walkState,
            () =>
            {
                if (boatInteraction.ShouldEnterBoat())
                {
                    boatInteraction.MountBoat();
                    return true;
                }
                return false;
            },
            boatState);

        AddTransition(
            boatState,
            () =>
            {
                if (boatInteraction.ShouldExitBoat())
                {
                    boatInteraction.DismountBoat();
                    return true;
                }
                return false;
            },
            walkState);
    }
}
