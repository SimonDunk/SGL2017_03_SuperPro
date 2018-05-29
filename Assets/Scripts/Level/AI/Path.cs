using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {

    public int num_points;
    private int cur_point_index;
    private bool finished;
    private List<Pathing_Point> path = new List<Pathing_Point>();
	// Update is called once per frame
	public Path() {
        cur_point_index = 0;
        finished = false;
	}

    public Pathing_Point Current_Point()
    {
        if (finished || path.Count == 0)
        {
            return null;
        }
        else if (cur_point_index < path.Count)
        {
            return path[cur_point_index];
        }
        return null;
    }

    public Pathing_Point Increment_Point()
    {
        if(!finished)
        {
            cur_point_index += 1;
        }
        if(cur_point_index == path.Count)
        {
            finished = true;
            return null;
        }
        else
        {
            return Current_Point();
        }
    }

    public void Append(Pathing_Point point)
    {
        path.Add(point);
        num_points += 1;
    }

    public List<Pathing_Point> Get_Path()
    {
        return path;
    }

    public bool isFinished()
    {
        return finished;
    }

    public void Reverse()
    {
        path.Reverse();
    }
}
