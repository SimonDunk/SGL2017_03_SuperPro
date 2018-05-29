using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDTracking : MonoBehaviour {

    public GameObject target;
	
	// Update is called once per frame
	void Update () {
        var wantedPos = Camera.main.WorldToScreenPoint(target.transform.position);
        transform.position = wantedPos;
    }
}