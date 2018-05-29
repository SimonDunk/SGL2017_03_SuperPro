using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager
{
    private enum RoundState { Waiting, Setup, Starting, Playing, Paused, Scoring };
    public bool Finished = false;
    // PUBLIC
    public AudioClip sound;

    // PRIVATE
    private MatchManager m_MatchManager = null;
    private GameManager m_GameManager = null;
    
    private AI_Manager m_AI_Manager = null;
    private ItemManager m_ItemManager = null;
    private SaleManager m_SaleManager = null;
    private ScoreScreenManager m_ScoreScreen = null;
    private CameraManager m_CameraManager = null;

    private Stat_Printer m_Stat_Printer = null;
    private AgentManager m_Round_Winner = null;
    private GameObject m_HUD = null;
    private Text m_UIClock_Text = null;
    private AudioSource source = null;
    private UITimer uiClock = null;
    private List<SpinningLight> m_EndLights = null;
    private bool m_endlightsStarted = false;
    private bool SetupComplete = false;

    private bool m_ClockAnimation = false;

    private Timer m_Timer = new global::Timer();
    private GameObject m_Countdown = null;

    private JukeBox m_jukebox;

    private RoundState m_State;

    public CameraManager Get_Camera_Manager()
    {
        return m_CameraManager;
    }
    public RoundManager(MatchManager match_manager, GameManager game_manager)
    {
        m_State = RoundState.Waiting;

        m_MatchManager = match_manager;
        m_GameManager = game_manager;
        m_Stat_Printer = new Stat_Printer(m_GameManager);
        m_CameraManager = new CameraManager(this, m_GameManager);

        sound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_ROUND_END);
        m_jukebox = GameObject.Find("CameraRig").GetComponentInChildren<JukeBox>();
    }

    ~RoundManager()
    {
        // Destructor for the RoundManager
        m_Stat_Printer = null;
        m_Round_Winner = null;
        m_AI_Manager = null;
        m_ItemManager = null;
        m_ScoreScreen = null;
        m_HUD = null;
        m_MatchManager = null;
        m_GameManager = null;
        source = null;
        uiClock = null;
        m_CameraManager = null;
    }
    public void Update()
    {
        // Update each frame
        // update the timers
        if (SetupComplete)
        {
            m_Timer.Update();
            Update_HUD_Timer();
        }

        switch (m_State)
        {
            case RoundState.Waiting:
                {
                    // Game hasnt been told to start yet so do nothing while waiting
                    if (m_Round_Winner == null)
                    {
                        SetState(RoundState.Setup);
                    }
                    break;
                }
            case RoundState.Setup:
                {
                    if (SetupComplete)
                    {
                        SetState(RoundState.Starting);
                    }
                    break;
                }
            case RoundState.Starting:
                {
                    // Just got told to start, wait for pre round timer before playing.
                    if (m_Timer.isComplete())
                    {
                        SetState(RoundState.Playing);
                    }
                    break;
                }
            case RoundState.Playing:
                {
                    if (m_ClockAnimation == false && m_Timer.Get_Time() < 10.0)
                    {
                        GameObject.Find("HUD").transform.Find("TimerObject").Find("Image").Find("ClockTimer").GetComponent<Animator>().SetTrigger("Activate");
                        GameObject.Find("HUD").transform.Find("TimerObject").Find("Image").Find("TimerPulse").GetComponent<Animator>().SetTrigger("Activate");
                        GameObject.Find("HUD").transform.Find("TimerObject").Find("Image").Find("TimerText").GetComponent<Animator>().SetTrigger("Activate");
                        
                        if (!m_endlightsStarted)
                        {
                            foreach (SpinningLight light in m_EndLights)
                            {
                                int direction = Random.Range(0, 2);
                                int speed = Random.Range(5, 9);
                                light.Activate(direction, speed, 10f);
                                m_endlightsStarted = true;
                            }
                        }
                    }
                    if (m_Timer.Get_Time() < 11)
                    {
                        GameObject.Find("HUD").transform.Find("RoundEndCountdown").Find("RoundEndCountdownImage").GetComponent<Animator>().SetTrigger("Activate");
                        GameObject.Find("HUD").transform.Find("RoundEndCountdown").Find("RoundEndCountdownPulse").GetComponent<Animator>().SetTrigger("Activate");
                    }
                    if (m_Timer.Get_Time() < 1)
                    {
                        GameObject.Find("FlashCanvas").transform.Find("Flash").GetComponent<Animator>().SetTrigger("Activate");
                    }
                    // Starting the playable round
                    if (PauseMenu.isPaused)
                    {
                        SetState(RoundState.Paused);
                    }
                    else if (m_Timer.isComplete())
                    {
                        SetState(RoundState.Scoring);
                    }
                    else
                    {
                        foreach (AgentManager player in m_GameManager.Get_Players())
                        {
                            player.Update();
                        }
                        m_SaleManager.Update();
                        m_ScoreScreen.Update();
                        m_CameraManager.Update();
                    }
                    break;
                }
            case RoundState.Paused:
                {
                    // Game has been paused do nothing
                    if (!PauseMenu.isPaused)
                    {
                        SetState(RoundState.Playing);
                    }
                    break;
                }
            case RoundState.Scoring:
                {
                    // Round over, score screen is running
                    if (m_ScoreScreen.Close)
                    {
                        Finished = true;
                        SetState(RoundState.Waiting);
                    }
                    else
                    {
                        m_ScoreScreen.Update();
                        foreach (AgentManager player in m_GameManager.Get_Players())
                        {
                            player.Update();
                        }
                    }
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in RoundManager.Update()");
                }
        }
    }

    private void SetState(RoundState state)
    {
        // Leave Previous State
        LeaveState();

        // New state
        m_State = state;
        switch (state)
        {
            case RoundState.Waiting:
                {
                    // Game hasnt been told to start yet
                    // Wait for further instructions
                    m_Timer.Reset();
                    break;
                }
            case RoundState.Setup:
                {
                    SetupRound();
                    break;
                }
            case RoundState.Starting:
                {
                    // Just got told to start, setup and start the start timer before playing.
                    m_Timer.Reset();
                    m_Timer.Add(GLOBAL_VALUES.ROUND_START_WAIT_TIME, true);
                    ResetPlayers();
                    DisablePlayers();
                    m_ItemManager.Reset_Me();
                    m_ItemManager.Spawn_Items(m_ItemManager.m_Spawners.Count);
                    m_Countdown.SetActive(true);
                    break;
                }

            case RoundState.Playing:
                {
                    m_HUD.SetActive(true);
                    m_Timer.Start();
                    ResumePlayers();
                    m_ItemManager.Resume();
                    // Starting the playable round
                    EnablePlayers();
                    break;
                }

            case RoundState.Paused:
                {
                    // Game has been paused
                    m_Timer.Pause();
                    PausePlayers();
                    m_ItemManager.Pause();
                    m_AI_Manager.Pause_AI();
                    m_GameManager.RumbleAll(0, 0);
                    break;
                }
            case RoundState.Scoring:
                {
                    m_HUD.SetActive(false);
                    m_Stat_Printer.Print_Stats(); // print out the collected stats
                    Heatmap_Printer hmp = GameObject.Find("Heatmap").GetComponent<Heatmap_Printer>();
                    if (hmp != null)
                    {
                        hmp.Print_Heatmap_Stats();
                    }
                    else
                    {
                        Debug.Log("ERROR: NO HEATMAP PRINTER COULD BE FOUND");
                    }
                    // play end of round sound
                    // Round over, score screen running
                    DisablePlayers(); // stop all players
                    m_ScoreScreen.Activate();
                    m_ItemManager.Disable_Timer(); // Stop the item manager
                    m_GameManager.RumbleAll(0, 0);
                    m_jukebox.PlayScoreScreen();
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in RoundManager.SetState()");
                }
        }
        m_State = state;
    }

    private void LeaveState()
    {
        switch(m_State)
        {
            case RoundState.Paused:
                {
                    m_Timer.Start();
                    ResumePlayers();
                    m_ItemManager.Resume();
                    m_AI_Manager.Resume_AI();
                    break;
                }
            case RoundState.Setup:
                {
                    break;
                }
            case RoundState.Playing:
                {
                    break;
                }
            case RoundState.Scoring:
                {
                    m_ScoreScreen = null;
                    foreach(AgentManager a in m_GameManager.Get_Players())
                    {
                        a.Set_Scale(1);
                    }
                    break;
                }
            case RoundState.Starting:
                {
                    //m_Countdown.SetActive(false);
                    m_Timer.Reset();
                    m_Timer.Add(m_GameManager.Get_Settings().RoundDuration, true);
                    m_ItemManager.Enable_Timer();
                    m_ItemManager.EnableSounds();
                    break;
                }
            case RoundState.Waiting:
                {
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in RoundManager.LeaveState()");
                }
        }
    }

    /*********************
     * ALL OTHER FUNCTIONS
     *********************/
    public void SetupRound()
    {
        // Link all objects
        m_HUD = GameObject.Find("HUD");
        m_HUD.transform.Find("ScoreIndicators").GetComponent<ScoreUIManager>().Init();
        m_AI_Manager = GameObject.FindGameObjectWithTag(GLOBAL_VALUES.TAG_AI_MANAGER).GetComponent<AI_Manager>();
        uiClock = GameObject.Find("ClockTimer").GetComponent<UITimer>();

        source = m_HUD.GetComponent<AudioSource>();
        m_ItemManager = m_HUD.gameObject.GetComponent<ItemManager>();
        m_SaleManager = new SaleManager(m_ItemManager);
        m_CameraManager = new CameraManager(this, m_GameManager);
        m_CameraManager.SetActiveCamera(GameObject.Find("Main Camera").GetComponent<Camera>());
        m_CameraManager.SetupScene();
        m_UIClock_Text = GameObject.Find("TimerText").GetComponent<Text>();
        m_ScoreScreen = new ScoreScreenManager(this, m_GameManager);
        SpawnPlayers();
        m_ClockAnimation = false;
        m_Countdown = GameObject.Find("CountDown");
        m_Countdown.SetActive(false);
        m_EndLights = new List<SpinningLight>();
        foreach (GameObject light in GameObject.FindGameObjectsWithTag("LIGHTS_END"))
        {
            m_EndLights.Add(light.GetComponent<SpinningLight>());
        }
        m_endlightsStarted = false;
        foreach (AgentManager a in m_GameManager.Get_Players())
        {
            //a.Get_Powerup_Manager().Setup_Camera_Manager(m_CameraManager);
            a.Setup_Item_Manager(m_ItemManager);
            a.GetPunchControl().SetEffectColour();
        }
        SetupComplete = true;
    }

    private void SpawnPlayers()
    {
        /* Spawn players in at a random position within a random spawn zone
         * since the player objects are already created, you only need to move them */
        foreach (AgentManager player in m_GameManager.Get_Players())
        {
            SpawnZone Spawner = Get_Random_Spawn_Point();
            player.Set_Spawn_Point(Spawner.gameObject);
            Vector3 new_position = Spawner.Get_Unique_Random_Spawn_Point();
            player.Set_Position(new_position);
        }
    }

    public AgentManager Get_Winner()
    {
        // Returns the winner of the round or null
        return m_Round_Winner;
    }

    // finding who won the round
    private AgentManager GetRoundWinner()
    {
        AgentManager ret = null;
        int winner_count = 0;
        // Check who has the highest score!
        foreach(AgentManager player in m_GameManager.Get_Players())
        {
            if(ret == null || (player.Get_Score() > ret.Get_Score()))
            {
                // for checking the highest score
                ret = player;
                winner_count = 1;
            }
            else if (player.Get_Score() == ret.Get_Score())
            {
                // for checking for a draw
                winner_count++;
            }
        }
        return winner_count == 1 ? ret : null;
    }

    private SpawnZone Get_Random_Spawn_Point()
    {
        // Get List of Spawn Zones
        GameObject[] Spawn_Zones = GameObject.FindGameObjectsWithTag(GLOBAL_VALUES.PLAYER_SPAWN);
        // Get a random one
        return Spawn_Zones[Random.Range(0, Spawn_Zones.Length)].GetComponent<SpawnZone>();
    }

    //used for displaying time on HUD
    private void Update_HUD_Timer()
    {
        // get the latest timer
        float time = m_Timer.Get_Time();
        // convert to minutes
        int minutes = (int)(Mathf.Round(time) / 60);
        int seconds = (int)(Mathf.Round(time) % 60);
        string sec_str = seconds >= 10 ? "" + seconds : "0" + seconds;
        m_UIClock_Text.text = minutes + ":" + sec_str;

        // Updated clock timer
        if (m_State == RoundState.Playing)
        {
            uiClock.UpdateTimer(time);
        }
    }

    //teleport players to spawn
    private void ResetPlayers()
    {
        foreach (AgentManager player in m_GameManager.Get_Players())
        {
            player.Reset();
        }
    }

    //disable actions from player
    private void DisablePlayers()
    {
        foreach(AgentManager player in m_GameManager.Get_Players())
        {
            player.Disable();
        }
    }

    //reenable actions from player
    private void EnablePlayers()
    {
        foreach (AgentManager player in m_GameManager.Get_Players())
        {
            player.Enable();
        }
    }

    private void PausePlayers()
    {
        foreach (AgentManager player in m_GameManager.Get_Players())
        {
            player.Pause();
        }
        m_AI_Manager.Pause_AI();
    }

    private void ResumePlayers()
    {
        foreach (AgentManager player in m_GameManager.Get_Players())
        {
            player.Resume();
        }
        m_AI_Manager.Resume_AI();
    }

    public ItemManager Get_ItemManager()
    {
        return m_ItemManager;
    }

    public SaleManager Get_SaleManager()
    {
        return m_SaleManager;
    }
    
}
