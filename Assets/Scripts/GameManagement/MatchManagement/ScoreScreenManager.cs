using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ScoreScreenState { Waiting, Setup, Dropping, Winner, Badges, Complete }
public class ScoreScreenManager {
    // Background
    private GameObject m_Background = null;
    // Player Objects
    private List<GameObject> m_DropZones = new List<GameObject>();
    private List<Transform> m_Drop_Points = new List<Transform>();
    private List<Transform> m_Player_Points = new List<Transform>();
    private List<Transform> m_Winner_Points = new List<Transform>();
    private List<GameObject> m_Trophies = new List<GameObject>();
    private List<List<GameObject>> m_Lights = new List<List<GameObject>>();
    // Foreground
    private GameObject m_Foreground = null;
    private List<GameObject> m_Badges = new List<GameObject>();
    private List<GameObject> m_Badge_Points = new List<GameObject>();
    private List<Text> m_Winner_Text = new List<Text>();
    //PUBLIC
    public bool Close;
    public Camera m_Camera = null;
    public Canvas m_Canvas = null;

    //PRIVATE
    private GameObject m_MainObject = null;
    private ScoreScreenState m_State;
    private bool SetupComplete = false;
    private RoundManager m_RoundManager = null;
    private ItemManager m_ItemManager = null;
    private GameManager m_GameManager = null;
    private CameraManager m_CameraManager = null;
    private AudioSource m_source = null;
    
    private List<GameObject> m_DroppedItems = new List<GameObject>();
    private List<int> m_Score_Values = new List<int>();
    private GameObject m_confettiEffects = null;
    private ParticleSystem m_dustEffect = null;

    private bool m_Activated = false;

    private Timer m_Timer = new Timer();

    public void Activate()
    {
        m_Activated = true;
    }

    public ScoreScreenManager(RoundManager rm, GameManager gm)
    {
        // setup
        m_MainObject = GameObject.Find("ScoreScreen");
        m_State = ScoreScreenState.Setup;
        m_RoundManager = rm;
        m_ItemManager = m_RoundManager.Get_ItemManager();
        m_GameManager = gm;
        // Camera
        m_Camera = m_MainObject.transform.Find("Score_Camera").GetComponent<Camera>();
        m_Camera.enabled = false;
        m_CameraManager = rm.Get_Camera_Manager();
        // Foreground and Background
        m_Background = m_MainObject.transform.Find("Backdrop").gameObject;
        m_Foreground = m_MainObject.transform.Find("Foreground").gameObject;
        m_confettiEffects = m_MainObject.transform.Find("ConfettiParticles").gameObject;
        m_dustEffect = m_MainObject.transform.Find("RoundEnd_Dust").GetComponent<ParticleSystem>();
        m_source = m_MainObject.GetComponent<AudioSource>();
        // Find all drop zones
        for (int i = 1; i < 5; i++)
        {
            // Populate drop zones
            m_DropZones.Add(m_MainObject.transform.Find("PlayerDropZone_" + i).gameObject);
            m_Winner_Text.Add(m_Foreground.transform.Find("Player" + i + "_WinText").GetComponent<Text>());
            for(int badge = 1; badge <= 4; badge++)
            {
                m_Badge_Points.Add(GameObject.Find("Player" + badge + "_Badge"));
            }
            foreach(GameObject o in GameObject.FindGameObjectsWithTag("BADGE_IMAGE"))
            {
                m_Badges.Add(o);
            }
        }
        // turn off all winner texts
        foreach(Text t in m_Winner_Text)
        {
            t.enabled = false;
        }
        // turn off all badges
        foreach(GameObject o in m_Badges)
        {
            o.SetActive(false);
        }
        // Find all drop zone objects
        foreach(GameObject dz in m_DropZones)
        {
            m_Drop_Points.Add(dz.transform.Find("DropPoint"));
            m_Player_Points.Add(dz.transform.Find("PlayerPoint"));
            m_Winner_Points.Add(dz.transform.Find("PlayerWinnerPoint"));
            m_Trophies.Add(dz.transform.Find("Trophy").gameObject);
            List<GameObject> l_lights = new List<GameObject>();
            for (int i = 1; i < 4; i++)
            {
                l_lights.Add(dz.transform.Find("Win_Light_" + i).gameObject);
            }
            m_Lights.Add(l_lights);
        }
        // turn all trophies and lights off
        foreach(GameObject t in m_Trophies)
        {
            t.SetActive(false);
        }
        foreach(List<GameObject> l_list in m_Lights)
        {
            foreach(GameObject l in l_list)
            {
                l.SetActive(false);
            }
        }
        SetupComplete = true;
    }
	
	// Update is called once per frame
	public void Update () {
        m_Timer.Update();
        switch(m_State)
        {
            case ScoreScreenState.Waiting:
                {
                    // do nothing, waiting to start;
                    if (m_Activated)
                    {
                        SetState(ScoreScreenState.Dropping);
                    }
                    break;
                }
            case ScoreScreenState.Setup:
                {
                    // hoping that setup completed
                    if (SetupComplete)
                    {
                        SetState(ScoreScreenState.Waiting);
                    }
                    break;
                }
            case ScoreScreenState.Dropping:
                {
                    // if more items need to be dropped, drop them
                    if((m_Timer.isComplete()) && (More_Items_Remaining()))
                    {
                        Drop_Item();
                        // after dropping 1 item, is there more left?
                        if (!More_Items_Remaining())
                        {
                            // None left, show winner
                            SetState(ScoreScreenState.Winner);
                        }
                        else
                        {
                            // Set timer to drop next item
                            m_Timer.Add(GLOBAL_VALUES.SCORE_SCREEN_DROP_SPEED, true);
                        }
                    }
                    else if (!More_Items_Remaining())
                    {
                        SetState(ScoreScreenState.Winner);
                    }
                    break;
                }
            case ScoreScreenState.Winner:
                {
                    // dropping complete, winner information showing
                    if (m_Timer.isComplete())
                    {
                        SetState(ScoreScreenState.Badges);
                    }
                    break;
                }
            case ScoreScreenState.Badges:
                {
                    // Badges shown, waiting for end of round timer
                    if (m_Timer.isComplete())
                    {
                        SetState(ScoreScreenState.Complete);
                    }
                    break;
                }
            case ScoreScreenState.Complete:
                {
                    // simply waiting for the round manager to realise we are finished
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in ScoreScreenManager.Update()");
                }
        }
	}

    private void SetState(ScoreScreenState new_state)
    {
        // leave previous state
        LeaveState();
        // change state
        m_State = new_state;
        switch (m_State)
        {
            case ScoreScreenState.Waiting:
                {
                    m_Activated = false;
                    break;
                }
            case ScoreScreenState.Setup:
                {
                    // reset the flag so you know current setup is completed.
                    SetupComplete = false;
                    break;
                }
            case ScoreScreenState.Dropping:
                {
                    // Reset all scores.
                    List<AgentManager> new_list = m_GameManager.Get_Players();
                    for (int i = 0; i < m_GameManager.Player_Count(); i++)
                    {
                        m_Score_Values.Add(new_list[i].Get_Score());
                    }
                    m_CameraManager.SetActiveCamera(m_Camera);
                    List<AgentManager> players = m_GameManager.Get_Players();
                    // show players
                    for (int i = 0; i < players.Count; i++)
                    {
                        AgentManager player = players[i];
                        player.Drop(false);
                        player.Set_Default_State();
                        player.Set_Position(m_Player_Points[i].position);
                        player.Set_Rotation(m_Player_Points[i].rotation);
                        player.Set_Scale(2);
                        player.Wake_Up();
                    }

                    // starting to drop items, set up the timer
                    if (m_Timer!= null)
                    {
                        m_Timer.Reset();
                        m_Timer.Add(GLOBAL_VALUES.SCORE_SCREEN_DROP_SPEED, true);
                    }
                    else
                    {
                        m_Timer = new Timer(GLOBAL_VALUES.SCORE_SCREEN_DROP_SPEED, true);
                    }

                    m_source.clip = Resources.Load("Sound/snd_DrumRoll") as AudioClip;
                    m_source.Play();
                    m_Background.transform.Find("TopBar").GetComponent<Animator>().SetTrigger("Activate");
                    m_Background.transform.Find("BottomBarPanel").transform.Find("BottomBar").GetComponent<Animator>().SetTrigger("Activate");
                    break;
                }
            case ScoreScreenState.Winner:
                {
                    // starting to show the winner of the round screen
                    // setup the timer
                    if (m_Timer != null)
                    {
                        m_Timer.Reset();
                        m_Timer.Add(GLOBAL_VALUES.SCORE_SCREEN_BADGE_DELAY_AFTER_WINNER, true);
                    }
                    else
                    {
                        m_Timer = new Timer(GLOBAL_VALUES.SCORE_SCREEN_BADGE_DELAY_AFTER_WINNER, true);
                    }
                    // do the extra celebrations we want
                    m_dustEffect.Play();
                    m_source.Stop();
                    m_source.PlayOneShot(Resources.Load("Sound/snd_DrumRollEnd") as AudioClip);
                    m_source.loop = false;
                    // TODO
                    List<int> winners = Get_Winners();
                    List<AgentManager> players = m_GameManager.Get_Players();
                    foreach (int win_index in winners)
                    {
                        // Show trophy
                        m_Trophies[win_index].SetActive(true);
                        // Drop Confetti
                        foreach (ParticleSystem p in m_confettiEffects.GetComponentsInChildren<ParticleSystem>())
                        {
                            p.Play();
                        }
                        // move player
                        players[win_index].Set_Position(m_Winner_Points[win_index].position);
                        players[win_index].Set_Rotation(m_Winner_Points[win_index].rotation);
                        players[win_index].Give_Round_Win();
                        //FIX WAVE TO LOOP
                        players[win_index].Get_Animator().SetTrigger(GLOBAL_VALUES.ANIM_TRIGGER_WAVE);
                        m_Winner_Text[win_index].enabled = true;
                        m_Winner_Text[win_index].text = players[win_index].Get_Name() + "\nWINS!";
                        // Show lights
                        foreach (GameObject lo in m_Lights[win_index])
                        {
                            lo.SetActive(true);
                        }
                    }
                    break;
                }
            case ScoreScreenState.Badges:
                {
                    // Show Badges
                    Show_Badges();
                    // Setup Timing
                    if (m_Timer != null)
                    {
                        m_Timer.Reset();
                        m_Timer.Add(GLOBAL_VALUES.SCORE_SCREEN_WAIT_TO_COMPLETE, true);
                    }
                    else
                    {
                        m_Timer = new Timer(GLOBAL_VALUES.SCORE_SCREEN_WAIT_TO_COMPLETE, true);
                    }
                    break;
                }
            case ScoreScreenState.Complete:
                {
                    Close = true;
                    m_CameraManager.SetActiveCamera(m_CameraManager.MainCamera());
                    foreach (GameObject item in m_DroppedItems)
                    {
                        GameObject.Destroy(item);
                    }
                    m_DroppedItems.Clear();
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in ScoreScreenManager.SetState() [" + new_state + "]");
                }
        }
    } 

    private void LeaveState()
    {
        switch (m_State)
        {
            case ScoreScreenState.Waiting:
                {
                    break;
                }
            case ScoreScreenState.Setup:
                {
                    break;
                }
            case ScoreScreenState.Dropping:
                {
                    break;
                }
            case ScoreScreenState.Winner:
                {
                    break;
                }
            case ScoreScreenState.Badges:
                {

                    break;
                }
            case ScoreScreenState.Complete:
                {
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in ScoreScreenManager.LeaveState() [" + m_State + "]");
                }
        }
    }

    void Drop_Item()
    {
        // Pick a random item from the pool of delivered items
        // and drop it in the zone of the player who delivered it

        // Pick random number
        int Roll = Random.Range(0, (m_ItemManager.m_Delivered_Items.Count - 1));
        // Get item of random number
        Item tempItem = m_ItemManager.m_Delivered_Items[Roll];
        // Get the number of the player who delivered it
        int delivered_by = tempItem.m_DeliveredBy.Get_Player_Number();
        if (delivered_by != 0)
        {
            // get the index from the player
            int player_index = delivered_by - 1;
            // Remove the item from the Item Managers list so its not picked again
            m_ItemManager.m_Delivered_Items.Remove(tempItem);
            // keep it in the list of already dropped items
            m_DroppedItems.Add(tempItem.gameObject);
            // move the item to the position of the drop zone
            tempItem.transform.position = m_Drop_Points[player_index].position;
            // turn off all drag for the object so it falls nicer
            Rigidbody rb = tempItem.gameObject.GetComponent<Rigidbody>();
            tempItem.itemBody.drag = 0;
            // add a force so it falls faster
            rb.AddForce(new Vector3(0, -1, 0));
        }
    }
    private bool More_Items_Remaining()
    {
        return m_ItemManager.m_Delivered_Items.Count > 0;
    }

    private void Show_Badges()
    {
        for (int i = 0; i < m_GameManager.Player_Count(); i++)
        {
            // Figure out which player needs which badge
            Assign_Badges();
            // Change the Text on the badge
            foreach(AgentManager a in m_GameManager.Get_Players())
            {
                // move their badge
                GameObject badge = Get_Badge_Object(a.Get_Badge());
                if (badge != null)
                {
                    badge.transform.position = m_Badge_Points[a.Get_Player_Number() - 1].transform.position;
                    switch (badge.name)
                    {
                        case "Badge_SALE_TIME":
                            {
                                badge.transform.Find("TEXT_COUNT").GetComponent<Text>().text = a.m_StatCollector.Get_Sale_Items_Checkedout().ToString();
                                break;
                            }
                        case "Badge_MEGA_SALE_TIME":
                            {
                                badge.transform.Find("TEXT_COUNT").GetComponent<Text>().text = a.m_StatCollector.Get_MegaSale_Items_Checkedout().ToString();
                                break;
                            }
                        case "Badge_EVERYTHING_MUST_GO":
                            {
                                badge.transform.Find("TEXT_COUNT").GetComponent<Text>().text = a.m_StatCollector.Get_Items_Bought().ToString();
                                break;
                            }
                        case "Badge_THEIF":
                            {
                                badge.transform.Find("TEXT_COUNT").GetComponent<Text>().text = a.m_StatCollector.Get_Items_Stolen().ToString();
                                break;
                            }
                        default:
                            {
                                // change nothing
                                break;
                            }
                    }
                    badge.SetActive(true);
                    badge.GetComponent<Animator>().SetTrigger("Activate");
                }
            }
        }
    }

    private GameObject Get_Badge_Object(string name)
    {
        foreach(GameObject o in m_Badges)
        {
            if (o.name == "Badge_" + name)
            {
                return o;
            }
        }
        return null;
    }
    private void Assign_Badges()
    {
        List<AgentManager> m_Players = m_GameManager.Get_Players();
        // worst badges first
        //YOU_CAN_RUN
        AgentManager badge_winner = null;
        float max_time = 0.0f;
        foreach(AgentManager a in m_Players)
        {
            if (a.m_StatCollector.Get_Security_Chase_Time() > max_time)
            {
                max_time = a.m_StatCollector.Get_Security_Chase_Time();
                badge_winner = a;
            }
            else if (a.m_StatCollector.Get_Security_Chase_Time() == max_time)
            {
                badge_winner = null;
            }
        }
        if (badge_winner != null)
        {
            badge_winner.Give_Badge("YOU_CAN_RUN");
            badge_winner = null;
        }
        //SELF_MEDICATOR
        float max_heal = 0.0f;
        foreach (AgentManager a in m_Players)
        {
            if (a.m_StatCollector.Get_Heal() > max_heal)
            {
                max_heal = a.m_StatCollector.Get_Heal();
                badge_winner = a;
            }
            else if (a.m_StatCollector.Get_Heal() == max_heal)
            {
                badge_winner = null;
            }
        }
        if (badge_winner != null)
        {
            badge_winner.Give_Badge("SELF_MEDICATOR");
            badge_winner = null;
        }
        //BURIED_ALIVE
        int max_items_hit = 0;
        foreach (AgentManager a in m_Players)
        {
            if (a.m_StatCollector.Get_Stunned_By_Items() > max_items_hit)
            {
                max_items_hit = a.m_StatCollector.Get_Heal();
                badge_winner = a;
            }
            else if (a.m_StatCollector.Get_Stunned_By_Items() == max_items_hit)
            {
                badge_winner = null;
            }
        }
        if (badge_winner != null)
        {
            badge_winner.Give_Badge("BURIED_ALIVE");
            badge_winner = null;
        }
        //PUNCHING_BAG
        float max_stun_time = 0.0f;
        foreach (AgentManager a in m_Players)
        {
            if (a.m_StatCollector.Get_Stunned_By_Items() > max_stun_time)
            {
                max_stun_time = a.m_StatCollector.Get_Stun_Time();
                badge_winner = a;
            }
            else if (a.m_StatCollector.Get_Stun_Time() == max_stun_time)
            {
                badge_winner = null;
            }
        }
        if (badge_winner != null)
        {
            badge_winner.Give_Badge("PUNCHING_BAG");
            badge_winner = null;
        }
        //EVERYTHING_MUST_GO
        int max_total_items = 0;
        foreach (AgentManager a in m_Players)
        {
            if (a.m_StatCollector.Get_Items_Bought() > max_total_items)
            {
                max_total_items = a.m_StatCollector.Get_Items_Bought();
                badge_winner = a;
            }
            else if (a.m_StatCollector.Get_Items_Bought() == max_total_items)
            {
                badge_winner = null;
            }
        }
        if (badge_winner != null)
        {
            badge_winner.Give_Badge("EVERYTHING_MUST_GO");
            badge_winner = null;
        }
        //INSURANCE
        int max_env_damage = 0;
        foreach (AgentManager a in m_Players)
        {
            if (a.m_StatCollector.Get_Environmental_Damage() > max_env_damage)
            {
                max_env_damage = a.m_StatCollector.Get_Environmental_Damage();
                badge_winner = a;
            }
            else if (a.m_StatCollector.Get_Environmental_Damage() == max_env_damage)
            {
                badge_winner = null;
            }
        }
        if (badge_winner != null)
        {
            badge_winner.Give_Badge("INSURANCE");
            badge_winner = null;
        }
        //SALE_TIME
        int max_sales_bought = 0;
        foreach (AgentManager a in m_Players)
        {
            if (a.m_StatCollector.Get_Sale_Items_Checkedout() > max_sales_bought)
            {
                max_sales_bought = a.m_StatCollector.Get_Sale_Items_Checkedout();
                badge_winner = a;
            }
            else if (a.m_StatCollector.Get_Sale_Items_Checkedout() == max_sales_bought)
            {
                badge_winner = null;
            }
        }
        if (badge_winner != null)
        {
            badge_winner.Give_Badge("SALE_TIME");
            badge_winner = null;
        }
        //THEIF
        int max_item_stolen = 0;
        foreach (AgentManager a in m_Players)
        {
            if (a.m_StatCollector.Get_Items_Stolen() > max_item_stolen)
            {
                max_item_stolen = a.m_StatCollector.Get_Items_Stolen();
                badge_winner = a;
            }
            else if (a.m_StatCollector.Get_Items_Stolen() == max_item_stolen)
            {
                badge_winner = null;
            }
        }
        if (badge_winner != null)
        {
            badge_winner.Give_Badge("THEIF");
            badge_winner = null;
        }
        //MEGA_SALE_TIME
        int max_mega_sales = 0;
        foreach (AgentManager a in m_Players)
        {
            if (a.m_StatCollector.Get_MegaSale_Items_Checkedout() > max_mega_sales)
            {
                max_mega_sales = a.m_StatCollector.Get_MegaSale_Items_Checkedout();
                badge_winner = a;
            }
            else if (a.m_StatCollector.Get_MegaSale_Items_Checkedout() == max_mega_sales)
            {
                badge_winner = null;
            }
        }
        if (badge_winner != null)
        {
            badge_winner.Give_Badge("MEGA_SALE_TIME");
            badge_winner = null;
        }
    }
    private List<int> Get_Winners()
    {
        int highest_score = int.MinValue;
        List<int> winners = new List<int>();
        for (int i = 0; i < m_Score_Values.Count; i++)
        {
            int score = m_Score_Values[i];
            if(score > highest_score)
            {
                winners.Clear();
                highest_score = score;
                winners.Add(i);
            }
            else if (score == highest_score)
            {
                winners.Add(i);
            }
        }
        return winners;
    }
}
