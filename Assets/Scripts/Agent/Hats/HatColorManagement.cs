using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatColorManagement : MonoBehaviour{
    //public
    public string m_HatName = "ENTER NAME HERE";

    //private
    private string[] m_Colors = new string[5] { "BLUE", "GREEN", "PINK", "RED", "YELLOW" };
    private int m_Current_Color = 0;
    private MeshRenderer m_Mesh;

    private void Awake()
    {
        m_Mesh = gameObject.GetComponent<MeshRenderer>();
    }
    private void UpdateHatMaterial()
    {
        Material temp_material;
        string temp_string = "Materials/Hat_Mats/HATMAT_" + m_HatName + "_" + m_Colors[m_Current_Color];
        temp_material = Resources.Load(temp_string) as Material;
        //if no material found give default grey
        if (temp_material == null)
        {
            Debug.Log("ERROR: COULD NOT FIND HAT MATERIAL [" + temp_string + "], FIX!");
            temp_material = Resources.Load(GLOBAL_VALUES.PLAYER_MATERIAL_PINK) as Material;
        }
        m_Mesh.material = temp_material;
    }

    public bool SetColor(int col_idx)
    {
        if ((col_idx >= m_Colors.Length) || (col_idx < 0))
        {
            // not in array
            return false;
        }
        m_Current_Color = col_idx;
        if (m_HatName != "CHICKEN" && m_HatName != "FIRE")
        {
            UpdateHatMaterial();
        }
        return true;
    }

    private void Change_Hat(int dir)
    {
        if (m_HatName != "CHICKEN" && m_HatName != "FIRE")
        {
            if (dir < 0)
            {
                // Previous color
                m_Current_Color--;
                if (m_Current_Color < 0)
                {
                    m_Current_Color = (m_Colors.Length - 1);
                }
            }
            else if (dir > 0)
            {
                // Next color
                m_Current_Color++;
                if (m_Current_Color > (m_Colors.Length - 1))
                {
                    m_Current_Color = 0;
                }
            }
            UpdateHatMaterial();
        }
    }

    public void Next_Color()
    {
        Change_Hat(1);
    }

    public void Previous_Color()
    {
        Change_Hat(-1);
    }
}
