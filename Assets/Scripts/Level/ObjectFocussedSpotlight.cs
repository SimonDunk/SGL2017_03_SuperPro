using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFocussedSpotlight : MonoBehaviour {
    private GameObject m_Focus;
    private Item i;
    private Light m_Light;
    private bool m_On = false;

    private void Start()
    {
        m_Light = gameObject.GetComponent<Light>();
    }
    // Update is called once per frame
	void Update () {
        if (m_Focus != null)
        {
            gameObject.transform.LookAt(m_Focus.transform.position);
        }

        if (m_Focus != null && i != null && i.BeingCarried() && !i.isRare)
        {
            Turn_Off();
        }
	}

    public void Set_Focus(GameObject new_target)
    {
        if (new_target != null)
        {
            m_Focus = new_target;
            i = m_Focus.GetComponent<Item>();
        }
    }

    public void Turn_On()
    {
        m_On = true;
        gameObject.SetActive(true);
    }

    public void Turn_Off()
    {
        m_On = false;
        gameObject.SetActive(false);
    }

    public void Clear_Focus()
    {
        m_Focus = gameObject;
    }
}
