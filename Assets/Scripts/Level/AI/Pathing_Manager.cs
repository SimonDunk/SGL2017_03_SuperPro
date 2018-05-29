using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing_Manager : MonoBehaviour {

    private List<Pathing_Point> m_Nodes = new List<Pathing_Point>();
    private GameManager m_GameManager;

	// Use this for initialization
	void Start ()
    {
        m_GameManager = GameObject.Find("GameManagerObject").GetComponent<GameManager>();
        Find_Nodes();
        Connect_Nodes();
	}
    
    public AgentManager Get_Nearest_Player(Vector3 start_pos)
    {
        AgentManager ret = null;
        float distance = float.PositiveInfinity;
        foreach (AgentManager player in m_GameManager.Get_Players())
        {
            RaycastHit hit;
            if (
                (Physics.Raycast(start_pos, player.Get_Position() - start_pos, out hit)) &&
                (hit.transform.gameObject == player.Get_Object())
                )
            {
                if (Vector3.Distance(start_pos, player.Get_Position()) < distance)
                {
                    distance = Vector3.Distance(start_pos, player.Get_Position());
                    ret = player;
                }
            }
        }
        return ret;
    }

    private void Turn_Node_Collision_Off(Pathing_Point node)
    {
        node.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    private void Turn_Node_Collision_On(Pathing_Point node)
    {
        node.gameObject.GetComponent<BoxCollider>().enabled = true;
    }
    public Pathing_Point Get_Nearest_Node(Vector3 start_pos)
    {
        Pathing_Point ret = null;
        float distance = float.PositiveInfinity;
        foreach (Pathing_Point node in m_Nodes)
        {
            Turn_Node_Collision_On(node);
            RaycastHit hit;
            if (
                (Physics.Raycast(start_pos, node.m_Position.position - start_pos, out hit)) &&
                (hit.transform.gameObject == node.m_Position.gameObject)
                )
            {
                if (Vector3.Distance(start_pos, node.m_Position.position) < distance)
                {
                    distance = Vector3.Distance(start_pos, node.m_Position.position);
                    ret = node;
                }
            }
            Turn_Node_Collision_Off(node);
        }
        return ret;
    }
    private void Connect_Nodes()
    {
        foreach (Pathing_Point start_node in m_Nodes)
        {
            foreach (Pathing_Point dest_node in m_Nodes)
            {
                if ((start_node != dest_node) &&
                    (!start_node.m_Connections.Contains(dest_node)) &&
                    (!dest_node.m_Connections.Contains(start_node)) &&
                    (Vector3.Distance(start_node.m_Position.position, dest_node.m_Position.position) < 10))
                {
                    // node not already in list, check it
                    RaycastHit hit;
                    if (
                        (Physics.Raycast(start_node.m_Position.position, dest_node.m_Position.position - start_node.m_Position.position, out hit)) &&
                        (hit.transform.gameObject == dest_node.m_Position.gameObject)
                        )
                    {
                        // the raycast hit the destination node
                        // connect them
                        start_node.m_Connections.Add(dest_node);
                        dest_node.m_Connections.Add(start_node);
                        Debug.DrawLine(start_node.m_Position.position, dest_node.m_Position.position, Color.blue, 120.0f, false);
                    }
                }
            }
        }
        // Disable Collision Meshs
        foreach (Pathing_Point node in m_Nodes)
        {
            Turn_Node_Collision_Off(node);
        }
    }

    private void Find_Nodes()
    {
        // Find all nodes
        foreach (GameObject node in GameObject.FindGameObjectsWithTag("Pathing_Node"))
        {
            Pathing_Point point = node.GetComponent<Pathing_Point>();
            point.m_Position = node.transform;
            point.gameObject.GetComponent<MeshRenderer>().enabled = false;
            m_Nodes.Add(point);
        }
    }

    private float Calculate_Cost(Pathing_Point start, Pathing_Point end)
    {
        float ret = 0.0f;
        ret = Vector3.Distance(start.m_Position.position, end.m_Position.position);
        return ret;
    }

    private float Calculate_Heuristic(Pathing_Point start, Pathing_Point end)
    {
        float ret = 0.0f;
        ret = Vector3.Distance(start.m_Position.position, end.m_Position.position);
        return ret;
    }

    private float Calculate_F(Pathing_Point curr, Pathing_Point next, Pathing_Point end_point)
    {
        float ret = 0.0f;
        ret = Calculate_Cost(curr, next);
        ret += Calculate_Heuristic(next, end_point);
        return ret;
    }

    public Path Find_Path(Vector3 start, Vector3 end)
    {
        Path ret = new Path();
        // Get the closest node to the start position
        Pathing_Point first_node = Get_Nearest_Node(start);
        // Get the closest node to the end position
        Pathing_Point end_node = Get_Nearest_Node(end);

        // reset pathing points
        foreach (Pathing_Point node in m_Nodes)
        {
            node.Setup_For_Search(end_node);
        }
        if (first_node != null && end_node != null)
        {
            ret = CompilePath(first_node, end_node);
            return ret;
        }
        else
        { return null; }
    }

    public bool Search(Pathing_Point curr, Pathing_Point target)
    {
        curr.m_Open = Pathing_Point.NodeState.Closed;
        List<Pathing_Point> next_nodes = GetAdjacentWalkableNodes(curr);
        next_nodes.Sort((node1, node2) => node1.Get_F().CompareTo(node2.Get_F()));
        foreach(Pathing_Point node in next_nodes)
        {
            if (node == target)
            {
                return true;
            }
            else
            if (Search(node, target)) // Note: Recurses back into Search(Node)
                return true;
        }
        return false;
    }

    public Path CompilePath(Pathing_Point start, Pathing_Point end)
    {
        Path path = new Path();
        bool success = Search(start, end);
        if (success)
        {
            Pathing_Point node = end;
            while (node.m_Parent != null)
            {
                path.Append(node);
                node = node.m_Parent;
            }
            path.Append(node);
            path.Reverse();
        }
        else if (start == end)
        {
            path.Append(start);
        }
        return path;
    }

    public List<Pathing_Point> GetAdjacentWalkableNodes(Pathing_Point curr)
    {
        List<Pathing_Point> walkableNodes = new List<Pathing_Point>();
        foreach (Pathing_Point node in curr.m_Connections)
        {   // Ignore already-closed nodes
            if (node.m_Open == Pathing_Point.NodeState.Closed)
                continue;

            // Already-open nodes are only added to the list if their G-value is lower going via this route.
            if (node.m_Open == Pathing_Point.NodeState.Open)
            {
                float traversalCost = Calculate_Cost(curr, node);
                float gTemp = curr.G + traversalCost;
                if (gTemp < node.G)
                {
                    node.m_Parent = curr;
                    walkableNodes.Add(node);
                }
            }
            else
            {
                // If it's untested, set the parent and flag it as 'Open' for consideration
                node.m_Parent = curr;
                node.m_Open = Pathing_Point.NodeState.Open;
                node.G = Calculate_Cost(node, node.m_Parent);
                walkableNodes.Add(node);
            }
        }
        return walkableNodes;
    }

    public Pathing_Point Get_Random_Node(Pathing_Point current)
    {
        Pathing_Point ret = current;
        while (ret == current)
        {
            ret = m_Nodes[Random.Range(0, m_Nodes.Count)];
        }
        return ret;
    }
}
