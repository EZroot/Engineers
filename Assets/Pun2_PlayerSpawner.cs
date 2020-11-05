using UnityEngine;
using Photon.Pun;

public class Pun2_PlayerSpawner : MonoBehaviourPunCallbacks
{
    //Player instance prefab, must be located in the Resources folder
    public GameObject playerPrefab;
    //Player spawn point
    public Transform spawnPoint;

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        //In case we started this demo with the wrong scene being active, simply load the menu scene
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Is not in the room, returning back to Lobby");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby");
            return;
        }

        //We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(spawnPoint.position.x + PhotonNetwork.PlayerList.Length*2, spawnPoint.position.y,spawnPoint.position.z), Quaternion.identity, 0);
    }

    void OnGUI()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;

        //Leave this Room
        if (GUI.Button(new Rect(5, 5, 125, 25), "Leave Room"))
        {
            PhotonNetwork.LeaveRoom();
        }

        //Show the Room name
        GUI.Label(new Rect(135, 5, 200, 25), PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnLeftRoom()
    {
        //We have left the Room, return back to the GameLobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby");
    }
}