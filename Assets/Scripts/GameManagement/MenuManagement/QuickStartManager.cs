using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickStartManager
{
    private GameManager m_Manager;
    private List<int> readied = new List<int>();
    private ControlSet m_Controllers;
    private Menu_Heads m_Heads_Holder;

    public bool Play = false;

	// Use this for initialization
	public QuickStartManager (GameManager gm, ControlSet i)
    {
        m_Manager = gm;
        m_Controllers = i;
        m_Heads_Holder = GameObject.Find("HeadsHolder").GetComponent<Menu_Heads>();
        DisableUnusedHeads();
    }
	
	// Update is called once per frame
	public void Update ()
    {
        Check_A_Press();
        if(readied.Count == m_Manager.Player_Count())
        {
            Play = true;
        }
    }

    private void Check_A_Press()
    {
        foreach (int? num in m_Controllers.A_Pressed_List())
        {
            int control_num = num ?? default(int);
            bool player_found = false;
            // do a actions for each person who hit A
            foreach (int i in readied)
            {
                // check if it has a player already
                if (i == control_num)
                {
                    player_found = true;
                    break;
                }
            }
            // if no player already exists, add it
            if (!player_found)
            {
                //set a head to player colour
                AgentManager a = m_Manager.Get_Player((int)num);
                m_Heads_Holder.Heads[(int)a.Get_Player_Number()-1].sprite = m_Heads_Holder.ReadySprites[a.Get_Color()];
                readied.Add((int)num);
            }
        }
    }

    void DisableUnusedHeads()
    {
        foreach(AgentManager a in m_Manager.Get_Players())
        {
            a.Get_Object().transform.position = new Vector3(0, -20, 0);
            m_Heads_Holder.Heads[(int)a.Get_Player_Number()-1].sprite = m_Heads_Holder.UnreadySprites[a.Get_Color()];
        }
    }
}
