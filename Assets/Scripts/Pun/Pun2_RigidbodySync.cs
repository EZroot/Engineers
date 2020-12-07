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

    private Rigidbody_Drag rbDrag;

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
        rbDrag = GetComponent<Rigidbody_Drag>();

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
            //gets rid of jitter caused by unneeded transfer ownership
            if (rbDrag != null)
            { 
                if (rbDrag.isGrabbed)
                    return;
            }

            if (collisionObjectRoot.CompareTag("Player"))
            {
                if(rbDrag != null)
                {
                    if (rbDrag.isGrabbed)
                        return;
                }
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }
        else
        {
            //If we own this obj, and its grabbed, transfer ownership to others
            //seems to only work partially? or after 1 collision?

            //we cant get the root obj because a lot of objs are organized in heirarchy

            //Other Solutions: make a local isGrabbed so we can tell if our client is grabbing it and then replace rb.isgrabbed with that, or combine them

            //Future Problem: cant move objs being collided by the collided obj of the obj grab collision lmao
            //Grab latern -> move crate -> which moves barrel, barrel may be frozen in place/jitter cause its not transfered
            //Future Solutions: get list of oncollisionstay colliders, if grabbed transfer em all unless being collided with by player?

            if (((1 << contact.gameObject.layer) & LayerMask.NameToLayer("Interactive")) == 0)
            {
                Debug.Log("Hitting obj "+contact.gameObject.name + " with "+gameObject.name);
                if (rbDrag != null)
                {
                    if (rbDrag.isGrabbed)
                    {
                        PhotonView otherpv = contact.gameObject.GetComponent<PhotonView>();
                        if (otherpv == null)
                        {
                            Debug.Log("OTHER PHOTONVIEW DOESNT EXIST!!!!");
                            return;
                        }
                        otherpv.TransferOwnership(PhotonNetwork.LocalPlayer);
                        Debug.Log(otherpv.ViewID + " Obj transfered to " + photonView.ViewID);
                    }
                    else
                    {
                        Debug.Log("Obj "+ gameObject.name+" not grabbed.");
                    }
                }
                else
                {
                    Debug.Log("rbDrag not found!");
                }
            }
        }
    }
}
