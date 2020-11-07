using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Task_Plugin : MonoBehaviourPun, IPlugin
{
    private Rigidbody rb;
    private Task_PluginSlot pluginSlot;
    private PhotonView pluginPhotonView;

    private bool isSlotted = false;
    public bool IsSlotted { get { return isSlotted; } set { isSlotted = value; } }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSlotted)
        {
            if (other.tag == "PluginSlot")
            {
                pluginSlot = other.gameObject.GetComponent<Task_PluginSlot>();
                pluginPhotonView = other.gameObject.GetComponent<PhotonView>();
                pluginPhotonView.RPC("UpdateSlot", RpcTarget.AllBufferedViaServer, true);
                isSlotted = true;
                rb.isKinematic = true;
                transform.position = pluginSlot.transform.position;
                transform.rotation = pluginSlot.transform.rotation;
                //transform.parent = slot.transform;
            }
        }
    }

    public void UnSlot()
    {
        rb.isKinematic = false;
        StartCoroutine(UnSlotEnumerator(1f));
    }

    IEnumerator UnSlotEnumerator(float timer)
    {
        yield return new WaitForSeconds(timer);
        photonView.RPC("IsSlottedRPC", RpcTarget.AllBufferedViaServer, false);
        pluginPhotonView.RPC("UpdateSlot", RpcTarget.AllBufferedViaServer, false);
        pluginPhotonView = null;
    }

    [PunRPC]
    void IsSlottedRPC(bool isslotted)
    {
        isSlotted = isslotted;
    }
}