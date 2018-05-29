using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
    private Timer delaytimer;
    private bool started; //Has Music Started Playing?
    private bool setup;
    private bool paused;
    private int songChoice;
    private AudioSource[] sources; //All audiosources
	private List<AudioSource> currentSong = new List<AudioSource>();
    private AudioSource source; //Default Audiosource, used for songs without layers

    //Level Loop Layers
    //Mitch's Layers
    //AudioSources
    private AudioSource mwBedSource;
    private AudioSource mwCrazyOrganSource;
    private AudioSource mwGuitarSource;
    private AudioSource mwMelodySource;
    private AudioSource mwOrganSource;

    //AudioClips
    private AudioClip mwBed;
    private AudioClip mwCrazyOrgan;
    private AudioClip mwGuitar;
    private AudioClip mwMelody;
    private AudioClip mwOrgan;

    //Bool Values
    public bool mwBedMute;
    public bool mwCrazyOrganMute;
    public bool mwGuitarMute;
    public bool mwMelodyMute;
    public bool mwOrganMute;

    //Yolanda's Layers
    //Will go here

    //Attract Loops
    private AudioClip attractLoopYD; //Yolanda's Loop
    private AudioClip attractLoopMW; //Mitch's Loop

    //Full Level Loops if we need them
    private AudioClip levelLoopYD; //Yolanda's Loop
    private AudioClip levelLoopMW; //Mitch's Loop

	// Use this for initialization
	void Start () {
        started = false;
        setup = false;
        paused = false;
        sources = GetComponents<AudioSource>();
        foreach (AudioSource s in sources)
        {
            s.loop = true;
        }

        source = sources[0]; //Default AudioSource

        //So the music starts when the round does
        delaytimer = new Timer();

        //Attract Loops
        attractLoopYD = Resources.Load<AudioClip>(GLOBAL_VALUES.MUSIC_ATTRACT_YD);
        attractLoopMW = Resources.Load<AudioClip>(GLOBAL_VALUES.MUSIC_ATTRACT_MW);

        //Full Level Loops
        levelLoopYD = Resources.Load<AudioClip>(GLOBAL_VALUES.MUSIC_FULL_LEVEL_LOOP_YD);
        levelLoopMW = Resources.Load<AudioClip>(GLOBAL_VALUES.MUSIC_FULL_LEVEL_LOOP_MW);

        //Level Loops
        //Mitch's Level Loop
        //AudioClips
        mwBed = Resources.Load<AudioClip>(GLOBAL_VALUES.MUSIC_MW_LEVEL_BED);
        mwCrazyOrgan = Resources.Load<AudioClip>(GLOBAL_VALUES.MUSIC_MW_LEVEL_CRAZY_ORGAN);
        mwGuitar = Resources.Load<AudioClip>(GLOBAL_VALUES.MUSIC_MW_LEVEL_GUITAR);
        mwMelody = Resources.Load<AudioClip>(GLOBAL_VALUES.MUSIC_MW_LEVEL_MELODY);
        mwOrgan = Resources.Load<AudioClip>(GLOBAL_VALUES.MUSIC_MW_LEVEL_ORGAN);
        //AudioSources
        mwBedSource = sources[1];
        mwBedSource.clip = mwBed;
        mwCrazyOrganSource = sources[2];
        mwCrazyOrganSource.clip = mwCrazyOrgan;
        mwGuitarSource = sources[3];
        mwGuitarSource.clip = mwGuitar;
        mwMelodySource = sources[4];
        mwMelodySource.clip = mwMelody;
        mwOrganSource = sources[5];
        mwCrazyOrganSource.clip = mwCrazyOrgan;
        //Bool Values
        mwBedMute = true;
        mwCrazyOrganMute = true;
        mwGuitarMute = true;
        mwMelodyMute = true;
        mwOrganMute = true;

        //Yolanda's Level Loop
        //Will go here
    }
	
	// Update is called once per frame
	void Update () {
        UpdateLayers();
        if (!started && setup)
        {
            delaytimer.Update();
            StartPlaying();
        }
	}

    public void Setup(int song, float delayvalue)
    {
        if (!setup)
        {
            songChoice = song;
            delaytimer.Add(delayvalue, true);
            setup = true;
        }
    }

    void StartPlaying() {
        if (delaytimer.isComplete())
        {
            started = true;
            switch (songChoice)
            {
                case 1: //Yolanda's Attract Loop
                    source.clip = attractLoopYD;
                    source.Play();
                    break;
                case 2: //Mitch's Attract Loop
                    source.clip = attractLoopMW;
                    source.Play();
                    break;
                case 3: //Yolanda's Full Level Loop
                    source.clip = levelLoopYD;
                    source.Play();
                    break;
                case 4: //Mitch's Full Level Loop
                    source.clip = levelLoopMW;
                    source.Play();
                    break;
                case 5: //Yolanda's Layered Level Loop
                        //No Layers Yet
                    break;
                case 6: //Mitch's Layered Level Loop
                    mwBedSource.Play();
                    mwCrazyOrganSource.Play();
                    mwGuitarSource.Play();
                    mwMelodySource.Play();
                    mwOrganSource.Play();
                    break;
                default:
                    break;
            }
        }
    }

    void UpdateLayers()
    {
        //Mitch's Level Loop
        if (!mwBedMute)
        {
            mwBedSource.mute = true;
        } else mwBedSource.mute = false;
        if (!mwCrazyOrganMute)
        {
            mwCrazyOrganSource.mute = true;
        } else mwCrazyOrganSource.mute = false;
        if (!mwGuitarMute)
        {
            mwGuitarSource.mute = true;
        } else mwGuitarSource.mute = false;
        if (!mwMelodyMute)
        {
            mwMelodySource.mute = true;
        } else mwMelodySource.mute = false;
        if (!mwOrganMute)
        {
            mwOrganSource.mute = true;
        } else mwOrganSource.mute = false;

        //Yolanda's Level Loop
        //Will go here
    }

    public void Pause()
    {
        if (!paused)
        {
			GetSources ();
            paused = true;
			foreach (AudioSource s in currentSong) 
			{
				if (s.isPlaying) 
				{
					s.Pause ();
				}
			}
        }
    }

    public void Resume()
    {
        if (paused)
        {
			GetSources ();
            paused = false;
			foreach (AudioSource s in currentSong) 
			{
				if (!s.isPlaying) 
				{
					s.UnPause ();
				}
			}
        }
    }

	void GetSources() 
	{
		currentSong.Clear ();
		switch (songChoice) 
		{
			case 1:
				currentSong.Add (source);
				break;
			case 2:
				currentSong.Add (source);
				break;
			case 3:
				currentSong.Add (source);
				break;
			case 4:
				currentSong.Add (source);
				break;
			case 5:
				break;
			case 6:
				currentSong.Add (mwBedSource);
				currentSong.Add (mwCrazyOrganSource);
				currentSong.Add (mwGuitarSource);
				currentSong.Add (mwMelodySource);
				currentSong.Add (mwOrganSource);
				break;
			default:
				break;
		}
	}
}
