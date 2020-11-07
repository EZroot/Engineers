using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_InteractionController : MonoBehaviour
{
    public float interactionDistance = 4f;
    public string buttonTag = "Button";

    private IInteractive interactive = null;
    private PhotonView otherPV;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                //if were dragging a draggable
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
    }
}
