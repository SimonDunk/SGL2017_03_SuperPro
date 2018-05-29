using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenManager {
    //PUBLIC
    public bool Finished = false; // and screen has finished its production
    //PRIVATE
    private GameManager m_GameManager = null;
    private MatchManager m_MatchManager = null;
    public EndScreenManager(MatchManager mm, GameManager gm)
    {
        m_MatchManager = mm;
        m_GameManager = gm;
    }
	public void Update () {
        // end screen does things...
        // for now just close
        Finished = true;
	}
}
