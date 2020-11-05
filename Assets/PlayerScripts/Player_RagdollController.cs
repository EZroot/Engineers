using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Make killimmediately a raise event
/// so we can send vector3s properly
/// </summary>
public class Player_RagdollController : MonoBehaviourPun
{
    //Disable to achieve ragdoll
    public Animator bodyAnimator;
    //Make kinematic so we dont apply unnessesary force
    public Rigidbody[] ragdollRigidbodies;
    //Disable colliders to avoid weird network hitter because of photon view
    public Collider[] ragdollColliders;

    private Player_Respawner respawner;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        //so we dont get unnessisary force applied, which slams the ragdoll
        foreach (Rigidbody rb in ragdollRigidbodies)
            rb.isKinematic = true;
        foreach (Collider col in ragdollColliders)
            col.enabled = false;
    }

    //attackerPos - for direction of force
    //force - for force
    //hit position - for force on the point of the rigidbody
    public void KillImmediataly(float force, Vector3 hitPoint)
    {
        RagdollOn();
        rb.AddForceAtPosition((hitPoint - transform.position) * force, hitPoint, ForceMode.Impulse);
    }

    void RagdollOn()
    {
        //rigidbodies hold on to the force and slams the ragdoll if not kinematic while animating
        bodyAnimator.enabled = false;
        //if we are currently ragdoll
        foreach (Rigidbody rb in ragdollRigidbodies)
            rb.isKinematic = false;
        foreach (Collider col in ragdollColliders)
            col.enabled = true;
    }

    void RagdollOff()
    {
        //rigidbodies hold on to the force and slams the ragdoll if not kinematic while animating
        bodyAnimator.enabled = true;
        foreach (Rigidbody rb in ragdollRigidbodies)
            rb.isKinematic = true;
        foreach (Collider col in ragdollColliders)
            col.enabled = false;
    }

    [PunRPC]
    void RagdollToggle(int playerViewId, bool goRagdoll)
    {
        if (playerViewId != photonView.ViewID)
            return;

        if(goRagdoll)
        {
            RagdollOn();
        }
        else
        {
            RagdollOff();
        }
    }
}
