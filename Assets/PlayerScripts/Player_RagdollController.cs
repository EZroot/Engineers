using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RagdollController : MonoBehaviour
{
    //Disable to achieve ragdoll
    public Animator bodyAnimator;
    public Rigidbody[] ragdollRigidbodies;

    //if this is enabled, turn off kinematics for ragdoll
    public bool isRagdoll = false;

    private void Start()
    {
        //so we dont get unnessisary force applied, which slams the ragdoll
        foreach (Rigidbody rb in ragdollRigidbodies)
            rb.isKinematic = true;
    }

    /// <summary>
    /// Toggle Ragdoll between On/Off
    /// </summary>
    public void RagdollToggle()
    {
        //rigidbodies hold on to the force and slams the ragdoll if not kinematic while animating
        bodyAnimator.enabled = !bodyAnimator.enabled;
        if (!bodyAnimator.enabled)
        {
            //if we are currently ragdoll
            foreach (Rigidbody rb in ragdollRigidbodies)
                rb.isKinematic = false;
        }
        else
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
                rb.isKinematic = true;
        }
    }
}
