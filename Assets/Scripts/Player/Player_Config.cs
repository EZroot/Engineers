using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_Config : MonoBehaviourPun
{
    //Syncing human + mosnter model
    public GameObject monster;
    public GameObject[] human;

    public Animator humanAnimator;
    public Animator monsterAnimator;
    public Animator handsAnimator;

    public bool hideCursor = true;
    //Role
    public bool isImposter;
    public bool isEngineer;
    public bool isChemist;
    public bool isBotanist;
    public bool isInvestigator;

    //speed
    public int walkSpeed = 2;
    public int runSpeed = 5;

    //Stamina
    public int staminaLevel = 60;
    public int staminaLevelForRecover = 15;

    [HideInInspector]
    public bool canRun = true;
    [HideInInspector]
    public bool staminaExhausted = false;
    [HideInInspector]
    public float runStamina;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = !hideCursor;
    }

    [PunRPC]
    void PickImposterRPC(int viewID, bool imposter)
    {
        if(viewID==photonView.ViewID)
        {
            isImposter = imposter;

            //Hacked together, needs to be fixed since we should only turn in to a mosnter when the player attacks, and not if hes just imposter
            if (isImposter)
            {
                monster.SetActive(true);
                foreach (GameObject o in human)
                {
                    o.SetActive(false);
                }
            }
            if (!isImposter)
            {
                monster.SetActive(false);
                foreach (GameObject o in human)
                {
                    o.SetActive(true);
                }
            }
        }
    }
}
