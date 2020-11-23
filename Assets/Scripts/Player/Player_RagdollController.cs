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
    //Make kinematic so we dont apply unnessesary force
    public Rigidbody[] ragdollRigidbodies;
    //Disable colliders to avoid weird network hitter because of photon view
    public Collider[] ragdollColliders;

    private Player_Config config;
    private Player_Respawner respawner;
    private Rigidbody rb;
    private bool isRagdoll = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        config = GetComponent<Player_Config>();

        //so we dont get unnessisary force applied, which slams the ragdoll
        foreach (Rigidbody rb in ragdollRigidbodies)
            rb.isKinematic = true;
        foreach (Collider col in ragdollColliders)
            col.enabled = false;
    }

    public void RagdollOn()
    {
        //enable human body for player

        //stop moving
        //add a canmove bool in config??
        isRagdoll = true;
        //rigidbodies hold on to the force and slams the ragdoll if not kinematic while animating
        config.humanAnimator.enabled = false;
        //if we are currently ragdoll
        foreach (Rigidbody rb in ragdollRigidbodies)
            rb.isKinematic = false;
        foreach (Collider col in ragdollColliders)
            col.enabled = true;
    }

    public void RagdollOff()
    {
        //rigidbodies hold on to the force and slams the ragdoll if not kinematic while animating
        config.humanAnimator.enabled = true;
        foreach (Rigidbody rb in ragdollRigidbodies)
            rb.isKinematic = true;
        foreach (Collider col in ragdollColliders)
            col.enabled = false;
        isRagdoll = false;
    }

    private void Update()
    {
        //Respawn
        if(isRagdoll)
        {
            //5 second death delay
            StartCoroutine(Respawn(5));
            isRagdoll = false;
        }
    }

    IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        photonView.RPC("RagdollToggle", RpcTarget.AllBufferedViaServer, photonView.ViewID, false);
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
