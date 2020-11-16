using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_InteractionController : MonoBehaviourPun
{
    public float interactionDistance = 4f;
    public string buttonTag = "Button";
    public string airFilterTag = "AirFilter";
    public string generatorTag = "Generator";

    private IInteractive interactive = null;
    private PhotonView otherPV;

    private float progressBarCount = 0f;
    private float progressBarCompleted = 20f;

    Player_Hud hud;

    private void Start()
    {
        hud = GetComponent<Player_Hud>();
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
                if (hit.transform.tag == buttonTag)
                {
                    interactive = hit.transform.GetComponent<IInteractive>();
                    otherPV = hit.transform.GetComponent<PhotonView>();
                    if(interactive!=null)
                    {
                        otherPV.RPC("PressedButton", RpcTarget.AllBufferedViaServer, !interactive.IsOn);
                    }
                }
                otherPV = null;
                interactive = null;
            }
        }

        //Cleaning air filter
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 4f))
            {
                if (hit.transform.tag == airFilterTag)
                {
                    //clean filter?
                    if (!hit.transform.GetComponent<Task_AirFiltration>().dirtyFilter)
                    {
                        hud.SetProgressBarText("");
                        return;
                    }

                    progressBarCount += 5f * Time.deltaTime;
                    //hud update
                    hud.SetProgressBarText("Cleaning (20): "+progressBarCount);
                    
                    if (progressBarCount < progressBarCompleted)
                        return;

                    PhotonView otherPV = hit.transform.gameObject.GetComponent<PhotonView>();
                    otherPV.RPC("CleanFilter", RpcTarget.AllBufferedViaServer);

                    progressBarCount = 0f;
                }
                hud.SetProgressBarText("");
                progressBarCount = 0f;
            }
        }
    }
}
