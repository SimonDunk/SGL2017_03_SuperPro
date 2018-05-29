using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIntroAnimEvents : MonoBehaviour {

    public Vector3 itemTarget;
    public GameObject itemPrefab;
    public GameObject checkoutArrow;

    private GameObject item;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SpawnItem()
    {
        item = (GameObject)Instantiate(itemPrefab, itemTarget, Quaternion.identity);
        checkoutArrow.SetActive(true);
    }

    public void DestroyItems()
    {
        Destroy(item);
    }

    public void DestroyArrow()
    {
        Destroy(checkoutArrow);
    }
}
