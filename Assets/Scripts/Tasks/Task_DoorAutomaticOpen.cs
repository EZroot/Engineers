using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_DoorAutomaticOpen : MonoBehaviour
{
    Animator animator;
    AudioSource audiosource;
    private void Start()
    {
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetTrigger("DoorOpen");
            audiosource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetTrigger("DoorClose");
            audiosource.Play();
        }
    }
}
