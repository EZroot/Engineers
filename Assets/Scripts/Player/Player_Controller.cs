using CMF;
using Photon.Pun;
using System.CodeDom;
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
    public LayerMask grabLayer;
    
    public float throwForce = 5f;

    //components
    private AdvancedWalkerController controllerWalker;
    private Player_AnimationController controllerAnimation;
    private Player_RagdollController controllerRagdoll;
    private Player_FightingController controllerFighting;
    private Player_Config config;
    private Player_Hud hud;

    private LineRenderer lineRenderGrab;
    private GameObject hitObject = null;

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

        lineRenderGrab = gameObject.GetComponent<LineRenderer>();
        lineRenderGrab.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
        lineRenderGrab.startColor = Color.white;
        lineRenderGrab.endColor = Color.white;
        lineRenderGrab.startWidth = 0.002f;
        lineRenderGrab.endWidth = 0.002f;

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
        
        HighlightGrabbedObject(4f, grabLayer);
        HighlightInteractables();
        MinimapControls();

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

    private void MinimapControls()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            hud.MinimapToggle();
            Debug.Log("minimap toggled");
        }
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

            if (hit.transform.tag == "Plugin")
            {
                hud.SetIdentityText("Plugin");
            }

            if (hit.transform.tag == "Lantern")
            {
                hud.SetIdentityText("Lantern");
            }

            if (hit.transform.tag=="Untagged")
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
    private void HighlightGrabbedObject(float grabDistance, LayerMask grabLayer)
    {
        Vector3 startGrabPos = Vector3.zero;

        Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, grabDistance, grabLayer))
        {
            if (Input.GetMouseButtonDown(0))
            {
                hitObject = hit.transform.gameObject;
                config.canAttack = false;
            }
        }

        if (Input.GetMouseButton(0) && hitObject != null)
        {
            startGrabPos = ray.origin + cameraTransform.forward;
            lineRenderGrab.SetPositions(new Vector3[] { startGrabPos, hitObject.transform.position });
        }

        if (Input.GetMouseButtonUp(0))
        {
            lineRenderGrab.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });

            startGrabPos = Vector3.zero;
            hitObject = null;
            config.canAttack = true;
        }

        //Throwing
        if (Input.GetMouseButtonDown(1))
        {
            if (hitObject == null)
                return;
            Rigidbody_Drag rbDrag = hitObject.GetComponent<Rigidbody_Drag>();
            if (rbDrag == null)
                return;

            rbDrag.StopDragging();
            rbDrag.thisRb.AddForceAtPosition((hitObject.transform.position - transform.position) * (throwForce), hitObject.transform.position, ForceMode.Impulse);

            lineRenderGrab.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });

            startGrabPos = Vector3.zero;
            hitObject = null;
            config.canAttack = true;
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