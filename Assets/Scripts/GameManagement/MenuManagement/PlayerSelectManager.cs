using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectManager {
    public enum PlayerSelectState { NotEnoughPlayers, WaitingForPlayers, AllReady, WaitingToStart };
    //PUBLIC
    public bool Close = false; // DOES THE PLAYER SELECT WANT TO CLOSE
    public bool Play = false; // DOES THE PLAYER SELECT WANT TO START THE GAME
    //PRIVATE
    private PlayerSelectState m_State;
    private Timer m_Timer = null;
    private MenuManager m_MenuManager = null;
    private GameManager m_GameManager = null;
    private ControlSet m_Controllers = null;
    private int m_MinimumPlayers = GLOBAL_VALUES.MINIMUM_PLAYERS;
    private float ReadyUpTime = GLOBAL_VALUES.READY_UP_TIME;
    private float ShortReadyTime = GLOBAL_VALUES.READY_UP_TIME_SHORT;
    private float tempTime;
    private bool m_firstFrame = true;

   // private Colours colours;
    //private Hats hats;

    // text box for timer countdown
    private Text timer;
    // controller select panels 
    private List<PlayerSelectPanel> Panels = new List<PlayerSelectPanel>();

    public PlayerSelectManager(MenuManager menu, GameManager game, ControlSet input)
    {
        m_State = PlayerSelectState.NotEnoughPlayers;
        m_MenuManager = menu;
        m_GameManager = game;
        m_Controllers = input;

        //colours = new Colours(m_GameManager);
        //hats = new Hats(m_GameManager);

        timer = GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchText").GetComponent<Text>();

        for (int i = 1; i < 5; i++)
        {
            Panels.Add(GameObject.Find("PlayerSelectPanel_" + i).GetComponent<PlayerSelectPanel>());
        }

        foreach(PlayerSelectPanel Panel in Panels)
        {
            Panel.Link_Panels(Panels);
        }
    }

	// Update is called once per frame from the menu manager
	public void Update ()
    {
        if (m_firstFrame)
        {
            m_firstFrame = false;
        }
        else
        {
            // ALL STATES
            /* CODE HERE */
            if (m_Timer != null)
            {
                timer.text = Mathf.Round(m_Timer.Get_Time()).ToString();
                m_Timer.Update();
                if (m_Timer.isComplete())
                {
                    Play = true;
                    foreach (PlayerSelectPanel panel in Panels)
                    {
                        if (panel.isOccupied())
                        {
                            panel.ForceState(PanelState.Ready);
                        }
                    }
                }
                else if (m_Timer.Get_Time() <= 10.0f)
                {
                    GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchImage").GetComponent<Animator>().SetTrigger("Activate");
                    GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchPulse").GetComponent<Animator>().SetTrigger("Activate");
                    GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchText").GetComponent<Animator>().SetTrigger("Activate");
                }
                else
                {
                    GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchImage").GetComponent<Animator>().ResetTrigger("Activate");
                    GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchPulse").GetComponent<Animator>().ResetTrigger("Activate");
                    GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchText").GetComponent<Animator>().ResetTrigger("Activate");

                    GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchImage").GetComponent<Animator>().Play("Idle", 0);
                    GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchPulse").GetComponent<Animator>().Play("Idle", 0);
                    GameObject.Find("PlayerSelectTimer").transform.Find("StopWatchText").GetComponent<Animator>().Play("Idle", 0);
                }
            }
            else
            {
                timer.text = "--";
            }

            Check_A_Press(); // Join / Ready
            Check_B_Press(); // Leave / Back

            if (m_GameManager.Player_Count() > 0)
            {
                Check_LT_Press(); // previous color
                Check_RT_Press(); // next color
                Check_LB_Press(); // previous hat
                Check_RB_Press(); // next hat
            }

            switch (m_State)
            {
                case PlayerSelectState.NotEnoughPlayers:
                    {
                        // STATE TRANSITIONS
                        if (m_GameManager.Player_Count() >= m_MinimumPlayers)
                        {
                            // enough players to start. move to waiting
                            SetState(PlayerSelectState.WaitingForPlayers);
                        }
                        break;
                    }
                case PlayerSelectState.WaitingForPlayers:
                    {
                        // STATE TRANSITIONS
                        if (m_GameManager.Player_Count() < m_MinimumPlayers)
                        {
                            // players left and now there is not enough, go back to not enough players
                            SetState(PlayerSelectState.NotEnoughPlayers);
                            break;
                        }

                        if (All_Players_Ready())
                        {
                            // all active players ready, move to all ready
                            SetState(PlayerSelectState.AllReady);
                            break;
                        }
                        break;
                    }
                case PlayerSelectState.AllReady:
                    {
                        if (!All_Players_Ready())
                        {
                            SetState(PlayerSelectState.WaitingForPlayers);
                            break;
                        }
                        if (m_GameManager.Player_Count() < m_MinimumPlayers)
                        {
                            // players left and now there is not enough, go back to not enough players
                            SetState(PlayerSelectState.NotEnoughPlayers);
                            break;
                        }
                        // All players are ready. waiting for timer
                        break;
                    }
                case PlayerSelectState.WaitingToStart:
                    {
                        // Have told the game manager to start a match. waiting.
                        break;
                    }
                default:
                    {
                        // THROW ERROR!
                        throw new System.NotImplementedException("State missing in PlayerSelectManager.Update()");
                    }
            }
            // ALL STATES
            /* CODE HERE */
        }
    }

    private void LeaveState()
    {
        switch (m_State)
        {
            case PlayerSelectState.NotEnoughPlayers:
                {
                    // Code for when you leave not enough players
                    // Create timer?
                    break;
                }
            case PlayerSelectState.WaitingForPlayers:
                {
                    // Code for leaving Waiting for players
                    break;
                }
            case PlayerSelectState.AllReady:
                {
                    // Code for leaving all ready
                    // add time to timer?
                    float calc = tempTime - m_Timer.Get_Time();
                    m_Timer.Add(calc, true);
                    break;
                }
            case PlayerSelectState.WaitingToStart:
                {
                    Play = false;
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in PlayerSelectManager.LeaveState()");
                }
        }
    }
    private void SetState(PlayerSelectState new_state)
    {
        LeaveState();
        // change state
        m_State = new_state;
        switch (m_State)
        {
            case PlayerSelectState.NotEnoughPlayers:
                {
                    // code for entering not enough players
                    // remove timer?
                    m_Timer = null;
                    timer.text = "--";
                    break;
                }
            case PlayerSelectState.WaitingForPlayers:
                {
                    // Code for entering waiting for players
                    // if no timer, create timer?
                    m_Timer = new Timer(ReadyUpTime, true);
                    // else add time to timer?
                    break;
                }
            case PlayerSelectState.AllReady:
                {
                    // Code for entering all ready
                    // remove time from timer
                    float calc;
                    tempTime = m_Timer.Get_Time();
                    if (ShortReadyTime < tempTime)
                    {
                        calc = ShortReadyTime - tempTime;
                    }
                    else
                    {
                        calc = 0;
                    }
                    m_Timer.Add(calc, true);
                    break;
                }
            case PlayerSelectState.WaitingToStart:
                {
                    Play = true;
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in PlayerSelectManager.SetState()");
                }
        }
    }
    /*
     * 
     * ADD ADDITIONAL FUNCTIONS HERE IF THEY ARE ONLY USED IN THIS CLASS PLEASE MAKE THEM PRIVATE
     * 
     */
    private bool All_Players_Ready()
    {
        bool t_ready = true;
        foreach (PlayerSelectPanel panel in Panels)
        {
            if (panel.isOccupied())
            {
                t_ready &= panel.isReady();
            }
        }
        return t_ready;
    }
    private void Check_A_Press()
    {
        foreach(int? num in m_Controllers.A_Pressed_List())
        {
            int control_num = num ?? default(int);
            bool player_found = false;
            // do a actions for each person who hit A
            foreach(PlayerSelectPanel Panel in Panels)
            {
                // check if it has a player already
                if (Panel.isOccupied() && (Panel.Get_Controller_Number() == control_num))
                {
                    Panel.Ready_Up();
                    player_found = true;
                    break;
                }
            }
            // if no player already exists, add it
            if (!player_found)
            {
                int? empty_panel = findFirstEmptyPanel();
                if (empty_panel != null)
                {
                    int empty_index = empty_panel ?? default(int);
                    Panels[empty_index].Add_Player(control_num);
                }
            }
        }
    }

    private void Check_B_Press()
    {
        if ((m_GameManager.Player_Count() == 0) && (m_Controllers.Any_B_Pressed()))
        {
            // nobody current in the game, go back to main menu
            Close = true;
        }
        else
        {
            // there is someone to leave
            foreach (int? num in m_Controllers.B_Pressed_List())
            {
                int controller_num = num ?? default(int);
                // for each controller who hit B call remove on their panel if they exist
                foreach(PlayerSelectPanel Panel in Panels)
                {
                    if(Panel.isOccupied() && (Panel.Get_Controller_Number() == controller_num))
                    {
                        // Player exists, let the panel decide what to do
                        Panel.Cancel();
                        break;
                    }
                }
            }
        }
    }

    private void Check_LT_Press()
    {

        foreach (int? num in m_Controllers.LT_Pressed_List())
        {
            int controller_num = num ?? default(int);
            foreach(PlayerSelectPanel Panel in Panels)
            {
                if (Panel.isOccupied() && (Panel.Get_Controller_Number() == controller_num))
                {
                    Panel.Previous_Color();
                    break;
                }
            }
        }              
    }

    private void Check_RT_Press()
    {
        foreach (int? num in m_Controllers.RT_Pressed_List())
        {
            // do up actions for each person who hit Down
            int controller_num = num ?? default(int);
            foreach (PlayerSelectPanel Panel in Panels)
            {
                if (Panel.isOccupied() && (Panel.Get_Controller_Number() == controller_num))
                {
                    Panel.Next_Color();
                    break;
                }
            }
        }
    }
    private void Check_LB_Press()
    {
        foreach (int? num in m_Controllers.LB_Pressed_List())
        {
            int controller_num = num ?? default(int);
            foreach (PlayerSelectPanel Panel in Panels)
            {
                if (Panel.isOccupied() && (Panel.Get_Controller_Number() == controller_num))
                {
                    Panel.Previous_Hat();
                    break;
                }
            }
        }
    }
    private void Check_RB_Press()
    {
        foreach (int? num in m_Controllers.RB_Pressed_List())
        {
            int controller_num = num ?? default(int);
            foreach (PlayerSelectPanel Panel in Panels)
            {
                if (Panel.isOccupied() && (Panel.Get_Controller_Number() == controller_num))
                {
                    Panel.Next_Hat();
                    break;
                }
            }
        }
    }

    private int? findFirstEmptyPanel()
    {
        foreach(PlayerSelectPanel Panel in Panels)
        {
            if(!Panel.isOccupied())
            {
                return Panels.IndexOf(Panel);
            }
        }
        return null;
    }
}
