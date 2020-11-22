using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public interface IItem
{
    string GetName();
    string GetInfo();
    void OnPickup();
    void OnDrop();
    PhotonView GetPhotonView();
    Item_Pickupable.ItemType GetItemType();
    Rigidbody GetRigidbody();
    void SetLocation(Vector3 pos);
}
