using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Ready up 
public class Player_Ready : MonoBehaviourPun
{
    public bool isReady = false;

    public void Ready()
    {
        if (!photonView.IsMine)
            return;
        photonView.RPC("IsReady",RpcTarget.AllBufferedViaServer,photonView.ViewID, true);

    }

    [PunRPC]
    public void IsReady(int viewId, bool ready)
    {
        if (photonView.ViewID != viewId)
            return;

        isReady = ready;
    }
}