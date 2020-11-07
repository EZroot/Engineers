using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Task_Plugin : MonoBehaviourPun
{
    public bool isSlotted = false;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    [PunRPC]
    void IsSlottedRPC(bool isslotted)
    {
        isSlotted = isslotted;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSlotted)
        {
            if (other.tag == "PluginSlot")
            {
                Task_PluginSlot slot = other.gameObject.GetComponent<Task_PluginSlot>();
                isSlotted = true;
                rb.isKinematic = true;
                transform.position = slot.transform.position;
                transform.rotation = slot.transform.rotation;
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
    }
}