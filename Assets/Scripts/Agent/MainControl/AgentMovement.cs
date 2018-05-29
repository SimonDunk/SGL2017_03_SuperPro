using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ExternalForce
{
    public Vector3 m_Force;
    public Timer m_Time;

    public ExternalForce(float time, Vector3 force)
    {
        m_Time = new Timer(time, true);
        m_Force = force;
    }

    public void Update_Time()
    {
        m_Time.Update();
    }
}

public class AgentMovement {

    public AgentManager m_manager;
    public Input_Methods m_input;
    public Rigidbody m_body;
    public float m_speed;
    public List<ExternalForce> m_Forces = new List<ExternalForce>();

    public void Add_External_Force(ExternalForce force)
    {
        m_Forces.Add(force);
    }

    public AgentMovement(AgentManager manager, Input_Methods input)
    {
        m_manager = manager;
        m_input = input;
        m_body = m_manager.Get_Body();
    }

    public void Update()
    {
        // Update velocity list timers and remove them if complete
        List<ExternalForce> toRemove = new List<ExternalForce>();
        foreach(ExternalForce f in m_Forces)
        {
            f.Update_Time();
            if (f.m_Time.isComplete())
            {
                toRemove.Add(f);
            }
        }
        foreach(ExternalForce f in toRemove)
        {
            m_Forces.Remove(f);
        }
    }

    public void Move()
    {
        if (!m_manager.isStunned())
        {
            Do_Rotation();
        }
        Do_Translation();
    }

    void Do_Translation()
    {
        // calculate the velocity for the agent and apply it to the rigidbody
        float movement_speed = m_manager.Get_Move_Speed();
        float mag = m_input.Get_Requested_Magnitude();
        m_speed = movement_speed * mag;
        Vector3 baseVelocity = (! m_manager.isStunned()) ? (m_body.transform.forward * m_speed) : Vector3.zero;
        baseVelocity = m_manager.isDashing() ? baseVelocity * GLOBAL_VALUES.DASH_EFFECT_SPEED : baseVelocity;
        baseVelocity = m_input.Get_B_Held() ? baseVelocity * GLOBAL_VALUES.GROUNDSLAM_SLOWDOWN_AMOUNT : baseVelocity;
        // add on all additional external forces
        foreach (ExternalForce f in m_Forces)
        {
            baseVelocity += f.m_Force;
        }

        m_body.velocity = baseVelocity;

        m_manager.Get_Animator().SetFloat(GLOBAL_VALUES.ANIM_FLOAT_SPEED, mag);
    }

    void Do_Rotation()
    {
        float rotate_speed = m_manager.Get_Rotate_Speed();
        float rotation_req = 0;
        float curr_ch = Custom_Math_Utils.nfmod(m_body.transform.rotation.eulerAngles.y, 360);
        if (m_input.Get_Requested_Magnitude() > 0)
        {
            rotation_req = m_input.Get_Requested_Rotation() - curr_ch;
            rotation_req = (Mathf.Abs(rotation_req) > 180) ? ((rotation_req > 0) ? (360 - Mathf.Abs(rotation_req)) * -1 : (360 - Mathf.Abs(rotation_req))) : rotation_req;
            rotation_req = (Mathf.Abs(rotation_req) > rotate_speed) ? ((rotation_req > 0) ? rotate_speed : (rotation_req < 0) ? - rotate_speed : 0) : rotation_req;
        }
        m_body.transform.Rotate(new Vector3(0.0f, rotation_req, 0.0f));
    }
}
