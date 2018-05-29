using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatmap_Area : MonoBehaviour {
    public string m_AreaName = "NO_NAME";
    // Private members
    private AgentSubscriber m_Subscriber = null;
    private List<GameObject> m_CurrentOccupants = new List<GameObject>();
    // Stat Counters
    private float m_TotalTimeSpent = 0;
    private int m_TotalEntries = 0;
    private int m_TotalThrows = 0;
    private int m_TotalStuns = 0;
    private int m_TotalPunches = 0;
    private int m_TotalDashs = 0;
    private int m_TotalScores = 0;
    private int m_TotalGroundSlams = 0;

    public string GetHeaderString()
    {
        // Return a header for the csv file
        return "BLOCK NAME, TIME, ENTRIES, THROWS, STUNS, DASHS, PUNCHES, SCORES, POSITION_X, POSITION_Y, SIZE_X, SIZE_Y, GROUND_SLAMS";
    }
    public void Reset()
    {
        // unsub from all agents
        foreach(GameObject player in m_CurrentOccupants)
        {
            player.GetComponent<CollisionDetection>().m_Manager.Unsubscribe(m_Subscriber);
        }
        // Clear list
        m_CurrentOccupants.Clear();
        // Clear all data
        m_TotalTimeSpent = 0;
        m_TotalEntries = 0;
        m_TotalThrows = 0;
        m_TotalStuns = 0;
        m_TotalPunches = 0;
        m_TotalDashs = 0;
        m_TotalScores = 0;
        m_TotalGroundSlams = 0;
}
    public string GetDataString()
    {
        // Return a data line for the csv file
        // the z coordinate is used for the saved y coordinate since its a 3D > 2D conversion
        return m_AreaName + "," +
            m_TotalTimeSpent + "," + 
            m_TotalEntries + "," + 
            m_TotalThrows + "," + 
            m_TotalStuns + "," +
            m_TotalDashs + "," + 
            m_TotalPunches + "," + 
            m_TotalScores + "," + 
            gameObject.transform.position.x + "," + 
            gameObject.transform.position.z + "," +
            gameObject.transform.localScale.x + "," + 
            gameObject.transform.localScale.z + "," +
            m_TotalGroundSlams;
    }
    void Awake()
    {
        // Create the subscriber to listen for agent actions
        m_Subscriber = new AgentSubscriber();
    }
    void Update()
    {
        // Update the total time spent in the area, and handle any new notifications
        m_TotalTimeSpent += Time.deltaTime * m_CurrentOccupants.Count;
        HandleNotifications();
    }
    void OnTriggerEnter(Collider Other)
    {
        // If its a player, 
        // - save it to the current occupants list
        // - increment the total entries
        // - Subscribe to notifications
        if (Other.gameObject.CompareTag(GLOBAL_VALUES.TAG_PLAYER))
        {
            Other.gameObject.GetComponent<CollisionDetection>().m_Manager.Subscribe(m_Subscriber);
            m_CurrentOccupants.Add(Other.gameObject);
            m_TotalEntries++;
        }
    }

    void OnTriggerExit(Collider Other)
    {
        // If its a player,
        // - Remove from current occupants list
        // - Unsubscribe from notifications
        if (Other.gameObject.CompareTag(GLOBAL_VALUES.TAG_PLAYER))
        {
            Other.gameObject.GetComponent<CollisionDetection>().m_Manager.Unsubscribe(m_Subscriber);
            m_CurrentOccupants.Remove(Other.gameObject);
        }
    }

    private void HandleNotifications()
    {
        // while there are still new notificaitons
        // check what they are and increment the appropriate counter
        while(m_Subscriber.hasNotifications())
        {
            switch(m_Subscriber.GetNextNotification())
            {
                case AgentAction.Null:
                    {
                        // do nothing
                        break;
                    }
                case AgentAction.Dash:
                    {
                        // count a dash
                        m_TotalDashs++;
                        break;
                    }
                case AgentAction.GetStunned:
                    {
                        // count a stun
                        m_TotalStuns++;
                        break;
                    }
                case AgentAction.Punch:
                    {
                        // count a punch
                        m_TotalPunches++;
                        break;
                    }
                case AgentAction.Score:
                    {
                        // count a score
                        m_TotalScores++;
                        break;
                    }
                case AgentAction.Throw:
                    {
                        // count a throw
                        m_TotalThrows++;
                        break;
                    }
                case AgentAction.GroundSlam:
                    {
                        // count a groundslam
                        m_TotalGroundSlams++;
                        break;
                    }
                default:
                    {
                        // do nothing
                        break;
                    }
            }
        }
    }
}
