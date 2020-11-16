using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_ImposterController : MonoBehaviourPun
{
    Player_Config config;
    public string[] tags = new string[2] { "Generator", "AirFilter" };
    private void Start()
    {
        config = GetComponent<Player_Config>();
    }
    
    //Change sabotages to a sabotage map or something similar instead of clicking
    private void Update()
    {
        SabotageController();
    }

    private void SabotageController()
    {
        if (config.isImposter)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 4f))
                {
                    //generator
                    foreach (string s in tags)
                    {
                        if (hit.transform.tag == s)
                        {
                            PhotonView otherPV = hit.transform.gameObject.GetComponent<PhotonView>();
                            otherPV.RPC("Sabotage", RpcTarget.AllBufferedViaServer);
                        }
                    }
                }
            }
        }
    }
}
