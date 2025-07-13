using UnityEngine;
using Fusion;

public class AnimationStateHandler : NetworkBehaviour
{
    private const string VELOCITY = "Velocity";
    private const string THROW = "Throw";
    
    private Animator animator;

    [Networked] private float Velocity { get; set; }

    public override void Spawned()
    {
        Velocity = 0.0f;
        animator.SetFloat(VELOCITY, Velocity);
    }

    public void StartMoveAnimation()
    {
        Debug.Log("StartMoveAnimation");
        Velocity = 1.0f;
        animator.SetFloat(VELOCITY, Velocity);
    }
    
    public void StopMoveAnimation()
    {
        Velocity = 0.0f;
        animator.SetFloat(VELOCITY, Velocity);
    }

    public void StartThrowAnimation()
    {
        animator.SetTrigger(THROW);
    }
}
