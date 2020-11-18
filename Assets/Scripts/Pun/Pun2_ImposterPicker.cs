using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pun2_ImposterPicker : MonoBehaviourPun
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(PickImposterEnumerator(3f, "Player"));
        }
    }
    IEnumerator PickImposterEnumerator(float delay, string playerTag)
    {
        Debug.Log("picking imposter...");
        yield return new WaitForSeconds(delay);
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        PhotonView[] pvs = new PhotonView[players.Length];
        for(int i =0; i< players.Length;i++)
        {
            pvs[i] = players[i].GetComponent<PhotonView>();
        }

        int pick = Random.Range(0, players.Length);

        int pickId = pvs[pick].ViewID;

        pvs[pick].RPC("PickImposterRPC", RpcTarget.AllBufferedViaServer, pickId, true);

        Debug.Log("Imposter should be picked");
    }
}
