using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownAnimEvents : MonoBehaviour
{
    private AudioSource source;
    private AudioClip clip;

	// Use this for initialization
	void Start ()
    {
        source = GetComponent<AudioSource>();
        clip = Resources.Load<AudioClip>("Sound/snd_Countdown");
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void CountdownSound()
    {
        source.volume += 0.025f;
        source.pitch += 0.025f;
        source.PlayOneShot(clip);
    }
}
