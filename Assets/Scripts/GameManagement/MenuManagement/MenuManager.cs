using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XInputDotNetPure;

public enum MenuState { Loading, MainMenu, SettingsMenu, AboutMenu, PlayerSelect, QuickStart };
public class MenuManager
{
    // PUBLIC
    public bool m_PlayRequested = false;
    // PRIVATE
    private MenuState m_State;
    private GameManager m_GameManager;
    private SettingsMenuManager m_SettingsManager = null;
    private PlayerSelectManager m_PlayerSelectManager = null;
    private QuickStartManager m_QuickStartMenu = null;
    private ControlSet m_Controllers = new ControlSet();

    private Canvas m_mainCanvas;
    private Canvas m_playerSelectCanvas;
    private Canvas m_aboutCanvas;
    private Canvas m_settingsCanvas;
    private Canvas m_QuickStartCanvas;
    private MenuState m_StartState;
    private bool m_firstFrame = true;

    // Use this for initialization
    public MenuManager (GameManager gm)
    {
        m_GameManager = gm;
        m_StartState = MenuState.MainMenu;
    }

    public MenuManager (GameManager gm, MenuState start_state)
    {
        m_GameManager = gm;
        m_StartState = start_state;
    }

    public void Update()
    {
        /* This code will run every frame, you should handle what the menu does in each state
         * This could include calling update on another manager if necessary, or nothing */

        // All states
        m_Controllers.Update();
        if (m_firstFrame)
        {
            m_GameManager.Preload_Next_Level();
            m_firstFrame = false;
        }

        switch(m_State)
        {
            case MenuState.Loading:
                {
                    if (m_GameManager.Current_Scene() == "Menu")
                    {
                        Setup();
                        SetState(m_StartState);
                    }
                    else
                    {
                        Debug.Log("MenuManager LOADING: Scene [" + m_GameManager.Current_Scene() + "]");
                    }
                    break;
                }
            case MenuState.MainMenu:
                {
                    // handled by the event manager
                    break;
                }
            case MenuState.AboutMenu:
                {
                    if(m_Controllers.Any_B_Pressed())
                    {
                        SetState(MenuState.MainMenu);
                    }
                    break;
                }
            case MenuState.SettingsMenu:
                {
                    m_SettingsManager.Update();
                    if(m_SettingsManager.Close)
                    {
                        SetState(MenuState.MainMenu);
                    }
                    break;
                }
            case MenuState.PlayerSelect:
                {
                    m_PlayerSelectManager.Update();
                    if(m_PlayerSelectManager.Close)
                    {
                        SetState(MenuState.MainMenu);
                    }
                    else if(m_PlayerSelectManager.Play)
                    {
                        //m_PlayRequested = true;
                        SetState(MenuState.QuickStart);
                    }
                    break;
                }
            case MenuState.QuickStart:
                {
                    m_QuickStartMenu.Update();
                    if (m_QuickStartMenu.Play)
                    {
                        m_PlayRequested = true;
                    }
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in MenuManager.Update()");
                }
        }
        // All States
        //re add shortcut when Y pressed to skip menu once refactoring complete 
    }

    private void LeaveState()
    {
        /* This is the code to run when you are transitioning OUT of a state
         * Used to neatly clean up any data necessary between states */

        switch (m_State)
        {
            case MenuState.Loading:
                {
                    break;
                }
            case MenuState.MainMenu:
                {
                    // close the main menu canvas
                    m_mainCanvas.gameObject.SetActive(false);
                    break;
                }
            case MenuState.AboutMenu:
                {
                    // close the about menu canvas
                    m_aboutCanvas.gameObject.SetActive(false);
                    break;
                }
            case MenuState.SettingsMenu:
                {
                    m_SettingsManager = null;
                    // close the settings menu canvas
                    m_settingsCanvas.gameObject.SetActive(false);
                    break;
                }
            case MenuState.PlayerSelect:
                {
                    m_PlayerSelectManager = null;
                    // close the player select canvas
                    m_playerSelectCanvas.gameObject.SetActive(false);
                    // hide any players?
                    break;
                }
            case MenuState.QuickStart:
                {
                    m_QuickStartCanvas.gameObject.SetActive(false);
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in MenuManager.LeaveState()");
                }
        }
    }
    private void SetState(MenuState new_state)
    {
        /* This will be called to change the MenuManager to a new state.
         * Here you should write the code to set up any data needed in the new state. */

        // leave old state
        LeaveState();
        // change state
        m_State = new_state;
        switch (m_State)
        {
            case MenuState.Loading:
                {
                    break;
                }
            case MenuState.MainMenu:
                {
                    // open the main menu canvas
                    m_mainCanvas.gameObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
                    #if UNITY_STANDALONE_WIN
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;                      
                    #endif
                    break;
                }
            case MenuState.AboutMenu:
                {
                    // open the about menu canvas
                    m_aboutCanvas.gameObject.SetActive(true);
                    break;
                }
            case MenuState.SettingsMenu:
                {
                    // open the settings menu canvas
                    m_settingsCanvas.gameObject.SetActive(true);
                    //sets selected item to first in the settings menu
                    EventSystem.current.SetSelectedGameObject(GameObject.Find("Quality").GetComponent<Dropdown>().gameObject);
                    m_SettingsManager = new SettingsMenuManager(this, m_GameManager);
                    break;
                }
            case MenuState.PlayerSelect:
                {   
                    // open the player select menu canvas
                    m_playerSelectCanvas.gameObject.SetActive(true);
                    m_PlayerSelectManager = new PlayerSelectManager(this, m_GameManager, m_Controllers);
                    m_GameManager.Clear_Players();
                    break;
                }
            case MenuState.QuickStart:
                {
                    m_QuickStartCanvas.gameObject.SetActive(true);
                    m_QuickStartMenu = new QuickStartManager(m_GameManager, m_Controllers);
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in MenuManager.SetState()");
                }
        }
    }
    /*
     * 
     *  ANY ADDITIONAL FUNCTIONS NEEDED GO AFTER THIS POINT.
     *  IF THEY ARE ONLY USED IN THIS CLASS PLEASE MAKE THEM PRIVATE 
     * 
     */

    private void Setup()
    {
        m_mainCanvas = GameObject.Find("mainCanvas").GetComponent<Canvas>();
        m_playerSelectCanvas = GameObject.Find("playerSelectCanvas").GetComponent<Canvas>();
        m_aboutCanvas = GameObject.Find("aboutCanvas").GetComponent<Canvas>();
        m_settingsCanvas = GameObject.Find("settingsCanvas").GetComponent<Canvas>();
        m_QuickStartCanvas = GameObject.Find("quickStartCanvas").GetComponent<Canvas>();

        // create all 4 player controllers for a control set
        for (int i = 0; i < 4; i++)
        {
            m_Controllers.Add_Input(new Controller_Input(i));
        }

        Disable_All_Canvas();
        SetOnClickButtons();
    }
    public void StartGame()
    {
        SetState(MenuState.PlayerSelect);
    }

    public void About()
    {
        SetState(MenuState.AboutMenu);
    }

    public void Settings()
    {
        SetState(MenuState.SettingsMenu);
    }

    public void Quit()
    {
        /* ==================================
        * COMMENTED OUT FOR OPEN DAY VERSION
        * TO NOT ALLOW PEOPLE TO CLOSE OUR
        * GAME AND SCREW THINGS UP
        * ==================================
       #if UNITY_EDITOR

           UnityEditor.EditorApplication.isPlaying = false;
       #else
           Application.Quit();
       #endif
       */
    }

    public void MainMenu()
    {
        SetState(MenuState.MainMenu);
    }

    public ControlSet Get_Controllers()
    {
        return m_Controllers;
    }

    public PlayerSelectManager Get_Player_Select()
    {
        return m_PlayerSelectManager;
    }

    private void Disable_All_Canvas()
    {
        m_mainCanvas.gameObject.SetActive(false);
        m_playerSelectCanvas.gameObject.SetActive(false);
        m_aboutCanvas.gameObject.SetActive(false);
        m_settingsCanvas.gameObject.SetActive(false);
        m_QuickStartCanvas.gameObject.SetActive(false);
    }

    private void SetOnClickButtons()
    {
        Button[] b = m_mainCanvas.GetComponentsInChildren<Button>();
        Button a = m_aboutCanvas.GetComponentInChildren<Button>();
        b[0].onClick.AddListener(StartGame);
        //b[1].onClick.AddListener(About);
        //b[1].onClick.AddListener(Settings);
        //b[1].onClick.AddListener(Quit);
        a.onClick.AddListener(MainMenu);
    }
}
