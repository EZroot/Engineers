using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_MusicPlayer : MonoBehaviour
{
    public Task_Button button;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();    
    }

    // Update is called once per frame
    void Update()
    {
        if(button.IsOn)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}
