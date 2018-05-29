using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanStack : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        DisableAll();
    }

    void OnTriggerEnter()
    {
        EnableAll();
    }

    public void DisableAll()
    {
        // disable all rigidbodies on children
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform tr = gameObject.transform.GetChild(i);
            if (tr.GetComponent<Rigidbody>() != null)
            {
                tr.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (tr.GetComponent<MeshCollider>() != null)
            {
                tr.GetComponent<MeshCollider>().enabled = false;
            }
        }
    }

    public void EnableAll()
    {
        // enable all rigidbodies on children
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform tr = gameObject.transform.GetChild(i);
            if (tr.GetComponent<Rigidbody>() != null)
            {
                tr.GetComponent<Rigidbody>().isKinematic = false;
            }
            if (tr.GetComponent<MeshCollider>() != null)
            {
                tr.GetComponent<MeshCollider>().enabled = true;
            }
        }
    }
}
