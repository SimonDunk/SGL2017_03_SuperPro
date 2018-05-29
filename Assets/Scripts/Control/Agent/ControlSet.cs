using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
/*
 * TODO: This class and the associated Controller_Input and Input_Methods class needs a better way to check for all the button presses.
 * possibly a dictionary of buttons pressed for quick lookup and a single function to call into that dictionary?
 */
public class ControlSet {
    // PRIVATE
    List<Input_Methods> m_Inputs = null;

    public ControlSet()
    {
        m_Inputs = new List<Input_Methods>();
    }
    
    public void Add_Input(Input_Methods new_input)
    {
        if (!m_Inputs.Contains(new_input))
        {
            m_Inputs.Add(new_input);
        }
    }
	// Update is called once per frame
	public void Update()
    {
        foreach (Input_Methods input in m_Inputs)
        {
            input.Update();
        }    
    }

    public void Reset()
    {
        foreach (Input_Methods input in m_Inputs)
        {
            input.Reset_Pressed();
        }
    }

    public List<int?> A_Pressed_List()
    {
        /* Return a list of input methods who hit A */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if(input.Get_A_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }

    public List<int?> B_Pressed_List()
    {
        /* Return a list of input methods who hit B */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_B_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }

    public List<int?> X_Pressed_List()
    {
        /* Return a list of input methods who hit X */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_X_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }

    public List<int?> Y_Pressed_List()
    {
        /* Return a list of input methods who hit Y */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Y_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }

    public List<int?> LB_Pressed_List()
    {
        /* Return a list of input methods who hit LB */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_LB_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }
    public List<int?> RB_Pressed_List()
    {
        /* Return a list of input methods who hit RB */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_RB_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }
    public List<int?> LT_Pressed_List()
    {
        /* Return a list of input methods who hit LT */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_LT_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }
    public List<int?> RT_Pressed_List()
    {
        /* Return a list of input methods who hit RT */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_RT_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }

    public List<int?> DU_Pressed_List()
    {
        /* Return a list of input methods who hit D-Pad Up */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_DU_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }
    public List<int?> DD_Pressed_List()
    {
        /* Return a list of input methods who hit D-Pad Down */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_DD_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }

    public List<int?> DL_Pressed_List()
    {
        /* Return a list of input methods who hit D-Pad Left */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_DL_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }

    public List<int?> DR_Pressed_List()
    {
        /* Return a list of input methods who hit D-Pad Right */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_DR_Pressed())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }
    public List<int?> JoyUp_Pressed_List()
    {
        /* Return a list of input methods who hit Joystick Up */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Vert_Positive())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }

    public List<int?> JoyDown_Pressed_List()
    {
        /* Return a list of input methods who hit Joystick Down */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Vert_Negative())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }
    public List<int?> JoyLeft_Pressed_List()
    {
        /* Return a list of input methods who hit Joystick Left */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Horz_Negative())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }
    public List<int?> JoyRight_Pressed_List()
    {
        /* Return a list of input methods who hit Joystick Right */
        List<int?> ret = new List<int?>();
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Horz_Positive())
            {
                ret.Add((int)input.Get_Player_Index());
            }
        }
        return ret;
    }
    public bool Any_A_Pressed()
    {
        /* Return whether any of the input methods in the list hit A */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_A_Pressed())
            {
                return true;
            }
        }
        return false;
    }

    public bool Any_B_Pressed()
    {
        /* Return whether any of the input methods in the list hit B */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_B_Pressed())
            {
                return true;
            }
        }
        return false;
    }

    public bool Any_X_Pressed()
    {
        /* Return whether any of the input methods in the list hit X */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_X_Pressed())
            {
                return true;
            }
        }
        return false;
    }

    public bool Any_Y_Pressed()
    {
        /* Return whether any of the input methods in the list hit Y */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Y_Pressed())
            {
                return true;
            }
        }
        return false;
    }

    public bool Any_LB_Pressed()
    {
        /* Return whether any of the input methods in the list hit LB */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_LB_Pressed())
            {
                return true;
            }
        }
        return false;
    }

    public bool Any_RB_Pressed()
    {
        /* Return whether any of the input methods in the list hit RB */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_RB_Pressed())
            {
                return true;
            }
        }
        return false;
    }
    public bool Any_LT_Pressed()
    {
        /* Return whether any of the input methods in the list hit LT */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_LT_Pressed())
            {
                return true;
            }
        }
        return false;
    }

    public bool Any_RT_Pressed()
    {
        /* Return whether any of the input methods in the list hit RT */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_RT_Pressed())
            {
                return true;
            }
        }
        return false;
    }
    public bool Any_DU_Pressed()
    {
        /* Return whether any of the input methods in the list hit D-Pad Up */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_DU_Pressed())
            {
                return true;
            }
        }
        return false;
    }
    public bool Any_DD_Pressed()
    {
        /* Return whether any of the input methods in the list hit D-Pad Down */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_DD_Pressed())
            {
                return true;
            }
        }
        return false;
    }
    public bool Any_DL_Pressed()
    {
        /* Return whether any of the input methods in the list hit D-Pad left */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_DL_Pressed())
            {
                return true;
            }
        }
        return false;
    }
    public bool Any_DR_Pressed()
    {
        /* Return whether any of the input methods in the list hit D-Pad Right */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_DR_Pressed())
            {
                return true;
            }
        }
        return false;
    }
    public bool Any_JoyUp_Pressed()
    {
        /* Return whether any of the input methods in the list hit Joystick Up */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Vert_Positive())
            {
                return true;
            }
        }
        return false;
    }
    public bool Any_JoyDown_Pressed()
    {
        /* Return whether any of the input methods in the list hit Joystick Down */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Vert_Negative())
            {
                return true;
            }
        }
        return false;
    }
    public bool Any_JoyLeft_Pressed()
    {
        /* Return whether any of the input methods in the list hit Joystick Left */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Horz_Negative())
            {
                return true;
            }
        }
        return false;
    }
    public bool Any_JoyRight_Pressed()
    {
        /* Return whether any of the input methods in the list hit Joystick Right */
        foreach (Input_Methods input in m_Inputs)
        {
            if (input.Get_Horz_Positive())
            {
                return true;
            }
        }
        return false;
    }
}
