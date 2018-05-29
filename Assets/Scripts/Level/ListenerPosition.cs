using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListenerPosition : MonoBehaviour {
    private float x;
    private float z;
    private float xPos;
    private float zPos;
    private float yPos;
    private List<AgentManager> _players;
    private Transform pos;

    // Use this for initialization
    void Start () {
        //Gets List of all Players
        _players = GameObject.Find("GameManagerObject").GetComponent<GameManager>().Get_Players();
        
        //The Transform of this Object 
        pos = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(_players.Count > 1)
        {
            //Reset Variables
            Vector3 total_dist = Vector3.zero;

            foreach (AgentManager p in _players)
            {
                //Total of x and z position of all players
                total_dist += p.Get_Position();
            }
            //Gets average of players x and z positions
            Vector3 avg = total_dist / _players.Count;

            //Sets new transform position
            pos.position = avg;

            float furthest = float.NegativeInfinity;
            foreach (AgentManager first in _players)
            {
                foreach (AgentManager second in _players)
                {
                    if (first != second)
                    {
                        float dist = Vector3.Distance(first.Get_Position(), second.Get_Position());
                        if (dist > furthest)
                        {
                            furthest = dist;
                        }
                    }
                }
            }

            pos.position = new Vector3(pos.position.x, furthest / 2, pos.position.z);
        }
        else
        {
            pos.position = _players[0].Get_Position();
        }       
	}
}
