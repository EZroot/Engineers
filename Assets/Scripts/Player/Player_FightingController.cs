using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_FightingController : MonoBehaviour
{
    public enum Weapon
    {
        Fists,
        Knife,
        Axe,
        Pistol
    }

    public Weapon selectedWeapon = Weapon.Fists;
    public float hitDistance = 4f;
    public string playerTag;
    public string[] rigidbodyTags;

    public float punchTimer = 1f;
    public float knifeTimer = .8f;
    public float axeTimer = 1.6f;
    public float pistolShotTimer = 1f;

    public float punchForce = 15f;
    public float imposterForceMultiplier = 3f;

    private bool isFighting = false;
    private bool punchCooldownOn = false;

    private Player_Config config;

    private void Start()
    {
        config = GetComponent<Player_Config>();
    }

    IEnumerator PunchCooldownTimer(Player_AnimationController controllerAnimation, Player_Controller controller)
    {
        isFighting = true;
        controllerAnimation.TriggerPunch();
        //controller.StopMoving = true;
        punchCooldownOn = true;
        yield return new WaitForSeconds(punchTimer);
        punchCooldownOn = false;
        //controller.StopMoving = false;
        controllerAnimation.ResetTriggerPunch();
        isFighting = false;
    }

    /*
     * NEED TO CREATE A CUSTOM TIMER FOR THIS OR we will always hit where we last hit it, even if we really miss
     */
    IEnumerator PunchDelay(float delayHitTimer, RaycastHit hit)
    {
        yield return new WaitForSeconds(delayHitTimer);
        //hit a draggable/door or something, apply force
        foreach (string tag in rigidbodyTags)
        {
            if (hit.transform.tag == tag)
            {
                //transfer ownership if available
                PhotonView otherPv = hit.transform.gameObject.GetComponent<PhotonView>();
                if (otherPv != null && !otherPv.IsMine)
                    otherPv.TransferOwnership(PhotonNetwork.LocalPlayer);

                //Apply force
                Rigidbody otherRb = hit.transform.gameObject.GetComponent<Rigidbody>();
                if (config.isImposter)
                {
                    //Rip off doors baby
                    HingeJoint doorhinge = hit.transform.gameObject.GetComponent<HingeJoint>();
                    if (doorhinge != null)
                    {
                        Destroy(doorhinge);
                    }

                    //hit obj
                    otherRb.AddForceAtPosition((hit.point - transform.position) * (punchForce * imposterForceMultiplier), hit.point, ForceMode.Impulse);
                }
                else
                    otherRb.AddForceAtPosition((hit.point - transform.position) * punchForce, hit.point, ForceMode.Impulse);
            }
        }
    }

    public void FightingControls(Player_Config config, Player_AnimationController controllerAnimation, Player_Controller controller)
    {
        if (Input.GetMouseButtonDown(1))
        {
            //so we can punch and move after animation is done
            if (isFighting)
                return;

            //Make sure no other coroutines are playing
            StopAllCoroutines();

            //Weapon selection
            switch (selectedWeapon)
            {
                case Weapon.Fists:
                    StartCoroutine(PunchCooldownTimer(controllerAnimation, controller));
                    break;
                case Weapon.Knife:
                    break;
            }

            //Action
            Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, hitDistance))
            {
                //imposters only
                if (config.isImposter)
                {
                    //if we hit another player
                    if (hit.transform.tag == playerTag)
                    {
                        //kill the player
                        PhotonView pvOther = hit.transform.gameObject.GetComponent<PhotonView>();
                        if (pvOther == null)
                            Debug.LogError("OTHER PLAYER DOESNT HAVE PHOTONVIEW?!");
                        pvOther.RPC("RagdollToggle", RpcTarget.AllBufferedViaServer, pvOther.ViewID, true);
                        //apply a force
                    }
                }

                //hit a draggable/door or something, apply force
                StartCoroutine(PunchDelay(0.25f, hit));
            }

        }
    }
}
