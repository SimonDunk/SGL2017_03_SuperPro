using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Powerup_Type {Shield, Earthquake, Magnet};

public class Powerup : MonoBehaviour {

    public Powerup_Type m_type;

    void OnTriggerEnter(Collider collider)
    {
        /*
        GameObject other = collider.gameObject;
        if (other.CompareTag(GLOBAL_VALUES.TAG_PLAYER))
        {
            CollisionDetection collision = other.gameObject.GetComponent<CollisionDetection>();
            AgentManager m_agent = collision.m_Manager;
            if(! m_agent.has_active_powerup())
            {
                m_agent.Set_Powerup_State(m_type);
                GameObject.Destroy(gameObject);
            }
        }
        */
    }
}
