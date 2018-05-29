using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeAI_Control : Input_Methods
{

    private Pathing_Point m_target = null;
    private AgentManager m_Manager;
    private Pathing_Manager m_Path_Manager;
    private PickupBox m_PickupBox;
    private Path m_Path;
    private Vector3 previous_position;
    private Timer movement_timer;
    private bool A_Pressed = false;
    private bool B_Pressed = false;
    private bool X_Pressed = false;
    private bool Y_Pressed = false;

    private void Check_Pathing()
    {
        if (m_target == null)
        {
            Get_Target();
        }
        if (m_Path == null)
        {
            Calculate_Path();
        }
    }
    public void Recalculate()
    {
        m_target = null;
        m_Path = null;
        m_target = m_Path_Manager.Get_Random_Node(null);
        Calculate_Path();
        Check_Pathing();
    }

    public override float Get_Requested_Rotation()
    {
        float ret = 0.0f;
        Check_Pathing();
        if (m_target != null)
        {
            try
            {
                Vector3 node_target = m_Path.Current_Point().m_Position.transform.position;
                Vector3 my_pos = m_Manager.Get_Position();
                float Joystick_X = node_target.x - my_pos.x;
                float Joystick_Y = node_target.z - my_pos.z;
                if ((Joystick_X != 0) || (Joystick_Y != 0))
                {
                    ret = (Custom_Math_Utils.nfmod(-(((Mathf.Atan2(Joystick_Y, Joystick_X)) * Mathf.Rad2Deg) - 90), 360));
                    ret = (ret < 0) ? ret + 360 : ret;
                }
            }
            catch
            { Check_Pathing(); }
        }
        return ret;
    }
    public override float Get_Requested_Magnitude() { return 0.45f; }

    private void Calculate_Movement()
    {
        bool recalc = false;
        try
        {
            if (movement_timer.isComplete())
            {
                // watchdog for mvoement
                if ((previous_position != new Vector3()) && (Vector3.Distance(m_Manager.Get_Position(), previous_position) < GLOBAL_VALUES.AI_TARGET_RADIUS))
                {
                    recalc = true;
                }
                previous_position = m_Manager.Get_Position();
                movement_timer.Add(1, true);
            }
            if (Vector3.Distance(m_Manager.Get_Position(), m_Path.Current_Point().m_Position.position) < 1)
            {
                // have arrived at the current target point
                m_Path.Increment_Point();
                if (m_Path.isFinished())
                {
                    Get_Target();
                }
            }
            if (recalc)
            {
                // stopped moving so recalculate path
                Recalculate();
            }
        }
        catch
        {
            Check_Pathing();
        }
    }

    private void Calculate_Path()
    {
        if (m_target != null)
        {
            m_Path = m_Path_Manager.Find_Path(m_Manager.Get_Position(), m_target.m_Position.position);
        }
    }

    private void Get_Target(bool recalc = false)
    {
        if (m_target == null || m_Path == null || recalc == true || m_Path.isFinished())
        {
            Recalculate();
        };
    }

    public LargeAI_Control(int? p_num, AgentManager manager) : base(p_num)
    {
        movement_timer = new global::Timer(1, true);
        m_Path_Manager = GameObject.FindGameObjectWithTag(GLOBAL_VALUES.TAG_AI_MANAGER).GetComponent<Pathing_Manager>();
        m_Manager = manager;
        m_PickupBox = m_Manager.Get_Object().GetComponentInChildren<PickupBox>();
    }

    public override void Update()
    {
        movement_timer.Update();
        Calculate_Movement();
    }

    public override void Reset_Pressed()
    {
        A_Pressed = false;
        B_Pressed = false;
        X_Pressed = false;
        Y_Pressed = false;
    }

    public override bool Get_A_Pressed()
    {
        if (A_Pressed)
        {
            A_Pressed = false;
            return true;
        }
        return false;
    }

    public override bool Get_B_Pressed()
    {
        if (B_Pressed)
        {
            B_Pressed = false;
            return true;
        }
        return false;
    }

    public override bool Get_X_Pressed()
    {
        if (X_Pressed)
        {
            X_Pressed = false;
            return true;
        }
        return false;
    }

    public override bool Get_Y_Pressed()
    {
        if (Y_Pressed)
        {
            Y_Pressed = false;
            return true;
        }
        return false;
    }
}
