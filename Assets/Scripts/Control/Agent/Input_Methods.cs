using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Input_Methods
{
    public int? p_num = null;
    public Input_Methods(int? player_number = null)
    {
        p_num = player_number;
    }

    public virtual float Get_Requested_Rotation() { return 0.0f; }
    public virtual float Get_Requested_Magnitude() { return 0.0f; }

    public virtual void Update() { }
    public virtual void Reset_Pressed() { Debug.Log("this does nothing"); }
    public virtual bool Get_A_Pressed() { return false; }
    public virtual bool Get_A_Released() { return false; }
    public virtual bool Get_B_Pressed() { return false; }
    public virtual bool Get_B_Held() { return false; }
    public virtual bool Get_B_Released() { return false; }
    public virtual bool Get_X_Pressed() { return false; }
    public virtual bool Get_Y_Pressed() { return false; }
    public virtual bool Get_LB_Pressed() { return false; }
    public virtual bool Get_RB_Pressed() { return false; }
    public virtual bool Get_LT_Pressed() { return false; }
    public virtual bool Get_RT_Pressed() { return false; }
    public virtual bool Get_Start_Pressed() { return false; }
    public virtual bool Get_Vert_Positive() { return false; }
    public virtual bool Get_Horz_Negative() { return false; }
    public virtual bool Get_Horz_Positive() { return false; }
    public virtual bool Get_Vert_Negative() { return false; }
    public virtual bool Get_DL_Pressed() { return false; }
    public virtual bool Get_DU_Pressed() { return false; }
    public virtual bool Get_DR_Pressed() { return false; }
    public virtual bool Get_DD_Pressed() { return false; }
    public virtual PlayerIndex Get_Player_Index() { return new PlayerIndex(); }
    public virtual void Controller_Rumble(float l, float r) { }
}