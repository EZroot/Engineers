using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class CrewManager : MonoBehaviourPun
{
    private static CrewManager _instance;
    public static CrewManager Instance { get { return _instance; } }

    public List<PhotonView> playerPhotonViews;
    public int totalCrewDeaths = 0;
    public Transform spawner;

    private int readyCount;
    bool showGameOverScreen = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        playerPhotonViews = new List<PhotonView>();
        StartCoroutine(UpdatePlayerList(1f));
        readyCount = 0;
    }

    IEnumerator UpdatePlayerList(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerPhotonViews = new List<PhotonView>();
        //Populate photonview list
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject o in players)
        {
            playerPhotonViews.Add(o.GetComponent<PhotonView>());
        }
        //if imposter is one, playerlist - 1 = imposter win cause he killed every1
        int playerDeathCount = 0;
        foreach(PhotonView pv in playerPhotonViews)
        {
            if(IsDead(pv.ViewID))
            {
                playerDeathCount++;
            }
        }
        //How many deaths
        totalCrewDeaths = playerDeathCount;
        //Debug.Log("Crew Deaths: " + totalCrewDeaths + " Out Of: " + playerPhotonViews.Count);

        //GAME OVER (if 1 person is still alive
        readyCount = 0;
        if(totalCrewDeaths==playerPhotonViews.Count-1 && playerPhotonViews.Count>1)
        {
            //check if everyone readied up
            foreach(PhotonView pv in playerPhotonViews)
            {
                if (pv.GetComponent<Player_Ready>().isReady)
                    readyCount++;
                
                pv.RPC("SetPlayersReadyHudText", RpcTarget.AllBufferedViaServer, readyCount, pv.ViewID);

                if (!showGameOverScreen)
                {
                    pv.RPC("OpenVictoryScreen", RpcTarget.AllBufferedViaServer, pv.ViewID, true);
                }
            }
           showGameOverScreen=true;
            //everyones readied up
            if (readyCount == playerPhotonViews.Count)
            {
                //Reload Level
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("ReloadScene", RpcTarget.AllBufferedViaServer);
                    //PhotonNetwork.LoadLevel(1);
                    /*foreach(PhotonView pv in playerPhotonViews)
                    {
                        pv.RPC("SetHealth", RpcTarget.AllBufferedViaServer, pv.ViewID, 100);
                        pv.RPC("RagdollToggle", RpcTarget.AllBufferedViaServer, pv.ViewID, false);
                    }*/
                    yield break; //this might not break the coroutine, which might be why it loads mape 3/4 times sometimes
                }
            }
        }
        //restart loop
        StartCoroutine(UpdatePlayerList(delay));
    }
    //Getters
    public PhotonView GetPlayerPhotonView(int viewId)
    {
        return playerPhotonViews.FirstOrDefault(x => x.ViewID == viewId);
    }

    public Player_Health GetPlayerHealth(int viewId)
    {
        PhotonView pv = playerPhotonViews.FirstOrDefault(x => x.ViewID == viewId);
        if (pv!=null)
        {
            return pv.gameObject.GetComponent<Player_Health>();
        }
        return null;
    }

    public int GetPlayerCount(int viewId)
    {
        return playerPhotonViews.FindIndex(pv => pv.ViewID == viewId);
    }

    public bool IsDead(int viewId)
    {
        float health = GetPlayerPhotonView(viewId).gameObject.GetComponent<Player_Health>().hitpoints;
        if (health <= 0)
            return true;
        return false;
    }

    public void AddPlayer(PhotonView photonView)
    {
        PhotonView pv = GetPlayerPhotonView(photonView.ViewID);
        if (pv==null)
        {
            playerPhotonViews.Add(photonView);
        }
    }

    public void RemovePlayer(PhotonView photonView)
    {
        PhotonView pv = GetPlayerPhotonView(photonView.ViewID);
        if (pv != null)
            playerPhotonViews.Remove(pv);
    }

    public Vector3 GetSpawnerPosition()
    {
        return spawner.position;
    }

    [PunRPC]
    void ReloadScene()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
