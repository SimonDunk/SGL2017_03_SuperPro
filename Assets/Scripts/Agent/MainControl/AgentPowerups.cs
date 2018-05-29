using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPowerups {

    public enum PowerupState { None, Shield, Earthquake, Magnet}

    private AgentManager m_Agent;
    private PowerupState m_state;
    private Timer m_PowerupTimer = new Timer();
    private GameManager m_GameManager = null;

    // Shield
    private GameObject m_Shield;

    // Earthquake
    CameraManager m_CameraManager = null;
    // Magnet

	// Use this for initialization
	public AgentPowerups(AgentManager manager)
    {
        m_GameManager = GameObject.Find("GameManagerObject").GetComponent<GameManager>();
        m_Agent = manager;
        m_state = PowerupState.None;
        m_Shield = m_Agent.Get_Object().transform.Find("Shield_Prefab").gameObject;
        m_Shield.SetActive(false);
    }
	public void Setup_Camera_Manager(CameraManager new_manager)
    {
        m_CameraManager = new_manager;
    }
    public void Pause()
    {
        m_PowerupTimer.Pause();
    }
    public void Resume()
    {
        m_PowerupTimer.Start();
    }
    // Update is called once per frame
    public void Update ()
    {
        m_PowerupTimer.Update();
        if (m_PowerupTimer.isComplete())
        {
            Set_State(PowerupState.None);
        }
        switch(m_state)
        {
            case PowerupState.Magnet:
                {
                    foreach(Item i in m_GameManager.Get_MatchManager().Get_RoundManager().Get_ItemManager().m_Items)
                    {
                        if (! i.BeingCarried())
                        {
                            i.itemBody.AddForce(Custom_Math_Utils.FindTargetAngle(m_Agent.Get_Position(), i.transform.position) * GLOBAL_VALUES.POWERUP_MAGNET_STRENGTH);
                        }
                    }
                    break;
                }
            case PowerupState.Earthquake:
                {
                    // lights flicker?
                    break;
                }
            case PowerupState.Shield:
                {
                    // check the shield strength
                    break;
                }
            default:
                {
                    break;
                }
        }
	}

    public PowerupState Get_State()
    {
        return m_state;
    }
    public bool isShielded()
    {
        return m_state == PowerupState.Shield;
    }

    private void Set_State(PowerupState state)
    {
        Leave_State();
        m_state = state;
        switch (m_state)
        {
            case PowerupState.None:
                {
                    break;
                }
            case PowerupState.Shield:
                {
                    m_PowerupTimer.Reset();
                    m_PowerupTimer.Add(GLOBAL_VALUES.POWERUP_SHIELD_TIME, true);
                    m_Shield.SetActive(true);
                    // turn on shield object
                    break;
                }
            case PowerupState.Magnet:
                {
                    m_PowerupTimer.Reset();
                    m_PowerupTimer.Add(GLOBAL_VALUES.POWERUP_MAGNET_TIME, true);
                    break;
                }
            case PowerupState.Earthquake:
                {
                    m_PowerupTimer.Reset();
                    m_PowerupTimer.Add(GLOBAL_VALUES.POWERUP_QUAKE_STUN_TIME, true);
                    m_CameraManager.SetScreenShake(GLOBAL_VALUES.POWERUP_QUAKE_CAMERASHAKE_TIME, GLOBAL_VALUES.POWERUP_QUAKE_CAMERASHAKE_INTENSITY);
                    foreach (AgentManager agent in m_GameManager.Get_Players())
                    {
                        if (agent != m_Agent)
                        {
                            agent.Stun_Agent(GLOBAL_VALUES.POWERUP_QUAKE_STUN_TIME);
                        }
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void Leave_State()
    {
        switch (m_state)
        {
            case PowerupState.None:
                {
                    break;
                }
            case PowerupState.Shield:
                {
                    m_Shield.SetActive(false);
                    // remove shield object
                    break;
                }
            case PowerupState.Earthquake:
                {
                    // make sure lights are fixed
                    break;
                }
            case PowerupState.Magnet:
                {
                    // stop magnet effect
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    public void Activate(Powerup_Type type)
    {       
        switch(type)
        {
            case Powerup_Type.Shield:
                {
                    Set_State(PowerupState.Shield);
                    break;
                }
            case Powerup_Type.Magnet:
                {
                    Set_State(PowerupState.Magnet);
                    break;
                }
            case Powerup_Type.Earthquake:
                {
                    Set_State(PowerupState.Earthquake);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
