using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityItemVision : MonoBehaviour {
    private List<GameObject> m_Objects = new List<GameObject>();
    private List<GameObject> m_Dangers = new List<GameObject>();
    public AgentManager m_Manager = null;

    private void Update()
    {
        foreach(GameObject obj in m_Objects)
        {
            if(!m_Dangers.Contains(obj))
            {
                if ((Custom_Math_Utils.Calculate_Kinetic_Energy(obj) >= GLOBAL_VALUES.SECURITY_ITEM_SPEED_THRESHOLD) && (obj.GetComponent<Item>().prev_owner != m_Manager))
                {
                    m_Dangers.Add(obj);
                }
            }
        }
        List<GameObject> to_delete = new List<GameObject>(m_Dangers);
        foreach (GameObject obj in to_delete)
        {
            Item l_item = obj.GetComponent<Item>();
            if(l_item.BeingCarried())
            {
                m_Dangers.Remove(obj);
            }
        }
    }
    public GameObject Get_Best_Item()
    {
        // Get object with highest damage
        GameObject ret = null;
        float force = float.PositiveInfinity;
        foreach(GameObject obj in m_Dangers)
        {
            float item_force = Custom_Math_Utils.Calculate_Kinetic_Energy(obj);
            if (item_force < force)
            {
                ret = obj;
                force = item_force;
            }
        }
        return ret;
    }

    private void OnTriggerEnter(Collider collision)
    {
        GameObject Col_Obj = collision.gameObject;
        if ((Col_Obj.CompareTag(GLOBAL_VALUES.PICKUP_ITEM)) && (! m_Objects.Contains(Col_Obj)))
        {
            // register item
            m_Objects.Add(Col_Obj);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        GameObject Col_Obj = collision.gameObject;
        if (m_Objects.Contains(Col_Obj))
        {
            // remove item
            m_Objects.Remove(Col_Obj);
        }
        if (m_Dangers.Contains(Col_Obj))
        {
            // remove item
            m_Dangers.Remove(Col_Obj);
        }
    }

    public bool NeedToCatch()
    {
        return m_Dangers.Count > 0;
    }
}
