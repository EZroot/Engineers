using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pun2_RigidbodySync : MonoBehaviourPun, IPunObservable
{
    private Vector3 lastPos;
    private Quaternion lastRot;
    private Vector3 velocity;
    private Vector3 angularVelocity;

    private Rigidbody rigidBody;
    private bool initStream;
    private bool wasHost;

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rigidBody.velocity);
            stream.SendNext(rigidBody.angularVelocity);
            initStream = false;
            wasHost = true;
        }
        else
        {
            lastPos = (Vector3)stream.ReceiveNext();
            lastRot = (Quaternion)stream.ReceiveNext();
            velocity = (Vector3)stream.ReceiveNext();
            angularVelocity = (Vector3)stream.ReceiveNext();
            if (initStream)
            {
                transform.position = lastPos;
                transform.rotation = lastRot;
                rigidBody.velocity = velocity;
                rigidBody.angularVelocity = angularVelocity;
                initStream = false;
            }
            wasHost = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        //gameObject.tag = "NetworkProp";
        initStream = true;
        wasHost = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!wasHost)
        {
            if (!initStream)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, lastPos, Time.deltaTime * 10.0f);
                Quaternion newRotation = Quaternion.Lerp(transform.rotation, lastRot, Time.deltaTime * 10.0f);
                transform.position = newPosition;
                transform.rotation = newRotation;
                rigidBody.velocity = velocity;
                rigidBody.angularVelocity = angularVelocity;
            }
            return;
        }
    }

    void OnCollisionEnter(Collision contact)
    {
        Transform collisionObjectRoot = contact.transform.root;
        if (photonView == null)
        {
            Debug.Log("No photon view on: " + contact.gameObject.name);
            return;
        }

        if (!photonView.IsMine)
        {
            if (collisionObjectRoot.CompareTag("Player"))
            {
                Debug.Log("COLLIDED WITH PLAYER");

                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
            return;
        }
    }
}
