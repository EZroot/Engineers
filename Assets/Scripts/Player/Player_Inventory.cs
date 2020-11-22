using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class Player_Inventory : MonoBehaviourPun
{
    private int inventorySize = 5;
    private Player_Hud hud;
    private List<IItem> itemList;

    //itemslot
    //item type
    //item count
    //item name
    int batteryCount = 0;
    Player_FightingController fightingController;

    private void Start()
    {
        itemList = new List<IItem>();
        hud = GetComponent<Player_Hud>();
        fightingController = GetComponent<Player_FightingController>();
    }

    //If theres enough space in our inventory
    //
    public void PickUp(IItem item)
    {
        itemList.Add(item);
        Item_Pickupable.ItemType itemType = item.GetItemType();
        switch(itemType)
        {
            case Item_Pickupable.ItemType.Battery:
                PhotonView pv = item.GetPhotonView();
                pv.RPC("OnPickup", RpcTarget.AllBufferedViaServer);
                break;
            case Item_Pickupable.ItemType.Knife:
                PhotonView s = item.GetPhotonView();
                s.RPC("OnPickup", RpcTarget.AllBufferedViaServer);
                fightingController.SelectWeaponModel(Player_FightingController.Weapon.Knife);
                break;
            case Item_Pickupable.ItemType.Pistol:
                PhotonView r = item.GetPhotonView();
                r.RPC("OnPickup", RpcTarget.AllBufferedViaServer);
                fightingController.SelectWeaponModel(Player_FightingController.Weapon.Pistol);
                break;
        }
    }

    //need to show inventory ui first, hover over item want to drop,
    //drop it
    public void Drop()
    {
        //select item to drop
        IItem battery = GetItem(Item_Pickupable.ItemType.Battery);
        if (battery == null) { 
            return;
        }
            battery.GetPhotonView().RPC("OnDrop", RpcTarget.AllBufferedViaServer);
        battery.SetLocation(Vector3.up*1.6f + transform.position+transform.GetChild(0).GetChild(0).forward);
            batteryCount -= 1;

        RemoveItem(battery);
    }


    //Getters
    private IItem GetItem(Item_Pickupable.ItemType itemType)
    {
        //search inventory for itemtype
        return itemList.FirstOrDefault(x => x.GetItemType() == itemType);
    }

    private void RemoveItem(IItem item)
    {
        if (item != null)
            itemList.Remove(item);
        Debug.Log(itemList.Count);
    }

    private void RemoveItem(Item_Pickupable.ItemType itemType)
    {
        IItem item = GetItem(itemType);
        if(item!=null)
            itemList.Remove(item);
        Debug.Log(itemList.Count);
    }
}
