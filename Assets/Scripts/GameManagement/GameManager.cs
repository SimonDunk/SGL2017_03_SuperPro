using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Menu, Match };
public class GameManager : MonoBehaviour {
    // PUBLIC
    public GameObject m_PlayerPrefab;
    // PRIVATE
    //private LevelManager m_LevelManager = null;
    private MenuManager m_MenuManager = null;
    private LevelManager m_LevelManager = null;
    private MatchManager m_MatchManager = null;
    private GameSettings m_Settings = null;
    private List<AgentManager> m_Players = new List<AgentManager>();
    private bool first_menu = true;
    private bool first_frame = true;
    private bool m_MenuRequested = false;

    private GameState m_State;

    void Awake () {
        // SET VARIABLES
        m_Settings = new GameSettings();
        m_Players = new List<AgentManager>();
        m_LevelManager = new LevelManager();
        SetState(GameState.Menu);
    }
	
    public bool Add_Player(AgentManager new_player)
    {
        if (m_Players.Count < 4)
        {
            new_player.Set_Player_Number(Get_First_Player_Number());
            m_Players.Add(new_player);
            return true;
        }
        return false;
    }

    public string Current_Scene()
    {
        return m_LevelManager.Current_Scene();
    }
    public bool Remove_Player(AgentManager toRemove)
    {
        if (m_Players.Contains(toRemove))
        {
            m_Players.Remove(toRemove);
            GameObject.Destroy(toRemove.Get_Object());
            return true;
        }
        else
        {
            return false;
        }
    }

	void Update () {
		switch(m_State)
        {
            case GameState.Menu:
                {
                    m_MenuManager.Update();
                    if (m_MenuManager.m_PlayRequested)
                    {
                        SetState(GameState.Match);
                    }
                    break;
                }
            case GameState.Match:
                {
                    m_MatchManager.Update();
                    if (m_MatchManager.Close || m_MenuRequested)
                    {
                        SetState(GameState.Menu);
                    }
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in GameManager.Update()");
                }
        }
	}

    private void LeaveState()
    {
        // use this function to neatly close down a state.
        switch (m_State)
        {
            case GameState.Menu:
                {
                    m_MenuManager = null;
                    break;
                }
            case GameState.Match:
                {
                    m_MatchManager = null;
                    foreach (AgentManager a in Get_Players())
                    {
                        a.Hide_Name();
                    }
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in GameManager.Leave_State()");
                }
        }
    }
    private void SetState(GameState newState)
    {
        // leave old state
        LeaveState();

        // move to new state
        m_State = newState;

        // Setup new state
        switch (m_State)
        {
            case GameState.Menu:
                {
                    m_MenuRequested = false;
                    Load_Menu();
                    Clear_Players();
                    if (first_menu)
                    {
                        m_MenuManager = new MenuManager(this);
                    }
                    else
                    {
                        m_MenuManager = new MenuManager(this, MenuState.PlayerSelect);
                    }
                    break;
                }
            case GameState.Match:
                {
                    Load_Next();
                    foreach (AgentManager a in Get_Players())
                    {
                        a.Show_Name();
                    }
                    first_menu = false;
                    m_MatchManager = new MatchManager(this);
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in GameManager.SetState()");
                }
        }
    }

    public GameState Get_State()
    {
        return m_State;
    }

    public List<AgentManager> Get_Players()
    {
        return m_Players;
    }

    public MatchManager Get_Match()
    {
        return m_MatchManager;
    }

    public List<GameObject> Get_Players_Objects()
    {
        List<GameObject> obj_list = new List<GameObject>();
        foreach (AgentManager player in m_Players)
        {
            obj_list.Add(player.Get_Object());
        }
        return obj_list;
    }

    public int Player_Count()
    {
        return m_Players.Count;
    }

    public GameSettings Get_Settings()
    {
        return m_Settings;
    }

    public MenuManager Get_MenuManager()
    {
        return m_MenuManager;
    }

    public MatchManager Get_MatchManager()
    {
        return m_MatchManager;
    }

    public void Preload_Next_Level()
    {
        StartCoroutine(m_LevelManager.BeginLoadNextLevel());
    }

    public void Load_Next()
    {
        m_LevelManager.LoadStagedLevel();
    } 

    public void Clear_Players()
    {
        foreach(AgentManager player in m_Players)
        {
            GameObject.Destroy(player.Get_Object());
        }
        m_Players.Clear();
    }

    public void Load_Menu()
    {
        m_LevelManager.LoadMenu();
    }

    public void ReturnToMenu()
    {
        m_MenuRequested = true;
    }
    
    public AgentManager Get_Player(int i)
    {
        foreach(AgentManager p in m_Players)
        {
            if (i == p.Get_Controller_Number())
            {
                return p;
            }
        }
        return null;
    }

    private int Get_First_Player_Number()
    {
        int result = 1;
        foreach(AgentManager a in m_Players)
        {
            if(result == a.Get_Player_Number())
            {
                result++;
            }
            else
            {
                break;
            }
        }
        return result;
    }

    public List<Transform> Get_Player_Objects_Transform()
    {
        List<Transform> result = new List<Transform>();
        foreach (GameObject p in Get_Players_Objects())
        {
            result.Add(p.transform);
        }
        return result;
    }

    public void RumbleAll(float l, float r)
    {
        foreach (AgentManager a in Get_Players())
        {
            a.Get_Input().Controller_Rumble(l, r);
        }
    }
}
