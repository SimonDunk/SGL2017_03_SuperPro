using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraForce
{
    private Vector3 vector;
    private Timer time;

	// Use this for initialization
	public CameraForce (Vector3 f, float t)
    {
        vector = f;
        time = new Timer(t, true);
	}
	
	// Update is called once per frame
	public void Update ()
    {
        time.Update();
	}

    public bool isComplete()
    {
        return time.isComplete();
    }

    public Vector3 Get_Vector()
    {
        return vector;
    }
}
