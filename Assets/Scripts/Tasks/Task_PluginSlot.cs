using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Task_PluginSlot : MonoBehaviourPun
{
    public bool isPluggedIn = false;

    [PunRPC]
    void UpdateSlot(bool ispluggedin)
    {
        isPluggedIn = ispluggedin;
    }
}
