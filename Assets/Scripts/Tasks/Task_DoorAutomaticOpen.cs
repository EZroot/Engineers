using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_DoorAutomaticOpen : MonoBehaviour
{
    public GameObject largeDoorWing;
    public GameObject smallDoorWing;

    Vector3 pos1;
    Vector3 pos2;

    private void Start()
    {
        pos1 = largeDoorWing.transform.position;
        pos2 = smallDoorWing.transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag=="Player")
        {
            largeDoorWing.transform.position = Vector3.Lerp(pos1, new Vector3(pos1.x - 45, pos1.y, pos1.z),  Time.deltaTime);
            smallDoorWing.transform.position = Vector3.Lerp(pos2, new Vector3(pos2.x + 25, pos2.y, pos2.z), Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            largeDoorWing.transform.position = Vector3.Lerp(pos1, new Vector3(pos1.x, pos1.y, pos1.z),  Time.deltaTime);
            smallDoorWing.transform.position = Vector3.Lerp(pos2, new Vector3(pos2.x, pos2.y, pos2.z), Time.deltaTime);
        }
    }
}
