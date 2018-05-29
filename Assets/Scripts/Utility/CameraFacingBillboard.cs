using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    public Camera m_Camera;

    void Awake()
    {
        m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    private void Find_Camera()
    {
        m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    void Update()
    {
        try
        {
            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
        }
        catch
        {
            Find_Camera();
        }
    }
}