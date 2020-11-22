using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Player_InventoryController : MonoBehaviourPun
{
    private Player_Config config;
    private Player_Hud hud;
    private Player_Inventory inventory;
    private IItem itemPickup = null;

    //make item pool
    //when pickup batterys, disable them over pun
    //when drop, enable and teleport them in front of you
    //the pool should only have as many items as there are in a scene

    private void Start()
    {
        config = GetComponent<Player_Config>();
        hud = GetComponent<Player_Hud>();
        inventory = GetComponent<Player_Inventory>();
    }

    private void Update()
    {
        PickUpControls();
        DropControls();
    }

    //Contains the photonview controls to disable the item after pickup
    private void PickUpControls()
    {
        RaycastHit hit;
        Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 5f))
        {
            //Physics objs
            HighlightInteractables(hit);
            //Pickup items
            itemPickup = HighlightItems(hit);
            if (itemPickup != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    inventory.PickUp(itemPickup);
                }
            }
        }
        else
        {
            hud.SetIdentityText("");
        }
    }

    private void DropControls()
    {
        //For Dropping
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //it needs to select an item from the UI of the inventory first
            inventory.Drop();
        }
    }


    /// <summary>
    /// Highlights the item that we can pickup, and returns it as well
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    private IItem HighlightItems(RaycastHit hit)
    {
        IItem item = null;
        if(hit.transform.tag=="PickupableItem")
        {
            item = hit.transform.gameObject.GetComponent<IItem>();
            hud.SetIdentityText(item.GetName()+"\n"+item.GetInfo()+"\nPress [E] to pickup");
        }
        return item;
    }

    /// <summary>
    /// Show the clients name that youre looking at
    /// </summary>
    private void HighlightInteractables(RaycastHit hit)
    {
        if (hit.transform.tag == "Player")
        {
            hud.SetIdentityText(hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName);
        }

        if (hit.transform.tag == "Door")
        {
            hud.SetIdentityText("[LMB] Drag Door ");
        }

        if (hit.transform.tag == "Button")
        {
            Task_DoorNumpad numpad = hit.transform.gameObject.GetComponent<Task_DoorNumpad>();
            if (numpad != null)
            {
                if (numpad.unlocked)
                    hud.SetIdentityText("[LMB] Unlock");
                else
                    hud.SetIdentityText("[LMB] Lock");
            }
            else
            {
                hud.SetIdentityText("[LMB] Press " +"Button");
            }
        }

        if (hit.transform.tag == "Plugin")
        {
            hud.SetIdentityText("[LMB] Grab "+"Plugin");
        }

        if (hit.transform.tag == "Lantern")
        {
            hud.SetIdentityText("[LMB] Grab " + "Lantern");
        }

        if(hit.transform.tag=="Prop")
        {
            hud.SetIdentityText("[LMB] Grab");
        }

        if (hit.transform.tag == "Untagged")
        {
            hud.SetIdentityText("");
        }
    }
}
