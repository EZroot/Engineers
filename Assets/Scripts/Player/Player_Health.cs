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
    Player_Hud hud;
    Player_Config config;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ragdollController = GetComponent<Player_RagdollController>();
        hud = GetComponent<Player_Hud>();
        config = GetComponent<Player_Config>();
    }

    private void Update()
    {
        hud.SetHealthHudText("HP " + hitpoints + "/100");

        if (hitpoints <= 0 && !hud.victoryScreen.activeSelf) //if crewmanager.intsance.everyonedead?
        {
            hud.victoryScreen.SetActive(true);
            config.ShowCursor();
        }
    }

    //not sure if pun sends vectors, i think it doesnt
    //Photonviews on rigidbody go through floor, idk why. colliders arent disabled on local player are they?
    [PunRPC]
    public void KillImmediately(int playerViewId, float force, Vector3 hitPoint)
    {
        if (playerViewId != photonView.ViewID)
            return;

        ragdollController.RagdollOn();
        rb.AddForceAtPosition((transform.forward + transform.up) * force, hitPoint, ForceMode.Impulse);
    }


    [PunRPC]
    public void Damage(int playerViewId, int dmg)
    {
        if (playerViewId != photonView.ViewID)
            return;

        hitpoints -= dmg;

        if (hitpoints <= 0)
            ragdollController.RagdollOn();
    }

    [PunRPC]
    public void SetHealth(int playerViewId, int amount)
    {
        if (playerViewId != photonView.ViewID)
            return;

        hitpoints = amount;
    }
}
