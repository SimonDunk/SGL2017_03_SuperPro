using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager
{
    public enum CameraState { Default, Shake, Animation};
    //PUBLIC

    //PRIVATE
    private GameManager m_GameManager = null;
    private RoundManager m_RoundManager = null;
    private Camera m_ActiveCamera = null;
    private Camera m_MainCamera = null;

    private float l_ShakeDuration = 0;
    private float l_ShakeIntensity = GLOBAL_VALUES.CAMERA_SHAKE_INTENSITY;

    private CameraState m_State;

    private Animator m_Cam_An;
    private Transform m_Rig_Pos;

    private string LevelNumber;

    private List<Light> m_lights = new List<Light>();

    [HideInInspector]
    public float
    l_DampTime = GLOBAL_VALUES.CAMERA_DAMPENING_TIME,
    l_ScreenEdgeBuffer = GLOBAL_VALUES.CAMERA_EDGE_BUFFER,
    l_MinSize = GLOBAL_VALUES.CAMERA_MIN_SIZE,
    l_MaxSize = GLOBAL_VALUES.CAMERA_MAX_SIZE;

    [HideInInspector]
    public Vector3
    //Position camera is moving towards
    l_DesiredPosition;

    private float l_ReduceMovementIntensity = GLOBAL_VALUES.CAMERA_RIG_MOVEMENT_INTENSITY;

    private float l_ZoomSpeed; //reference speed for the smooth damping of the orthographic size
    private Vector3 l_MoveVelocity; //Reference velocity for the smooth damping of the position

    private List<CameraForce> VectorForces = new List<CameraForce>();
    private Vector3 DesiredForce;
    private float l_DefaultYPos;
    private float l_ZPosOffset = 6;

    public CameraManager(RoundManager round_manager, GameManager game_manager)
    {
        m_RoundManager = round_manager;
        m_GameManager = game_manager;
        m_MainCamera = Camera.main;
        m_ActiveCamera = m_MainCamera;
        m_ActiveCamera.enabled = true;
        m_Rig_Pos = m_ActiveCamera.transform.parent;
        l_DefaultYPos = m_Rig_Pos.position.y;
        m_Cam_An = m_ActiveCamera.GetComponentInParent<Animator>();
        LevelNumber = 6.ToString();
        m_lights = new List<Light>(GameObject.Find("Lights").GetComponentsInChildren<Light>());
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Camera"))
        {
            if (obj.name != m_ActiveCamera.gameObject.name)
            {
                obj.GetComponent<Camera>().enabled = false;
            }
        }
    }

    public Camera MainCamera()
    {
        return m_MainCamera;
    }
	public void Update ()
    {
        switch(m_State)
        {
            case CameraState.Default:
                Move();
                Zoom();
                UpdateForces();
                if (l_ShakeDuration > 0 || VectorForces.Count > 0)
                {
                    m_Rig_Pos.localPosition = Random.insideUnitSphere * l_ShakeIntensity + m_Rig_Pos.localPosition;
                    l_ShakeDuration -= Time.deltaTime;
                    m_GameManager.RumbleAll(l_ShakeIntensity, l_ShakeIntensity);
                }
                break;
            case CameraState.Shake:

                /*else
                {
                    SetState(CameraState.Default);
                }*/
                break;
            case CameraState.Animation:
                if (m_Cam_An != null)
                {
                    if (m_Cam_An.GetCurrentAnimatorStateInfo(0).IsName("Camera_Anim_" + LevelNumber))
                    {
                        m_Cam_An.enabled = false;
                        SetState(CameraState.Default);
                    }
                }
                break;
        }
    }

    public void SetState(CameraState state)
    {
        m_State = state;
        switch (state)
        {
            case CameraState.Default:
                //move camera back to spawn
                break;
            case CameraState.Shake:

                break;
            case CameraState.Animation:
                if (m_Cam_An != null)
                {
                    m_Cam_An.Play("Camera_Anim_" + LevelNumber);
                }
                break;
        }
    }

    public void SetActiveCamera(Camera new_camera)
    {
        if (m_ActiveCamera != null)
        {
            m_ActiveCamera.enabled = false;
        }
        m_ActiveCamera = new_camera;
        if (m_ActiveCamera != null)
        {
            m_ActiveCamera.enabled = true;
        }
    }

    public void SetupScene()
    {
        SetState(CameraState.Animation);
    }

    ~CameraManager()
    {
        // Destructor, when this is destroyed what it does
        m_ActiveCamera = null;
        m_GameManager = null;
    }

    private void Move()
    {
        //Average out positions of all players
        FindAveragePosition();
        // Smoothly transition to desired position
        m_Rig_Pos.position = Vector3.SmoothDamp(m_Rig_Pos.position, l_DesiredPosition, ref l_MoveVelocity, l_DampTime*1.5f);
    }

    private void FindAveragePosition()
    {
        Vector3 avgPos = new Vector3();
        int numTargets = 0;
        List<Transform>  l_Targets = m_GameManager.Get_Player_Objects_Transform();
        for (int i = 0; i < l_Targets.Count; i++)
        {
            //add to average
            avgPos += l_Targets[i].position;
            numTargets++;
        }
        //averaging targets locations
        if (numTargets > 0)
            avgPos /= numTargets*l_ReduceMovementIntensity;
        //constant y value (players can't jump)
        //avgPos.y = m_Rig_Pos.position.y;
        avgPos.y = l_DefaultYPos;
        avgPos.z -= l_ZPosOffset;
        l_DesiredPosition = avgPos;
    }

    private void Zoom()
    {
        //finds required zoom screen size
        float requiredSize = FindRequiredSize();
        if (m_ActiveCamera.orthographic)
        {
            //smoothly transitions to correct size
            m_ActiveCamera.orthographicSize = Mathf.SmoothDamp(m_ActiveCamera.orthographicSize, requiredSize, ref l_ZoomSpeed, l_DampTime);
        }
        else
        {
            m_ActiveCamera.fieldOfView = Mathf.SmoothDamp(m_ActiveCamera.fieldOfView, requiredSize, ref l_ZoomSpeed, l_DampTime);
        }
    }

    private float FindRequiredSize()
    {
        //find postion camera is moving towards in local space
        Vector3 desiredLocalPos = m_Rig_Pos.InverseTransformPoint(l_DesiredPosition);

        float size = 0f;
        List<Transform> l_Targets = m_GameManager.Get_Player_Objects_Transform();
        for (int i = 0; i < l_Targets.Count; i++)
        {
            //find the position of the target in the camera's local space.
            Vector3 targetLocalPos = m_Rig_Pos.InverseTransformPoint(l_Targets[i].position);
            // Find the position of the target from the desired position of the camera's local space.
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
            // Choose the largest out of the current size and the distance of the player 'up' or 'down' from the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            // Choose the largest out of the current size and the calculated size based on the player being to the left or right of the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_ActiveCamera.aspect);
        }
        // Add the edge buffer to the size.
        size += l_ScreenEdgeBuffer;
        // Make sure the camera's size isn't below the minimum.
        size = Mathf.Max(size, l_MinSize);
        size = Mathf.Min(size, l_MaxSize);
        return size;
    }

    public void SetStartPositionAndSize()
    {
        //Initializing Camera
        FindAveragePosition();
        m_Rig_Pos.position = l_DesiredPosition;
        if (m_ActiveCamera.orthographic)
        {
            m_ActiveCamera.orthographicSize = FindRequiredSize();
        }
        else
        {
            m_ActiveCamera.fieldOfView = FindRequiredSize();
        }
    }
        
    public void SetScreenShake(float duration, float intensity)
    {
        //SetState(CameraState.Shake);
        intensity = Mathf.Lerp(0, 1, intensity);
        l_ShakeIntensity = intensity;
        l_ShakeDuration = duration;
    }

    public void SetScreenShake(float intensity)
    {
        intensity = Mathf.Lerp(0, 1, intensity);
        l_ShakeIntensity = intensity;
    }

    private void UpdateForces()
    {
        List<CameraForce> toDelete = new List<CameraForce>();
        DesiredForce = m_Rig_Pos.position;
        for (int i = VectorForces.Count-1; i >= 0; i--)
        {
            CameraForce f = VectorForces[i];
            f.Update();
            if (f.isComplete())
            {
                toDelete.Add(f);
            }
            else
            {
                //DesiredForce.x = (DesiredForce.x += f.Get_Vector().x) > GLOBAL_VALUES.CAMERA_SHAKE_MAX_FORCE ? GLOBAL_VALUES.CAMERA_SHAKE_MAX_FORCE : DesiredForce.x += f.Get_Vector().x;
                //DesiredForce.y = (DesiredForce.y += (f.Get_Vector().y/2)) > GLOBAL_VALUES.CAMERA_SHAKE_MAX_FORCE ? GLOBAL_VALUES.CAMERA_SHAKE_MAX_FORCE : DesiredForce.x += f.Get_Vector().y;
                //DesiredForce.z = (DesiredForce.z += f.Get_Vector().z) > GLOBAL_VALUES.CAMERA_SHAKE_MAX_FORCE ? GLOBAL_VALUES.CAMERA_SHAKE_MAX_FORCE : DesiredForce.x += f.Get_Vector().z;
                DesiredForce.x += f.Get_Vector().x; // 5 and -5
                DesiredForce.y += f.Get_Vector().y * .5f;
                DesiredForce.z += f.Get_Vector().z; //-2 and -8
            }
        }
        foreach(CameraForce cf in toDelete)
        {
            VectorForces.Remove(cf);
        }
        if(VectorForces.Count > 0)
        {
            m_Rig_Pos.position = Vector3.SmoothDamp(m_Rig_Pos.position, DesiredForce, ref l_MoveVelocity, l_DampTime);
            SetScreenShake(VectorForces.Count / GLOBAL_VALUES.CAMERA_FORCE_SHAKE_INTENSITY);
        }
    }

    public void AddForce(float f, Vector3 v)
    {
        float effectTime = f;
        VectorForces.Add(new CameraForce(v/GLOBAL_VALUES.CAMERA_FORCE_MOVEMENT_INTENSITY, effectTime));
    }

    public void AddForce(float f)
    {
        float effectTime = f;
        VectorForces.Add(new CameraForce(new Vector3(0,0,0), effectTime));
    }

    public void SetLightIntensity(float f, float p)
    {
        int i = 0;
        foreach (Light l in m_lights)
        {
            if(i <= 1)
            {
                l.intensity = f;
            }
            else
            {
                l.intensity = p;
            }
            i++;
            
        }
    }
}
