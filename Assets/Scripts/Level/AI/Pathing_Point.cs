using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing_Point : MonoBehaviour{
    public enum NodeState { Untested, Open, Closed };
    public List<Pathing_Point> m_Connections = new List<Pathing_Point>();
    public Transform m_Position;
    public NodeState m_Open;
    public Pathing_Point m_Parent;
    public float F = float.PositiveInfinity;
    public float G = float.PositiveInfinity;
    public float H = float.PositiveInfinity;
    public string the_Name;

    void Start()
    {
        the_Name = gameObject.transform.name;
    } 
    public Pathing_Point Get_Random_Connection()
    {
        return m_Connections[Random.Range(0, m_Connections.Count)];
    }

    public void Setup_For_Search(Pathing_Point target)
    {
        if(target != null)
        {
            H = Vector3.Distance(m_Position.position, target.m_Position.position);
        }
        else
        {
            H = float.PositiveInfinity;
        }
        m_Parent = null;
        m_Open = NodeState.Untested;
    }

    public float Get_F()
    {
        F = G + H;
        return F;
    }
}
