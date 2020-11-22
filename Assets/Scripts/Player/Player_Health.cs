using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class Player_Health : MonoBehaviourPun
{
    public float hitpoints = 100;

    Player_RagdollController ragdollController;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ragdollController = GetComponent<Player_RagdollController>();
    }

    //attackerPos - for direction of force
    //force - for force
    //hit position - for force on the point of the rigidbody
    //need to make this in to raise event
    public void KillImmediataly(float force, Vector3 hitPoint)
    {
        ragdollController.RagdollOn();
        rb.AddForceAtPosition((hitPoint - transform.position) * force, hitPoint, ForceMode.Impulse);
    }


    [PunRPC]
    void Damage(int playerViewId, int dmg)
    {
        if (playerViewId != photonView.ViewID)
            return;

        hitpoints -= dmg;
    }
}
