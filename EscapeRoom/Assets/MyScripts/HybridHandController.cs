using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;

public class HybridHandController : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;

    [Header("Controller Input")]
    public InputActionProperty gripAction;
    public InputActionProperty triggerAction;

    [Header("Tracking Components")]
    public MonoBehaviour handSkeletonDriver;
    public MonoBehaviour handMeshController;

    [Header("Hand Tracking")]
    public XRHandSubsystem handSubsystem;
    public bool isLeftHand = true;

    void Update()
    {
        bool tracked = IsHandTracked();

        
        if (tracked)
        {
            if (handSkeletonDriver != null)
                handSkeletonDriver.enabled = true;

            if (handMeshController != null)
                handMeshController.enabled = true;

            if (animator != null)
                animator.enabled = false;
        }
        else
        {
            if (handSkeletonDriver != null)
                handSkeletonDriver.enabled = false;

            if (handMeshController != null)
                handMeshController.enabled = false;

            if (animator != null)
                animator.enabled = true;

            float grip = gripAction.action.ReadValue<float>();
            float trigger = triggerAction.action.ReadValue<float>();

            animator.SetFloat("Grip", grip);
            animator.SetFloat("Trigger", trigger);
        }
    }

    bool IsHandTracked()
    {
        if (handSubsystem == null)
            return false;

        XRHand hand = isLeftHand
            ? handSubsystem.leftHand
            : handSubsystem.rightHand;

        return hand.isTracked;
    }
}