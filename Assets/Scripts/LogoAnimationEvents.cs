using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoAnimationEvents : MonoBehaviour {

    private AudioClip m_hitSound;
    public AudioSource m_soundsource;
	// Use this for initialization
	void Start ()
    {
        m_hitSound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_LOGO_HIT);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Logo_Hit_Event()
    {
        m_soundsource.PlayOneShot(m_hitSound);
    }
}
