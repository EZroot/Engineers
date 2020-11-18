using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    private static TaskManager _instance;
    public static TaskManager Instance { get { return _instance; } }

    public GameObject[] taskObjects;
    private ITask[] tasks;

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
        for (int i =0; i < taskObjects.Length;i++)
        {
            tasks[i] = taskObjects[i].GetComponent<ITask>();
        }

        StartCoroutine(UpdateTasks(1f));
    }

    IEnumerator UpdateTasks(float delay)
    {
        yield return new WaitForSeconds(delay);
        //if task is broken
        //or if task needs to be done by the player role thats selected
        foreach (ITask t in tasks)
        {
            if (t.IsBroken())
            {
                t.OutlineTaskOn();
            }
            else
            {
                t.OutlineTaskOff();
            }
        }
        StartCoroutine(UpdateTasks(delay));
    }

    public ITask GetGeneratorTask()
    {
        return tasks[0];
    }

    public ITask GetAirFilter()
    {
        return tasks[1];
    }
}
