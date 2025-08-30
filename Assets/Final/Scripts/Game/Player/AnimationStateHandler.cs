using Fusion;
using UnityEngine;

public class AnimationStateHandler : NetworkBehaviour
{
    private const string VELOCITY = "Velocity";
    private const string THROW = "Throw";

    [SerializeField] private PlayerCharacterController controller;
    [SerializeField] private Animator animator;

    [Networked] private float Velocity { get; set; }

    public override void Spawned()
    {
        Velocity = 0.0f;
    }

    private void OnEnable()
    {
        if (Object == null) return;

        controller.OnRangedAttack += StartThrowAnimation;
        controller.OnStartMoving += StartMoveAnimation;
        controller.OnStopMoving += StopMoveAnimation;
    }
    private void OnDisable()
    {
        if (Object == null) return;

        controller.OnRangedAttack -= StartThrowAnimation;
        controller.OnStartMoving -= StartMoveAnimation;
        controller.OnStopMoving -= StopMoveAnimation;
    }
    public void StartMoveAnimation()
    {
        Velocity = 1.0f;
        animator.SetFloat(VELOCITY, Velocity);
    }
    
    public void StopMoveAnimation()
    {
        Velocity = 0.0f;
        animator.SetFloat(VELOCITY, Velocity);
    }

    public void StartThrowAnimation(Vector2 _)
    {
        animator.SetTrigger(THROW);
    }
}
