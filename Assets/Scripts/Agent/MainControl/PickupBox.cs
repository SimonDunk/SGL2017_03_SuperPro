using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBox : MonoBehaviour {

    public List<GameObject> items_in_range = new List<GameObject>();
    private Item registered_item = null;
    private AgentManager m_Agent = null;
	// Use this for initialization
	void Start () {
		
	}
	
    public void Add_Agent(AgentManager agent)
    {
        m_Agent = agent;
    }
    void OnTriggerEnter (Collider Other)
    {
        if (Other.CompareTag(GLOBAL_VALUES.PICKUP_ITEM))
        {
            items_in_range.Add(Other.gameObject);
        }
    }

    void OnTriggerExit (Collider Other)
    {
        if (items_in_range.Contains(Other.gameObject))
        {
            items_in_range.Remove(Other.gameObject);
        }
    }
    public GameObject Get_Closest_Object()
    {
        float? nearest_distance = null;
        float distance;
        GameObject ret = null;
        if (items_in_range.Count > 0)
        {
            for (int i = 0; i < items_in_range.Count; i++)
            {
                distance = Vector3.Distance(transform.parent.transform.position, items_in_range[i].transform.position);
                if ((nearest_distance == null || nearest_distance > distance) && (!items_in_range[i].GetComponent<Item>().BeingCarried()))
                {
                    ret = items_in_range[i];
                }
            }
        }
        return ret;
    }
	// Update is called once per frame
	void Update () {
        if (m_Agent != null && m_Agent.isPlayer())
        {
            if (! m_Agent.isCarrying())
            {
                GameObject closest = Get_Closest_Object();
                if (closest != null)
                {
                    // if the closest item isnt being carried than register it
                    if ((registered_item == null || registered_item != closest.GetComponent<Item>()) && (!closest.GetComponent<Item>().BeingCarried()))
                    {
                        if (registered_item != null)
                        {
                            registered_item.Deregister_Agent_Targetting(m_Agent);
                            registered_item = null;
                        }
                        registered_item = closest.GetComponent<Item>();
                        registered_item.Register_Agent_Targetting(m_Agent);
                    }
                }
                else
                {
                    // ensure nothing is registered
                    if (registered_item != null)
                    {
                        registered_item.Deregister_Agent_Targetting(m_Agent);
                        registered_item = null;
                    }
                }
            }
            else
            {
                // ensure nothing is registered
                if (registered_item != null)
                {
                    registered_item.Deregister_Agent_Targetting(m_Agent);
                    registered_item = null;
                }
            }
        }
	}
}
    
