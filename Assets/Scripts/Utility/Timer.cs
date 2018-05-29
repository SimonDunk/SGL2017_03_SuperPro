using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer{

    private float m_Timer;
    private bool m_Running;
	// Use this for initialization
	public Timer(float init_time = 0, bool autostart = false) {
        m_Timer = init_time;
        m_Running = autostart;
	}
	
	// Update is called once per frame
	public void Update() {
        // Update the timer, ancor at 0
        m_Timer = m_Running ? m_Timer - Time.deltaTime : m_Timer;
        m_Timer = m_Timer < 0 ? 0 : m_Timer;
	}

    public void Add(float seconds, bool start = false)
    {
        // Add a number of seconds to the timer and whether to start it 
        // or leave it in the current state
        m_Timer += seconds;
        m_Running = start ? true : m_Running;
    }

    public bool isComplete()
    {
        // return whether the timer is complete or not
        return m_Timer <= 0;
    }

    public void Pause()
    {
        // pause the timer
        m_Running = false;
    }

    public void Reset()
    {
        // Set timer to 0 and stop it
        m_Timer = 0;
        m_Running = false;
    }

    public void Start()
    {
        // start the timer
        m_Running = true;
    }

    public float Get_Time()
    {
        return m_Timer;
    }

    public int Get_Seconds()
    {
        return (int)m_Timer;
    }
}
