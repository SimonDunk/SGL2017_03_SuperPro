using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCollector {

    AgentManager m_Manager;
    int Punch_Usage;
    int Groundslam_Usage;
    int Dash_Usage;
    float Stun_Duration;
    int Heal_Done;
    int Pickup_Usage;
    int Items_Thrown;
    int Items_Dropped;
    int Players_Stunned;
    int AI_Stunned;
    int Pauses;
    int A_Button_Missed;
    int B_Button_Missed;
    int X_Button_Missed;
    int Y_Button_Missed;
    int Items_Checkedout;
    int Sale_Items_Checkedout;
    int MegaSale_Items_Checkedout;
    float Security_Chase_Time;
    int Items_Stolen;
    int Stunned_By_Items_Count;
    int Environmental_Damage;

    // Use this for initialization
    public StatCollector (AgentManager Manager)
    {
        m_Manager = Manager;
        // Initial setup of the player stats
        Punch_Usage = 0;
        Dash_Usage = 0;
        Stun_Duration = 0.0f;
        Security_Chase_Time = 0.0f;
        Heal_Done = 0;
        Pickup_Usage = 0;
        Items_Thrown = 0;
        Items_Dropped = 0;
        Players_Stunned = 0;
        AI_Stunned = 0;
        Pauses = 0;
        A_Button_Missed = 0;
        B_Button_Missed = 0;
        X_Button_Missed = 0;
        Y_Button_Missed = 0;
        Groundslam_Usage = 0;
        Items_Checkedout = 0;
        Sale_Items_Checkedout = 0;
        MegaSale_Items_Checkedout = 0;
        Items_Stolen = 0;
        Stunned_By_Items_Count = 0;
        Environmental_Damage = 0;
    }

    public string Get_String()
    {
        return "" + m_Manager.Get_Controller_Number() + "," + Dash_Usage + "," + Punch_Usage + "," + Groundslam_Usage + "," + Items_Thrown + ", " + Pickup_Usage + "," + Items_Dropped + "," + Stun_Duration + "," + Heal_Done + "," + Players_Stunned + "," + AI_Stunned + "," + Pauses + "," + A_Button_Missed + "," + B_Button_Missed + "," + X_Button_Missed + "," + Y_Button_Missed + "," + m_Manager.Get_Score();
    }

    public void Reset()
    {
        Punch_Usage = 0;
        Dash_Usage = 0;
        Stun_Duration = 0.0f;
        Heal_Done = 0;
        Pickup_Usage = 0;
        Items_Thrown = 0;
        Items_Dropped = 0;
        Players_Stunned = 0;
        AI_Stunned = 0;
        Pauses = 0;
        A_Button_Missed = 0;
        B_Button_Missed = 0;
        X_Button_Missed = 0;
        Y_Button_Missed = 0;
        Groundslam_Usage = 0;
        Items_Checkedout = 0;
        Sale_Items_Checkedout = 0;
        MegaSale_Items_Checkedout = 0;
        Items_Stolen = 0;
        Stunned_By_Items_Count = 0;
        Environmental_Damage = 0;
    }

    public void Count_Punch()
    {
        Punch_Usage += 1;
    }
    public int Get_Punches()
    {
        return Punch_Usage;
    }
    public void Count_Dash()
    {
        Dash_Usage += 1;
    }
    public int Get_Dashes()
    {
        return Dash_Usage;
    }
    public void Count_Stun(float duration)
    {
        Stun_Duration += duration;
    }
    public float Get_Stun_Time()
    {
        return Stun_Duration;
    }
    public void Count_Heal()
    {
        Heal_Done += 1;
    }
    public int Get_Heal()
    {
        return Heal_Done;
    }
    public void Count_Pickup()
    {
        Pickup_Usage += 1;
    }
    public int Get_Pickup()
    {
        return Pickup_Usage;
    }
    public void Count_Throw()
    {
        Items_Thrown += 1;
    }
    public int Get_Throws()
    {
        return Items_Thrown;
    }
    public void Count_Drop()
    {
        Items_Dropped += 1;
    }
    public int Get_Drops()
    {
        return Items_Dropped;
    }
    public void Count_Player_Stun()
    {
        Players_Stunned += 1;
    }
    public int Get_Players_Stunned()
    {
        return Players_Stunned;
    }
    public void Count_AI_Stunned()
    {
        AI_Stunned += 1;
    }
    public int Get_AI_Stunned()
    {
        return AI_Stunned;
    }
    public void Count_Pause()
    {
        Pauses += 1;
    }
    public int Get_Pauses()
    {
        return Pauses;
    }
    public void Count_A_Miss()
    {
        A_Button_Missed += 1;
    }
    public int Get_A_Misses()
    {
        return A_Button_Missed;
    }
    public void Count_B_Miss()
    {
        B_Button_Missed += 1;
    }
    public int Get_B_Misses()
    {
        return B_Button_Missed;
    }
    public void Count_X_Miss()
    {
        X_Button_Missed += 1;
    }
    public int Get_X_Misses()
    {
        return X_Button_Missed;
    }
    public void Count_Y_Miss()
    {
        Y_Button_Missed += 1;
    }
    public int Get_Y_Misses()
    {
        return Y_Button_Missed;
    }
    public void Count_Groundslam()
    {
        Groundslam_Usage += 1;
    }
    public int Get_Groundslams()
    {
        return Groundslam_Usage;
    }
    public void Count_Item_Checkout()
    {
        Items_Checkedout += 1;
    }
    public int Get_Regular_Items_Checkedout()
    {
        return Items_Checkedout;
    }
    public int Get_Items_Bought()
    {
        return Items_Checkedout + Sale_Items_Checkedout + MegaSale_Items_Checkedout;
    }
    public void Count_Sale_Item_Checkout()
    {
        Sale_Items_Checkedout += 1;
    }
    public int Get_Sale_Items_Checkedout()
    {
        return Sale_Items_Checkedout;
    }
    public void Count_MegaSale_Item_Checkout()
    {
        MegaSale_Items_Checkedout += 1;
    }
    public int Get_MegaSale_Items_Checkedout()
    {
        return MegaSale_Items_Checkedout;
    }
    public void Count_Item_Stolen()
    {
        Items_Stolen += 1;
    }
    public int Get_Items_Stolen()
    {
        return Items_Stolen;
    }
    public void Count_Stunned_By_Item()
    {
        Stunned_By_Items_Count += 1;
    }
    public int Get_Stunned_By_Items()
    {
        return Stunned_By_Items_Count;
    }
    public void Count_Environmental_Damage(int damage)
    {
        Environmental_Damage += damage;
    }
    public int Get_Environmental_Damage()
    {
        return Environmental_Damage;
    }
    public void Count_Security_Chase_Time(float time)
    {
        Security_Chase_Time += time;
    }
    public float Get_Security_Chase_Time()
    {
        return Security_Chase_Time;
    }
}
