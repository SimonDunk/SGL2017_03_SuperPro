using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {

    private List<AgentManager> m_AllAI = new List<AgentManager>();
    private List<AgentManager> m_BaseAI = new List<AgentManager>();
    private List<AgentManager> m_LargeAI = new List<AgentManager>();
    private List<AgentManager> m_SecurityAI = new List<AgentManager>();
    public int m_NumberOfBaseAI = 5;
    public GameObject m_BaseAI_Prefab;
    public int m_NumberOfLargeAI = 2;
    public GameObject m_LargeAI_Prefab;
    public int m_NumberOfSecurityAI = 1;
    public GameObject m_SecurityAI_Prefab;
    private GameManager m_GameManager;

    public void Pause_AI()
    {
        foreach(AgentManager agent in m_AllAI)
        {
            agent.Pause();
        }
    }

    public void Resume_AI()
    {
        foreach (AgentManager agent in m_AllAI)
        {
            agent.Resume();
        }
    }
    // Use this for initialization
    void Start ()
    {
        m_GameManager = GameObject.Find("GameManagerObject").GetComponent<GameManager>();
        if(m_GameManager != null)
        {
            if(m_GameManager.Get_Settings().ShopperEnabled)
            {
                Load_Base_AI();
                Load_Large_AI();
            }
            if (m_GameManager.Get_Settings().SecurityEnabled)
            {
                Load_Security_AI();
            }
        }
        else { Debug.Log("Game Manager not found for AI..."); }
    }
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < m_AllAI.Count; i++)
        {
            m_AllAI[i].Update();
        }
	}

    private void Load_Base_AI()
    {
        for (int i = 0; i < m_NumberOfBaseAI; i++)
        {
            SpawnZone Spawner = Get_Random_Spawn_Point();
            AgentManager newAI = new AgentManager();
            newAI.Set_Spawn_Point(Spawner.gameObject);
            //create players
            newAI.Set_Instance(Instantiate(m_BaseAI_Prefab, Spawner.Get_Unique_Random_Spawn_Point(), newAI.Get_Spawn_Point().transform.rotation) as GameObject);
            newAI.Initialise_Agent(Agent_Type.Base_AI, 0);
            newAI.Enable();
            m_BaseAI.Add(newAI);
            m_AllAI.Add(newAI);
        }
    }

    private void Load_Large_AI()
    {
        for (int i = 0; i < m_NumberOfLargeAI; i++)
        {
            SpawnZone Spawner = Get_Random_Spawn_Point();
            AgentManager newAI = new AgentManager();
            newAI.Set_Spawn_Point(Spawner.gameObject);
            //create players
            newAI.Set_Instance(Instantiate(m_LargeAI_Prefab, Spawner.Get_Unique_Random_Spawn_Point(), newAI.Get_Spawn_Point().transform.rotation) as GameObject);
            newAI.Initialise_Agent(Agent_Type.Large_AI, 0);
            newAI.Enable();
            m_LargeAI.Add(newAI);
            m_AllAI.Add(newAI);
        }
    }

    private void Load_Security_AI()
    {
        for (int i = 0; i < m_NumberOfSecurityAI; i++)
        {
            SpawnZone Spawner = Get_Random_Spawn_Point();
            AgentManager newAI = new AgentManager();
            newAI.Set_Spawn_Point(Spawner.gameObject);
            //create players
            newAI.Set_Instance(Instantiate(m_SecurityAI_Prefab, Spawner.Get_Unique_Random_Spawn_Point(), newAI.Get_Spawn_Point().transform.rotation) as GameObject);
            newAI.Initialise_Agent(Agent_Type.Security_AI, 0);
            newAI.Enable();
            m_SecurityAI.Add(newAI);
            m_AllAI.Add(newAI);
        }
    }

    private SpawnZone Get_Random_Spawn_Point()
    {
        // Get List of Spawn Zones
        GameObject[] Spawn_Zones = GameObject.FindGameObjectsWithTag(GLOBAL_VALUES.PLAYER_SPAWN);
        // Get a random one
        return Spawn_Zones[UnityEngine.Random.Range(0, Spawn_Zones.Length)].GetComponent<SpawnZone>();
    }
}
