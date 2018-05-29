using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_And_Agent_Tracker : MonoBehaviour {

    public List<GameObject> m_Items = new List<GameObject>();
    public List<GameObject> m_Agents = new List<GameObject>();
    public List<GameObject> m_Desctructables = new List<GameObject>();

    void OnTriggerEnter(Collider Other)
    {
        if (Other.CompareTag(GLOBAL_VALUES.PICKUP_ITEM))
        {
            m_Items.Add(Other.gameObject);
        }
        else if (Other.CompareTag(GLOBAL_VALUES.TAG_PLAYER) || 
            Other.CompareTag(GLOBAL_VALUES.TAG_AI_BASIC) || 
            Other.CompareTag(GLOBAL_VALUES.TAG_AI_LARGE) ||
            Other.CompareTag(GLOBAL_VALUES.TAG_AI_SECURITY))
        {
            m_Agents.Add(Other.gameObject);
        }
        
        else if (Other.CompareTag(GLOBAL_VALUES.TAG_DESTRUCTABLE_PARENT))
        {
            m_Desctructables.Add(Other.gameObject);
        }
    }

    void OnTriggerExit(Collider Other)
    {
        if (m_Items.Contains(Other.gameObject))
        {
            m_Items.Remove(Other.gameObject);
        }
        else if(m_Agents.Contains(Other.gameObject))
        {
            m_Agents.Remove(Other.gameObject);
        }
        else if (m_Desctructables.Contains(Other.gameObject))
        {
            m_Desctructables.Remove(Other.gameObject);
        }
    }
}
