using CMF;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player_Controller : MonoBehaviourPun
{
    //to rotate body model
    public Transform cameraTransform;
    public Transform bodyTransform;
    public Light flashLight;

    //components
    private AdvancedWalkerController controllerWalker;
    private Player_AnimationController controllerAnimation;
    private Player_RagdollController controllerRagdoll;
    private Player_FightingController controllerFighting;
    private Player_Config config;
    private Player_Hud hud;

    private bool monstersHidden = false;
    //movement
    private bool stopMoving = false;
    public bool StopMoving { get { return stopMoving; } set { stopMoving = value; } }

    private void Start()
    {
        //movement controller
        controllerWalker = GetComponent<AdvancedWalkerController>();
        controllerAnimation = GetComponent<Player_AnimationController>();
        controllerRagdoll = GetComponent<Player_RagdollController>();
        controllerFighting = GetComponent<Player_FightingController>();

        config = GetComponent<Player_Config>();
        hud = GetComponent<Player_Hud>();

        //stamina
        config.runStamina = config.staminaLevel;

        //hud
        StartCoroutine("UpdateHUD");
    }

    private void Update()
    {
        FlashLightControls();
        RunningControls(config);
        //Melee/Gun play
        controllerFighting.FightingControls(config, controllerAnimation, this);
        
        HighlightGrabbedObject(4f, "Interactive");
        HighlightInteractables();

        //prevents monster model from showing to myself
        if (config.isImposter && !monstersHidden)
        {
            for(int i =0; i < config.monsterGraphics.Length-1;i++) //hard coded to not hide hands
            {
                config.monsterGraphics[i].SetActive(false);
            }
            monstersHidden = true;
        }

        UpdateMovementStatus();
    }

    /// <summary>
    /// Show the clients name that youre looking at
    /// </summary>
    private void HighlightInteractables()
    {
        RaycastHit hit;
        Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.transform.tag == "Player")
            {
                hud.SetIdentityText(hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName);
            }
            if (hit.transform.tag == "Door")
            {
                hud.SetIdentityText("Door ");
            }
            if (hit.transform.tag == "Button")
            {
                Task_DoorNumpad numpad = hit.transform.gameObject.GetComponent<Task_DoorNumpad>();
                if (numpad != null)
                {
                    if(numpad.unlocked)
                        hud.SetIdentityText("Door Unlocked");
                    else
                        hud.SetIdentityText("Door Locked");
                }
                else
                {
                    hud.SetIdentityText("Button");
                }
            }
            if(hit.transform.tag=="Untagged")
            {
                hud.SetIdentityText("");
            }
        }
        else
        {
            hud.SetIdentityText("");
        }
    }

    /// <summary>
    /// Turning off and on the flashlight, synced in player sync
    /// </summary>
    private void FlashLightControls()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Enabling flashlight");
            flashLight.enabled = !flashLight.enabled;
        }
    }

    /// <summary>
    /// Indicate were grabbing an object, potentially make it glow/highlight with a shader to make telenetic effect. idk yet
    /// </summary>
    /// <param name="grabDistance"></param>
    /// <param name="grabbableTag"></param>
    private void HighlightGrabbedObject(float grabDistance, string grabbableTag)
    {
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, grabDistance))
            {
                //if were dragging a draggable
                if (hit.transform.tag == grabbableTag)
                {
                    //highlight it with fancy shaders,
                    //disable our movement? slow us down?
                    //show animation of hand
                    //controllerAnimation.Set
                    StopMoving = true;
                }
            }
        }
    }

    /// <summary>
    /// Disables movement speed/rotation if stopped
    /// Updates body model rotation to match camera
    /// </summary>
    private void UpdateMovementStatus()
    {
        if (stopMoving)
        {
            controllerWalker.movementSpeed = 0;
            return;
        }

        //Turn our model to camera
        bodyTransform.rotation = cameraTransform.rotation;
        bodyTransform.eulerAngles = new Vector3(0, bodyTransform.eulerAngles.y, bodyTransform.eulerAngles.z);
    }

    /// <summary>
    /// SHIFT + Movement makes speed go from 2 to 5, update animation threshold if values change!
    /// </summary>
    /// <param name="config"></param>
    private void RunningControls(Player_Config config)
    {
        //Running speed adjustment
        if (Input.GetKey(KeyCode.LeftShift) && config.canRun)
        {
            controllerWalker.movementSpeed = config.runSpeed;
            config.runStamina -= 8f * Time.deltaTime;
        }
        else
        {
            if (config.runStamina < config.staminaLevel)
                config.runStamina += 8f * Time.deltaTime;
            controllerWalker.movementSpeed = config.walkSpeed;
        }

        //Stamina
        //exhausted? stop running
        if (config.runStamina <= 0)
        {
            config.staminaExhausted = true;
            config.canRun = false;
        }

        //if not exhausted, we can run
        if (config.runStamina > 0 && !config.staminaExhausted)
        {
            config.canRun = true;
        }
        //if exhausted, we cant run
        else if (config.runStamina > 0 && config.staminaExhausted)
        {
            if (config.runStamina > config.staminaLevelForRecover)
                config.staminaExhausted = false;
        }
    }

    /// <summary>
    /// Update HUD every second
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateHUD()
    {
        yield return new WaitForSeconds(1);

        hud.SetNameText(photonView.Owner.NickName);
        hud.SetStaminaText( "Stamina " + config.runStamina.ToString("F1"));
        
        if (config.isImposter)
            hud.SetImposterText("I am Imposter");
        else
            hud.SetImposterText("I am Crewmate");

        StartCoroutine("UpdateHUD");
    }
}