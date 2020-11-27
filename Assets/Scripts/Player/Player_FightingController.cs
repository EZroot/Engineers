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

    public string playerTag;
    public string[] rigidbodyTags;

    public float punchTimer = .7f;
    public float knifeTimer = .8f;
    public float axeTimer = 1.6f;
    public float pistolShotTimer = 1f;

    public float punchForce = 15f;
    public float imposterForceMultiplier = 3f;

    public LayerMask playerCapsuleLayermask;

    private float hitDistance = 2f;
    private float hitDamage = 15f;

    private bool isFighting = false;
    private bool punchCooldownOn = false;

    private Player_Config config;
    private Player_AnimationController controllerAnimation;

    private void Start()
    {
        config = GetComponent<Player_Config>();
        controllerAnimation = GetComponent<Player_AnimationController>();

        SelectWeaponModel(selectedWeapon);
        config.humanAnimator.SetLayerWeight(1, 0f);
    }

    bool playedClip = false;

    IEnumerator PunchCooldownTimer(Player_AnimationController controllerAnimation)
    {
        config.humanAnimator.SetLayerWeight(1, 1f);
        isFighting = true;
        controllerAnimation.TriggerPunch();
        config.firstAudioSource.clip = config.punchClip;
        if (!config.firstAudioSource.isPlaying && !playedClip)
        {
            config.firstAudioSource.Play();
            playedClip = true;
        }
        //controller.StopMoving = true;
        punchCooldownOn = true;
        yield return new WaitForSeconds(punchTimer);
            config.firstAudioSource.Stop();
        punchCooldownOn = false;
        //controller.StopMoving = false;
        controllerAnimation.ResetTriggerPunch();
        isFighting = false;
        playedClip = false;
        config.humanAnimator.SetLayerWeight(1, 0f);

    }

    IEnumerator StabCooldownTimer(Player_AnimationController controllerAnimation)
    {
        isFighting = true;
        controllerAnimation.TriggerStab();
        config.firstAudioSource.clip = config.knifeClip;
        if (!config.firstAudioSource.isPlaying && !playedClip)
        {
            config.firstAudioSource.Play();
            playedClip = true;
        }
        //controller.StopMoving = true;
        punchCooldownOn = true;
        yield return new WaitForSeconds(punchTimer);
            config.firstAudioSource.Stop();
        punchCooldownOn = false;
        //controller.StopMoving = false;
        controllerAnimation.ResetTriggerStab();
        isFighting = false;
        playedClip = false;

    }

    IEnumerator ShootCooldownTimer(Player_AnimationController controllerAnimation)
    {
        isFighting = true;
        controllerAnimation.TriggerShoot();
        config.firstAudioSource.clip = config.shootClip;
        if (!config.firstAudioSource.isPlaying && !playedClip)
        {
            config.firstAudioSource.Play();
            playedClip = true;
        }
        //controller.StopMoving = true;
        punchCooldownOn = true;
        yield return new WaitForSeconds(punchTimer);
            config.firstAudioSource.Stop();
        punchCooldownOn = false;
        //controller.StopMoving = false;
        controllerAnimation.ResetTriggerShoot();
        isFighting = false;
        playedClip=false;
    }

    /*
     * NEED TO CREATE A CUSTOM TIMER FOR THIS OR we will always hit where we last hit it, even if we really miss
     */
    IEnumerator PunchDelay(float delayHitTimer, RaycastHit hit)
    {
        config.secondAudioSource.Stop();
        yield return new WaitForSeconds(delayHitTimer);
        //hit a draggable/door or something, apply force
        foreach (string tag in rigidbodyTags)
        {
            if (hit.transform.tag == tag)
            {
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

                config.secondAudioSource.clip = config.punchHitClip;
                config.secondAudioSource.Play();
            }
        }
    }

    private void Update()
    {
        FightingControls(config, controllerAnimation);
    }

    public void SelectWeaponModel(Weapon weaponType)
    {
        //Weapon selection
        switch (weaponType)
        {
            case Weapon.Fists:
                config.humanFPSFists.SetActive(true);
                config.humanFPSKnife.SetActive(false);
                config.humanFPSPistol.SetActive(false);

                break;
            case Weapon.Knife:
                config.humanFPSFists.SetActive(false);
                config.humanFPSKnife.SetActive(true);
                config.humanFPSPistol.SetActive(false);

                break;
            case Weapon.Pistol:
                config.humanFPSFists.SetActive(false);
                config.humanFPSKnife.SetActive(false);
                config.humanFPSPistol.SetActive(true);
                break;
        }
        selectedWeapon = weaponType;
    }

    public void FightingControls(Player_Config config, Player_AnimationController controllerAnimation)
    {
        if (!config.canAttack)
            return;

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
                    hitDistance = 1.7f;
                    hitDamage = 20f;
                    StartCoroutine(PunchCooldownTimer(controllerAnimation));
                    break;
                case Weapon.Knife:
                    hitDistance = 1.7f;
                    hitDamage = 33;
                    StartCoroutine(StabCooldownTimer(controllerAnimation));
                    break;
                case Weapon.Pistol:
                    hitDistance = 22f;
                    hitDamage = 75f;
                    StartCoroutine(ShootCooldownTimer(controllerAnimation));
                    break;
            }

            //Action
            Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, hitDistance, ~playerCapsuleLayermask))
            {
                if (hit.transform.tag == "PlayerRagdoll")
                {
                    Debug.Log("Punched " + hit.collider.gameObject.name + " On other player!");

                    //damage and kill the player
                    PhotonView pvOther = hit.transform.root.gameObject.GetComponent<PhotonView>();
                    if (pvOther == null)
                        Debug.LogError("OTHER PLAYER DOESNT HAVE PHOTONVIEW?!");
                    pvOther.RPC("Damage", RpcTarget.AllBufferedViaServer, pvOther.ViewID, DamageMultiplier(hit.collider.gameObject.name, hitDamage));
                    //pvOther.RPC("KillImmediately", RpcTarget.AllBufferedViaServer, pvOther.ViewID, 15f, hit.point);

                }

                //transfer ownership if available (punching obj)
                PhotonView otherPv = hit.transform.gameObject.GetComponent<PhotonView>();
                if (otherPv != null && !otherPv.IsMine)
                    otherPv.TransferOwnership(PhotonNetwork.LocalPlayer);


                //hit a draggable/door or something, apply force
                StartCoroutine(PunchDelay(0.25f, hit));
            }
        }
    }

    int DamageMultiplier(string bodyPartName, float hitAmount)
    {
        float dmg = hitAmount;
        switch(bodyPartName)
        {
            case "lowerarm_r":
                dmg *= 0.3f;
                break;
            case "lowerarm_l":
                dmg *= 0.3f;
                break;
            case "upperarm_r":
                dmg *= 0.7f;
                break;
            case "upperarm_l":
                dmg *= 0.7f;
                break;
            case "thigh_r":
                dmg *= 0.7f;
                break;
            case "thigh_l":
                dmg *= 0.7f;
                break;
            case "pelvis":
                dmg *= 1f;
                break;
            case "head":
                dmg *= 2f;
                break;
        }
        return (int)dmg;
    }
}
