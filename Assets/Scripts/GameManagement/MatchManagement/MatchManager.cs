using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum MatchState { StartingMatch, RoundPlaying, RoundFinished, MatchOver };
public class MatchManager
{
    //PUBLIC
    public bool Close;
    //PRIVATE
    private MatchState m_State;
    private RoundManager m_RoundManager = null; // For managing each individual round
    private GameManager m_GameManager = null;
    private EndScreenManager m_EndScreenManager = null; // For Showing who won the match!

    public MatchManager(GameManager game_manager)
    {
        m_State = MatchState.StartingMatch;
        m_GameManager = game_manager;
    }

    // Update is called once per frame
    public void Update()
    {
        switch (m_State)
        {
            case MatchState.StartingMatch:
                {
                    SetState(MatchState.RoundPlaying);
                    break;
                }
            case MatchState.RoundPlaying:
                {
                    if (m_RoundManager.Finished)
                    {
                        SetState(MatchState.RoundFinished);
                    }
                    else
                    {
                        m_RoundManager.Update();
                    }
                    break;
                }
            case MatchState.RoundFinished:
                {
                    // nothing here yet
                    break;
                }
            case MatchState.MatchOver:
                {
                    m_EndScreenManager.Update();
                    if (m_EndScreenManager.Finished)
                    {
                        Close = true;
                        m_GameManager.Load_Menu();
                    }
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in MatchManager.Update()");
                }
        }
    }

    private void LeaveState()
    {
        // use this to neatly clean up a state when leaving it
        switch (m_State)
        {
            case MatchState.StartingMatch:
                {
                    break;
                }
            case MatchState.RoundPlaying:
                {
                    break;
                }
            case MatchState.RoundFinished:
                {
                    break;
                }
            case MatchState.MatchOver:
                {
                    m_EndScreenManager = null;
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in MatchManager.LeaveState()");
                }
        }
    }

    private void SetState(MatchState newState)
    {
        // leave current state
        LeaveState();

        // change to new state
        m_State = newState;

        // set up new state
        switch (m_State)
        {
            case MatchState.StartingMatch:
                {
                    m_GameManager.Preload_Next_Level();
                    foreach (AgentManager player in m_GameManager.Get_Players())
                    {
                        player.Zero_Score();
                        player.Zero_Rounds_Won();
                    }
                    break;
                }
            case MatchState.RoundPlaying:
                {
                    // Create the RoundManager
                    m_RoundManager = new RoundManager(this, m_GameManager);
                    break;
                }
            case MatchState.RoundFinished:
                {
                    //save the winner of the round
                    if (MatchWon())
                    {
                        SetState(MatchState.MatchOver);
                    }
                    else
                    {
                        SetState(MatchState.RoundPlaying);
                    }
                    break;
                }
            case MatchState.MatchOver:
                {
                    m_EndScreenManager = new EndScreenManager(this, m_GameManager);
                    break;
                }
            default:
                {
                    // THROW ERROR!
                    throw new System.NotImplementedException("State missing in MatchManager.SetState()");
                }
        }
    }


    /*********************
     * ALL OTHER FUNCTIONS
     *********************/
    private SpawnZone Get_Random_Spawn_Point()
    {
        // Get List of Spawn Zones
        GameObject[] Spawn_Zones = GameObject.FindGameObjectsWithTag(GLOBAL_VALUES.PLAYER_SPAWN);
        // Get a random one
        return Spawn_Zones[UnityEngine.Random.Range(0, Spawn_Zones.Length)].GetComponent<SpawnZone>();
    }

    private bool MatchWon()
    {
        foreach (AgentManager player in m_GameManager.Get_Players())
        {
            if (player.Rounds_Won() >= m_GameManager.Get_Settings().RoundsToWin)
            {
                // game won
                return true;
            }
        }
        return false;
    }
    public RoundManager Get_RoundManager()
    {
        return m_RoundManager;
    }
}
