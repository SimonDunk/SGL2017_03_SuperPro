using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIndicator
{
    private GameObject m_UI_Element;
    private UI_HEAD_HOLDER m_Head_Holder;
    private GameObject m_Head;
    private Text m_Text;
    private Text m_PlayerName;
    private Animator m_Animator;
    private int m_State = 0; // Idle
    private AgentManager m_Player;
    private bool idle = true;

    public ScoreIndicator(GameObject element, AgentManager player)
    {
        m_UI_Element = element;
        m_Head_Holder = m_UI_Element.GetComponent<UI_HEAD_HOLDER>();
        m_Head = m_UI_Element.transform.Find("PlayerHead").gameObject;
        m_Text = m_UI_Element.transform.Find("ScoreText").GetComponent<Text>();
        m_PlayerName = m_UI_Element.transform.Find("PlayerName").GetComponent<Text>();
        m_Animator = m_UI_Element.GetComponent<Animator>();
        Change_Player_At_Position(player);
    }
    
    public void Change_Player_At_Position(AgentManager new_player)
    {
        m_Player = new_player;
        m_State = 1; // Going Out
    }

    public void Turn_On()
    {
        m_UI_Element.SetActive(true);
    }
    public void Turn_Off()
    {
        m_UI_Element.SetActive(false);
    }

    public void Update()
    {
        switch(m_State)
        {
            case (0): // Idle
                {
                    if ((!idle) && ((m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ScoreIn"))))
                    {
                        m_Animator.ResetTrigger("GoOut");
                        m_Animator.ResetTrigger("GoIn");
                        idle = true;
                    }
                    m_Text.text = "$" + m_Player.Get_Score();
                    break;
                }
            case (1): // Going Out
                {
                    idle = false;
                    m_Animator.SetTrigger("GoOut");
                    m_State = 2;
                    break;
                }
            case (2): // Changing Face
                {
                    if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ScoreOut"))
                    {
                        // Animation is done
                        m_Head.GetComponent<Image>().sprite = m_Head_Holder.Heads[m_Player.Get_Color()];
                        m_Text.text = "$" + m_Player.Get_Score();
                        m_PlayerName.text = m_Player.Get_Name();
                        m_State = 3;
                    }
                    break;
                }
            case (3): // Going In
                {
                    m_Animator.SetTrigger("GoIn");
                    m_State = 0;
                    break;
                }
        }
    }
}

public class ScoreUIManager : MonoBehaviour {
    
    MatchManager m_MatchManager;
    List<AgentManager> m_Players = new List<AgentManager>();
    List<int> m_Positions = new List<int>();
    List<ScoreIndicator> m_UI = new List<ScoreIndicator>();
    GameObject LeaderBanner_Top = null;
    GameObject LeaderBanner_Bottom = null;
    GameObject LeaderBanner_Background_Drop = null;
    GameObject LeaderBanner_BGOutline = null;
    Text LeaderBanner_LeaderName = null;
    bool m_First_Score = true;
    bool m_First_Score_Effect = false;
    //GameObject newLeaderBanner = null;

    public void Init()
    {

        Transform ScoreIndicators = GameObject.Find("HUD").transform.Find("ScoreIndicators");
        LeaderBanner_Top = GameObject.Find("HUD").transform.Find("LeaderObject").Find("LeaderPanel").Find("LeaderBackground").gameObject;
        LeaderBanner_Bottom = GameObject.Find("HUD").transform.Find("LeaderObject").Find("LeaderPanel").Find("LeaderBottomBar").gameObject;
        LeaderBanner_Background_Drop = GameObject.Find("HUD").transform.Find("LeaderObject").Find("LeaderPanel").Find("LeaderBackgroundDrop").gameObject;
        LeaderBanner_BGOutline = GameObject.Find("HUD").transform.Find("LeaderObject").Find("LeaderPanel").Find("LeaderBGOutline").gameObject;
        LeaderBanner_LeaderName = LeaderBanner_Top.transform.Find("LeaderTextDrop").GetComponent<Text>();
        //newLeaderBanner = GameObject.Find("HUD").transform.Find("ScoreIndicators").Find("NewLeader").gameObject;
        m_Players = GameObject.Find("GameManagerObject").GetComponent<GameManager>().Get_Players();
        for (int i = 0; i < m_Players.Count; i++) 
        {
            m_Positions.Add(i + 1);
        }
        
        m_UI.Add(new ScoreIndicator(ScoreIndicators.Find("GoldScoreBorder").gameObject, m_Players[0]));
        if (m_Positions.Count > 1)
        {
            m_UI.Add(new ScoreIndicator(ScoreIndicators.Find("Silver Score Border").gameObject, m_Players[1]));
        }
        if (m_Positions.Count > 2)
        {
            m_UI.Add(new ScoreIndicator(ScoreIndicators.Find("BronzeScoreBorder1").gameObject, m_Players[2]));
        }
        if (m_Positions.Count > 3)
        {
            m_UI.Add(new ScoreIndicator(ScoreIndicators.Find("BronzeScoreBorder2").gameObject, m_Players[3]));
        }
        // turn them all off (incase they arent used)
        ScoreIndicators.Find("GoldScoreBorder").gameObject.SetActive(false);
        ScoreIndicators.Find("Silver Score Border").gameObject.SetActive(false);
        ScoreIndicators.Find("BronzeScoreBorder1").gameObject.SetActive(false);
        ScoreIndicators.Find("BronzeScoreBorder2").gameObject.SetActive(false);
        foreach (ScoreIndicator ind in m_UI)
        {
            ind.Turn_On();
        }
    }
	
	void Update ()
    {
        Check_Positions();
        foreach(ScoreIndicator si in m_UI)
        {
            si.Update();
        }
    }

    private void Check_Positions()
    {
        List<int> new_positions = new List<int>();
        for (int i = 0; i < m_Players.Count; i++)
        {
            int pos = Get_Player_Position(m_Players[i]);
            m_Players[i].Set_Score_Position(pos);
            new_positions.Add(pos);
            if (m_First_Score)
            {
                if (m_Players[i].Get_Score() > 0)
                {
                    m_First_Score_Effect = true;
                    m_First_Score = false;
                }
            }
        }
        for (int i = 0; i < new_positions.Count; i++)
        {
            if (new_positions[i] != m_Positions[i] || m_First_Score_Effect)
            {
                m_UI[new_positions[i] - 1].Change_Player_At_Position(m_Players[i]);
                if (new_positions[i] == 1)
                {
                    LeaderBanner_Background_Drop.GetComponent<Animator>().SetTrigger("Activate");
                    LeaderBanner_Background_Drop.GetComponent<Image>().color = GLOBAL_VALUES.COLOR_NUMBERS[m_Players[i].Get_Color()];
                    LeaderBanner_Background_Drop.transform.Find("LeaderPlayerHeadDrop").GetComponent<Image>().sprite = LeaderBanner_Top.GetComponent<UI_HEAD_HOLDER>().Heads[m_Players[i].Get_Color()];
                    LeaderBanner_Background_Drop.transform.Find("LeaderTextDrop").GetComponent<Text>().text = m_Players[i].Get_Name();
                    LeaderBanner_Top.transform.Find("LeaderPlayerHeadDrop").GetComponent<Image>().sprite = LeaderBanner_Top.GetComponent<UI_HEAD_HOLDER>().Heads[m_Players[i].Get_Color()];
                    LeaderBanner_Top.transform.Find("LeaderTextDrop").GetComponent<Text>().text = m_Players[i].Get_Name();
                    LeaderBanner_Bottom.transform.Find("TakesTheLead").GetComponent<Animator>().SetTrigger("Activate");
                    LeaderBanner_Bottom.GetComponent<Image>().color = GLOBAL_VALUES.COLOR_NUMBERS[m_Players[i].Get_Color()];
                    LeaderBanner_LeaderName.text = m_Players[i].Get_Name();
                }
            }
            if (m_First_Score_Effect)
            {
                m_First_Score_Effect = false;
            }
        }
        m_Positions = new_positions;
    }

    private int Get_Player_Position(AgentManager player_agent)
    {
        List<AgentManager> temp_list = new List<AgentManager>(m_Players);
        temp_list.Sort(delegate (AgentManager p1, AgentManager p2) { return p1.Get_Score().CompareTo(p2.Get_Score()); });
        temp_list.Reverse();
        return temp_list.IndexOf(player_agent) + 1;
    }
}
