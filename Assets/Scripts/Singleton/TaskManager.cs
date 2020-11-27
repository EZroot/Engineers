using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TaskManager : MonoBehaviour
{
    private static TaskManager _instance;
    public static TaskManager Instance { get { return _instance; } }

    public GameObject[] taskObjects;
    private ITask[] tasks;
    private PhotonView[] tasksPhotonViews;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        tasks = new ITask[taskObjects.Length];
        tasksPhotonViews = new PhotonView[taskObjects.Length];

        for (int i =0; i < taskObjects.Length;i++)
        {
            tasks[i] = taskObjects[i].GetComponent<ITask>();
            tasksPhotonViews[i] = taskObjects[i].GetComponent<PhotonView>();
        }

       // StartCoroutine(UpdateTasks(1f));
    }

    IEnumerator UpdateTasks(float delay)
    {
        yield return new WaitForSeconds(delay);
        //if task is broken
        //or if task needs to be done by the player role thats selected
        /*foreach (ITask t in tasks)
        {
            if (t.IsBroken())
            {
                t.OutlineTaskOn();
            }
            else
            {
                t.OutlineTaskOff();
            }
        }*/
        StartCoroutine(UpdateTasks(delay));
    }

    public ITask GetGeneratorTask()
    {
        return tasks[0];
    }

    public PhotonView GetGeneratorPhotonView()
    {
        return tasksPhotonViews[0];
    }

    public ITask GetAirFilter()
    {
        return tasks[1];
    }

    public PhotonView GetAirFilterPhotonView()
    {
        return tasksPhotonViews[1];
    }

    public ITask GetWaterTank()
    {
        return tasks[2];
    }

    public PhotonView GetWaterTankPhotonView()
    {
        return tasksPhotonViews[2];
    }
}
