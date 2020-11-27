using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Pun2_PlayerSync : MonoBehaviourPun, IPunObservable
{
    //List of the scripts that should only be active for the local player (ex. PlayerController, MouseLook etc.)
    public MonoBehaviour[] removeScriptsFromOthers;
    //List of the GameObjects that should only be active for the local player (ex. Camera, AudioListener etc.)
    public GameObject[] removeObjectsFromOthers;
    //List of the Rigidbodies that should only be active for the local player (ex. Camera, AudioListener etc.)
    public LineRenderer[] removeLinerendererFromOthers;
    public Rigidbody[] rigidbodyFromOthersToSetKinematic;
    //List of GameObjects that should be removed for the user (eg. body for an fps, so we cant see clipping)
    public GameObject[] removeLocalObjects;

    public Collider[] ragdollCollidersToDisableTillDeath;

    //Values that will be synced over network
    public Transform modelTransform;
    public GameObject flashLight;

    //Config to check imposter status
    private Player_Config config;

    //syncing vars
    Vector3 latestPos;
    Quaternion latestRot;
    bool lightOn = false;
    Quaternion lightRot;

    // Use this for initialization
    void Start()
    {
        config = GetComponent<Player_Config>();

        //Local
        if (photonView.IsMine)
        {
            //Remove the body model from FPS view so we dont see clipping
            for (int i = 0; i < removeLocalObjects.Length; i++)
            {
                removeLocalObjects[i].SetActive(false);
            }
            for (int i = 0; i < ragdollCollidersToDisableTillDeath.Length; i++)
            {
                ragdollCollidersToDisableTillDeath[i].enabled = false;
            }
        }
        else //Client
        {
            //Player is Remote, deactivate the scripts and object that should only be enabled for the local player
            for (int i = 0; i < removeScriptsFromOthers.Length; i++)
            {
                removeScriptsFromOthers[i].enabled = false;
            }
            for (int i = 0; i < removeObjectsFromOthers.Length; i++)
            {
                removeObjectsFromOthers[i].SetActive(false);
            }
            for (int i = 0; i < removeLinerendererFromOthers.Length; i++)
            {
                removeLinerendererFromOthers[i].enabled = (false);
            }
            for (int i = 0; i < rigidbodyFromOthersToSetKinematic.Length; i++)
            {
                rigidbodyFromOthersToSetKinematic[i].isKinematic = true;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(modelTransform.transform.rotation);
            stream.SendNext(flashLight.activeSelf);
            stream.SendNext(flashLight.transform.rotation);
        }
        else
        {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            lightOn = (bool)stream.ReceiveNext();
            lightRot = (Quaternion)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Remote player
        if (!photonView.IsMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            modelTransform.transform.rotation = Quaternion.Lerp(modelTransform.transform.rotation, latestRot, Time.deltaTime * 5);
            flashLight.SetActive(lightOn);
            flashLight.transform.rotation = Quaternion.Lerp(flashLight.transform.rotation, lightRot, Time.deltaTime * 5);
        }
    }

    void OnDestroy()
    {
        PhotonNetwork.Destroy(photonView);
    }
}