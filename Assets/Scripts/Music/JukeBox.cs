using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class JukeBox : MonoBehaviour 
{
    private AudioClip[] Track1;
    private AudioClip Menu1;
    private AudioClip Game1;
    private AudioClip scoreScreen;
    private AudioClip[] Stings;
    public AudioSource[] Source;
    private AudioSource stingSource;
    public float bpm = 170;

    public AudioMixerSnapshot outOfCombat;
    public AudioMixerSnapshot inCombat;
    public bool menu = false;

    private float m_TransitionIn;
    private float m_TransitionOut;
    private float m_QuarterNote;

    void Awake()
    {
        //Track1 = Resources.LoadAll(GLOBAL_VALUES.MUSIC_TRACK_1) as AudioClip[];
        Menu1 = Resources.Load("Sound/snd_AttractLoopYD") as AudioClip;
        Game1 = Resources.Load("Sound/snd_LevelLoopMW") as AudioClip;
        scoreScreen = Resources.Load("Sound/snd_ScoreScreen") as AudioClip;
        //Menu1 = Resources.LoadAll(GLOBAL_VALUES.MUSIC_MENU_1) as AudioClip[];
        stingSource = GetComponentInChildren<AudioSource>();
        Source = GetComponents<AudioSource>();     
    }

	// Use this for initialization
	void Start () 
    {
        if (!menu)
        {
            Source[0].clip = Game1;
            //Source[1].clip = Track1[1];
        }
        else
        {
            Source[0].clip = Menu1;
            //Source[1].clip = Menu1[1];
        }

        //calc quarter note
        m_QuarterNote = 60 / bpm;
        m_TransitionIn = m_QuarterNote;
        m_TransitionOut = m_QuarterNote * 32;
        Source[0].volume = .25f;
        if (menu)
        {
            Source[0].PlayDelayed(1f);
        }
        else
        {
            Source[0].Play();
        }
        
    }

	public void transition()
    {
        inCombat.TransitionTo(m_TransitionIn);
        playSting();
    }

    public void returnMusic()
    {
        outOfCombat.TransitionTo(m_TransitionOut);
    }

    private void playSting()
    {
        int randClip = Random.Range(0, Stings.Length);
        stingSource.clip = Stings[randClip];
        stingSource.Play();
    }

    public void PlayScoreScreen()
    {
        Source[0].clip = scoreScreen;
        Source[0].Play();
    }
}
