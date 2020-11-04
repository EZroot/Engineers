using CMF;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    //to rotate body model
    public Transform cameraTransform;
    public Transform bodyTransform;

    //HUD
    public TextMeshProUGUI HUDStamina;

    private AdvancedWalkerController controllerWalker;
    private Player_AnimationController controllerAnimation;
    private Player_Config config;

    private bool stopMoving = false;
    public bool StopMoving { get { return stopMoving; } set { stopMoving = value; } }

    private void Start()
    {
        //movement controller
        controllerWalker = GetComponent<AdvancedWalkerController>();
        controllerAnimation = GetComponent<Player_AnimationController>();

        config = GetComponent<Player_Config>();

        //stamina
        config.runStamina = config.staminaLevel;
        StartCoroutine("UpdateHUD");
    }

    private void Update()
    {
        ImposterControls(4f, "Player");
        UpdateBodyModelRotation();
        RunningControls(config);

        //Debug
        if (Input.GetMouseButtonDown(0))
        {
            stopMoving = !stopMoving;
        }

        //if we need to stop moving, stop moving
        //should i return? maybe not incase of future gameplay? idk
        UpdateMovementStatus();
    }

    private void HighlightGrabbedObject(float grabDistance, string grabbableTag)
    {
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, grabDistance))
            {
                //if were dragging a draggable
                if (hit.transform.tag == grabbableTag)
                {
                    //highlight it with fancy shaders,
                    //disable our movement? slow us down?
                    //show animation of hand
                    StopMoving = true;
                }
            }
        }
    }


    private void ImposterControls(float killDistance, string playerTag)
    {
        //imposters only
        if(config.isImposter)
        {
            //left mouse click
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, killDistance))
                {
                    //if we hit another player
                    if(hit.transform.tag==playerTag)
                    {
                        //kill the player
                        hit.transform.gameObject.GetComponent<Player_Respawner>().KillImmediataly(transform.position, 5f);
                    }
                }
            }
        }
    }

    private void UpdateBodyModelRotation()
    {
        //Turn our model to camera
        bodyTransform.rotation = cameraTransform.rotation;
        bodyTransform.eulerAngles = new Vector3(0, bodyTransform.eulerAngles.y, bodyTransform.eulerAngles.z);
    }

    private void UpdateMovementStatus()
    {
        if (stopMoving)
            controllerWalker.movementSpeed = 0;
        else
            controllerWalker.movementSpeed = config.walkSpeed;
    }

    private void RunningControls(Player_Config config)
    {
        //Running speed adjustment
        if (Input.GetKey(KeyCode.LeftShift) && config.canRun)
        {
            controllerWalker.movementSpeed = config.runSpeed;
            config.runStamina -= 15f * Time.deltaTime;
        }
        else
        {
            if (config.runStamina < config.staminaLevel)
                config.runStamina += 5f * Time.deltaTime;
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

    //No need to update HUD GUI 50 times a frame
    IEnumerator UpdateHUD()
    {
        yield return new WaitForSeconds(1);
        HUDStamina.text = "STAMINA " + config.runStamina.ToString("F1");
        StartCoroutine("UpdateHUD");
    }
}
