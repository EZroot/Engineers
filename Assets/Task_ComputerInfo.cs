using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Task_ComputerInfo : MonoBehaviour
{
    public GameObject taskObject;
    public TextMeshProUGUI infoTextMesh;

    //Used to grab task info from gameobject
    private ITask taskInfo;

    private void Start()
    {
        taskInfo = taskObject.GetComponent<ITask>();
        StartCoroutine(UpdateText(1f));
        Debug.Log(taskObject.name);
    }

    IEnumerator UpdateText(float delay)
    {
        yield return new WaitForSeconds(delay);
        infoTextMesh.text = taskInfo.GetInfo();
        Debug.Log("Updaitng console text for " + taskObject.name);

        StartCoroutine(UpdateText(delay)); 
    }
}
