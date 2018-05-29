using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {
    private AudioSource source;
    public AudioClip step;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        step = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_PLAYER_STEP);
	}

    public void StepSound()
    {
        //Debug.Log("Step");
        //source.PlayOneShot(step);
    }
}
