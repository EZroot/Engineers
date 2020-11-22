using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item_Pickupable : MonoBehaviourPun, IItem
{
    Rigidbody rb;

    public enum ItemType
    {
        Battery,
        Keycard,
        Knife,
        Pistol
    }

    public string itemName = "Default";
    public string info = "Set this text";

    public ItemType itemType = ItemType.Battery;
    public int amount;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public string GetInfo()
    {
        return info;
    }

    public string GetName()
    {
        return itemName;
    }

    public ItemType GetItemType()
    {
        return itemType;
    }

    [PunRPC]
    public void OnPickup()
    {
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void OnDrop()
    {
        gameObject.SetActive(true);
    }

    public void SetLocation(Vector3 pos)
    {
        transform.position=(pos);
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    public PhotonView GetPhotonView()
    {
        return photonView;
    }
}
