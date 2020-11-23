using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_InteractionController : MonoBehaviourPun
{
    public float interactionDistance = 2f;
    public string buttonTag = "Button";
    public string airFilterTag = "AirFilter";
    public string generatorTag = "Generator";
    public string waterTankTag = "WaterTank";
    private IInteractive interactive = null;
    private PhotonView otherPV;

    private float progressBarCount = 0f;
    private float progressBarCompleted = 5f;

    Player_Hud hud;
    Player_Config config;

    private void Start()
    {
        hud = GetComponent<Player_Hud>();
        config = GetComponent<Player_Config>();
    }

    // Update is called once per frame
    void Update()
    {
        //Locking/unlocking door keypads
        //Pressing generator/any button
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                //press a button
                if (hit.collider.transform.tag == buttonTag)
                {
                    interactive = hit.collider.transform.GetComponent<IInteractive>();
                    otherPV = hit.collider.transform.GetComponent<PhotonView>();
                    if(interactive!=null)
                    {
                        otherPV.RPC("PressedButton", RpcTarget.AllBufferedViaServer, !interactive.IsOn);
                    }
                }
                otherPV = null;
                interactive = null;
            }
        }

        //Tasks
        if (Input.GetMouseButton(0))
        {

            Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                //Cleaning air filter
                if (hit.transform.tag == airFilterTag)
                {
                    config.audioSource.clip = config.cleaningClip;

                    //clean filter?
                    if (hit.transform.GetComponent<Task_AirFiltration>().cleanFilter)
                    {

                        //Audio stop
                        if (config.audioSource.isPlaying)
                            config.audioSource.Stop();
                        //Turn off UI
                        hud.SetProgressBarOff();
                        hud.SetProgressBarText("");
                        return;
                    }

                    progressBarCount += 1f * Time.deltaTime;
                    //hud updat0
                    hud.SetProgressBarText("Cleaning Filter ("+progressBarCompleted+"s) "+progressBarCount.ToString("F0")+"s");
                    //Turn on UI
                    if (!hud.progressBarBackground.IsActive())
                        hud.SetProgressBarOn();
                    hud.SetProgressBarFillAmount((progressBarCount/5));

                    if (progressBarCount < progressBarCompleted)
                    {
                        //Audio play
                        if (!config.audioSource.isPlaying)
                            config.audioSource.Play();
                        return;
                    }


                    //Audio stop
                    if (config.audioSource.isPlaying)
                        config.audioSource.Stop();
                    //Turn off UI
                    hud.SetProgressBarText("");
                    hud.SetProgressBarOff();
                    PhotonView otherPV = hit.transform.gameObject.GetComponent<PhotonView>();
                    otherPV.RPC("CleanFilter", RpcTarget.AllBufferedViaServer);

                    progressBarCount = 0f;
                }

                //Clear water tank
                if (hit.transform.tag == waterTankTag)
                {
                    config.audioSource.clip = config.valveSpinClip;
                    Task_WaterTank tank = hit.transform.GetComponent<Task_WaterTank>();
                    //clean filter?
                    if (tank.clearWater)
                    {
                        //Audio stop
                        if (config.audioSource.isPlaying)
                            config.audioSource.Stop();
                        //Turn off UI
                        hud.SetProgressBarOff();
                        hud.SetProgressBarText("");
                        return;
                    }

                    tank.SpinValve(50f);
                    progressBarCount += 1f * Time.deltaTime;

                    //hud update
                    hud.SetProgressBarText("Spinning Valve ("+ progressBarCompleted+"s) "+progressBarCount.ToString("F0")+"s");
                    //Turn on UI
                    if (!hud.progressBarBackground.IsActive())
                        hud.SetProgressBarOn();
                    hud.SetProgressBarFillAmount((progressBarCount / 5));

                    if (progressBarCount < progressBarCompleted)
                    {
                        //Audio play
                        if (!config.audioSource.isPlaying)
                            config.audioSource.Play();
                        return;
                    }


                    //Audio stop
                    if (config.audioSource.isPlaying)
                        config.audioSource.Stop();
                    //Turn off UI
                    hud.SetProgressBarText("");
                    hud.SetProgressBarOff();
                    PhotonView otherPV = hit.transform.gameObject.GetComponent<PhotonView>();
                    otherPV.RPC("ClearedWater", RpcTarget.AllBufferedViaServer);

                    progressBarCount = 0f;
                }
                progressBarCount = 0f;
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            //Audio stop
            if (config.audioSource.isPlaying)
                config.audioSource.Stop();
            //Turn off UI
            hud.SetProgressBarText("");
            hud.SetProgressBarOff();
        }
    }
}
