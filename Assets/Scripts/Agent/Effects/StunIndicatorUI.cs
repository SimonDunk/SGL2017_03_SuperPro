using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StunIndicatorUI: MonoBehaviour
{
    public Canvas stunCanvas;
    public Image AButton;
    public Sprite AUp;
    public Sprite ADown;

	void Start ()
    {
        stunCanvas = GetComponent<Canvas>();
        AButton = stunCanvas.GetComponentInChildren<Image>();
        stunCanvas.gameObject.SetActive(false);
    }
	
    public void SetStunned()
    {
        stunCanvas.gameObject.SetActive(true);
    }

    public void SetNotStunned()
    {
        stunCanvas.gameObject.SetActive(false);
    }

    public void A_Pressed()
    {        
        AButton.sprite = AUp;      
    }

    public void A_Released()
    {
        AButton.sprite = ADown;
    }

}
