using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
public class Controller_Input : Input_Methods
{
    bool playerIndexSet = false;
    GamePadState state;
    GamePadState prevState;
    PlayerIndex playerIndex;
    Timer t = new Timer();

    public Controller_Input(int? p_num) : base(p_num)
    {
        playerIndex = (PlayerIndex)p_num;
    }

    public override float Get_Requested_Rotation ()
    {
        // Return the current rotation of the joystick
        float Joystick_X = state.ThumbSticks.Left.X;
        float Joystick_Y = state.ThumbSticks.Left.Y;    
        float ret = 0.0f;
        if ((Joystick_X != 0) || (Joystick_Y != 0))
        {
            ret = (Custom_Math_Utils.nfmod(-(((Mathf.Atan2(Joystick_Y, Joystick_X)) * Mathf.Rad2Deg) - 90), 360));
            ret = (ret < 0) ? ret + 360 : ret;
        }
        return ret;
    }

    public override float Get_Requested_Magnitude ()
    {
        // Return the current magnitude of the joystick
        return Mathf.Max(Mathf.Abs(state.ThumbSticks.Left.X), Mathf.Abs(state.ThumbSticks.Left.Y));
    }

    public override void Update()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);
        t.Update();
        if (t.isComplete())
        {
            GamePad.SetVibration(playerIndex, 0, 0);
        }
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet && !prevState.IsConnected)
        {
            //for (int i = 0; i < 4; ++i)
            //{
            PlayerIndex testPlayerIndex = playerIndex;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected)
            {
                playerIndex = testPlayerIndex;
                playerIndexSet = true;
            }
            else
            {
                //Debug.Log(string.Format("GamePad not found {0}", testPlayerIndex));
            }
            //}
        }
        else if (prevState.IsConnected && !state.IsConnected && playerIndexSet)
        {
            Debug.Log(string.Format("Disconnected GamePad {0}", playerIndex));
        }
        else if (!prevState.IsConnected && state.IsConnected && playerIndexSet)
        {
            Debug.Log(string.Format("Reconnected GamePad {0}", playerIndex));
        }
        /*
        if(state.IsConnected && playerIndexSet)
        {
            Debug.Log("Update here");
        }*/


    }

    public override bool Get_A_Pressed() 
	{
        return (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed);
    }

    public override bool Get_A_Released()
    {
        return (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released);
    }
    public override bool Get_B_Pressed()
    {
        return (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed);
    }
    public override bool Get_B_Held()
    {
        return (prevState.Buttons.B == ButtonState.Pressed && state.Buttons.B == ButtonState.Pressed);
    }
    public override bool Get_B_Released()
    {
        return (prevState.Buttons.B == ButtonState.Pressed && state.Buttons.B == ButtonState.Released);
    }
    public override bool Get_X_Pressed()
    {
        return (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed);
    }
    public override bool Get_Y_Pressed()
    {
        return (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed);
    }
    public override bool Get_LB_Pressed()
    {
        return (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed);
    }
    public override bool Get_RB_Pressed()
    {
        return (prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed);
    }
    public override bool Get_LT_Pressed()
    {
        return (prevState.Triggers.Left == 0 && state.Triggers.Left > 0);
    }
    public override bool Get_RT_Pressed()
    {
        return (prevState.Triggers.Right == 0 && state.Triggers.Right > 0);
    }
    public override bool Get_Start_Pressed()
    {
        return (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed);
    }
	
    public override bool Get_Vert_Positive()
    {
        return (prevState.ThumbSticks.Left.Y == 0 && state.ThumbSticks.Left.Y > 0);
	}

    public override bool Get_Vert_Negative()
    {
        return (prevState.ThumbSticks.Left.Y == 0 && state.ThumbSticks.Left.Y < 0);
    }

    public override bool Get_Horz_Positive()
    {
        return (prevState.ThumbSticks.Left.X == 0 && state.ThumbSticks.Left.X > 0);
    }

    public override bool Get_Horz_Negative()
    {
        return (prevState.ThumbSticks.Left.X == 0 && state.ThumbSticks.Left.X < 0);
    }

    public override bool Get_DL_Pressed()
    {
        return (prevState.DPad.Left == 0  && state.DPad.Left > 0);
    }

    public override bool Get_DU_Pressed()
    {
        return (prevState.DPad.Up == 0 && state.DPad.Up > 0);
    }

    public override bool Get_DR_Pressed()
    {
        return (prevState.DPad.Right == 0 && state.DPad.Right > 0);
    }
    public override bool Get_DD_Pressed()
    {
        return (prevState.DPad.Down == 0 && state.DPad.Down > 0);
    }

    public bool Get_Player_Index_Set
    {
        get { return playerIndexSet; }
        set { playerIndexSet = value; }
    }

    public override PlayerIndex Get_Player_Index()
    {
        return playerIndex;
    }

    public override void Controller_Rumble(float l, float r)
    {
        t = new Timer(GLOBAL_VALUES.CONTROLLER_RUMBLE_DURATION, true);
        GamePad.SetVibration(playerIndex, l, r);
    }

}