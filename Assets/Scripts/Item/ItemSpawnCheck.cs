using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnCheck : MonoBehaviour {

    public List<GameObject> items;
    public bool busy;

	// Use this for initialization
	void Start ()
    {
        items = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        GameObject other = collider.gameObject;
        if (other.CompareTag("PICKUP_ITEM"))
        {
            items.Add(other);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        GameObject other = collider.gameObject;

        if (other.CompareTag("PICKUP_ITEM"))
        {
            if (items.Contains(other))
            {
                items.Remove(other);
            }
        }
    }

    public bool isFree()
    {
        return items.Count == 0;
    }

    public void Reset()
    {
        items.Clear();
    }
}
