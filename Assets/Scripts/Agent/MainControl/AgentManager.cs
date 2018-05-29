using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public enum Agent_Type { Player , Base_AI, Large_AI, Security_AI};
public enum Agent_State { Active, Stunned }
public enum Agent_Movement_State { Enabled, Disabled };

public class AgentManager
{
    // Public
    public StatCollector m_StatCollector;

    // Private
    //-- Stats
    private float m_Strength = 0.0f;
    private float m_MoveSpeed = 0.0f;
    private float m_RotateSpeed = 0.0f;
    private string m_BadgeAllocation = "WINDOW_SHOPPER_1";
    //-- Timers
    private Timer m_Stun_Timer = new Timer();
    //-- Randoms?
    private Agent_Type m_Type;
    private Agent_State m_State;
    private Agent_Movement_State m_Enabled_State;
    private int m_Round_Score;
    private int m_Round_Score_Position;
    private int m_Round_Wins;
    private GameObject m_SpawnPoint = null;
    private GameObject m_CarriedItem = null;
    private int m_Colour = -1;
    private int m_ControllerNumber;
    private List<AgentSubscriber> m_Subscribers = new List<AgentSubscriber>();
    private int m_Player_Number = 0;
    //-- name things
    private string m_Player_Name = "";
    private Text m_Player_Name_Text;
    private GameObject m_Player_Nameplate;
    //-- Sub Componants
    private Input_Methods m_Input;
    private AgentMovement m_Movement;
    private AgentActions m_Actions;
    private CollisionDetection m_Collision_Detector;
    private GameObject m_Crown;
    private Animator m_Animator;
    //private AgentPowerups m_Powerup_State;
    //-- Body Bits
    private GameObject m_PlayerHat;
    private int m_HatNum = -1;
    private GameObject m_Instance;
    private Rigidbody m_Body;
    //-- Constraint settings
    private RigidbodyConstraints DISABLED_CONSTRAINTS = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    private RigidbodyConstraints ENABLED_CONSTRAINTS = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
    //-- Sound
    private AudioClip m_Sound_Stun;
    private AudioSource m_Sound_Source;
    //-- Particle Effects
    private GameObject m_Effect_Stun_Lights;
    private List<GameObject> m_Stun_Stars;
    private float m_Stun_Star_Time;
    private int m_Stun_Stars_Active;
    private GameObject m_Effect_Impact;
    private GameObject m_punchImpact;
    private GameObject m_throwImpact;
    private GameObject m_dashImpact;
    //-- UI
    private StunIndicatorUI m_UI_Stun;
    //-- Button Locks
    private bool A_Locked = false;
    private bool B_Locked = false;
    private bool X_Locked = false;
    private bool Analog_Locked = false;
    public bool bAction = false;

    public bool isALocked()
    {
        return A_Locked;
    }
    public bool isBLocked()
    {
        return B_Locked;
    }
    public bool isXLocked()
    {
        return X_Locked;
    }
    public bool isAnalogLocked()
    {
        return Analog_Locked;
    }
    public void Lock_A_Button()
    {
        A_Locked = true;
    }
    public void Lock_B_Button()
    {
        B_Locked = true;
    }
    public void Lock_X_Button()
    {
        X_Locked = true;
    }
    public void Lock_Analog()
    {
        Analog_Locked = true;
    }
    public void Unlock_A_Button()
    {
        A_Locked = false;
    }
    public void Unlock_B_Button()
    {
        B_Locked = false;
    }
    public void Unlock_X_Button()
    {
        X_Locked = false;
    }
    public void Unlock_Analog()
    {
        Analog_Locked = false;
    }
    public void Set_Name(string new_name)
    {
        if (isPlayer())
        {
            m_Player_Name = new_name;
            m_Player_Name_Text.color = GLOBAL_VALUES.COLOR_NUMBERS[m_Colour];
            m_Player_Name_Text.text = m_Player_Name;
        }
    }
    public string Get_Name()
    {
        if (isPlayer())
        {
            return m_Player_Name;
        }
        return "";
    }
    public void Hide_Name()
    {
        m_Player_Nameplate.SetActive(false);
    }

    public void Show_Name()
    {
        m_Player_Nameplate.SetActive(true);
    }
    private void Update_Name()
    {
        if (m_Colour < 0)
        {
            Set_Name("NO COL");
        }
        else if (m_HatNum < 0)
        {
            Set_Name("NO HAT");
        }
        else
        {
            Set_Name(GLOBAL_VALUES.PLAYER_NAMES_ARRAY[m_Colour, m_HatNum]);
        }
    }
    // Basic State Functions
    public void Update()
    {
        // All States
        if (Enabled())
        {
            m_Movement.Update();
            m_Actions.Do_Actions();
            if(!isAnalogLocked())
            m_Movement.Move();
            /*
            if(isPlayer())
            {
                m_Powerup_State.Update();
            }*/

            // State Specific
            switch (m_State)
            {
                case Agent_State.Active:
                    {
                        break;
                    }
                case Agent_State.Stunned:
                    {
                        m_Stun_Timer.Update();
                        m_StatCollector.Count_Stun(Time.deltaTime);
                        if (isPlayer() || isSecurityAI())
                        {
                            if (Math.Round(m_Stun_Star_Time * (m_Stun_Stars_Active),1) == Math.Round(m_Stun_Timer.Get_Time(),1))
                            {
                                try
                                {
                                    if (m_Stun_Stars[m_Stun_Stars_Active].GetComponent<ParticleSystem>() != null)
                                    {
                                        m_Stun_Stars[m_Stun_Stars_Active].GetComponent<ParticleSystem>().Play();
                                    }
                                    m_Stun_Stars[m_Stun_Stars_Active].SetActive(false);
                                    m_Stun_Stars_Active--;
                                }
                                catch
                                {
                                    Debug.Log("Failed to disable Stun Stars: Found " + m_Stun_Stars_Active + "/" + m_Stun_Stars.Count);
                                }

                                
                            }
                        }
                        if (m_Stun_Timer.isComplete())
                        {
                            Set_State(Agent_State.Active);
                        }                      
                        break;
                    }
                default:
                    {
                        throw new System.NotImplementedException("State missing in AgentManager.Update(): [" + m_State + "]");
                    }
            }
            // All States
        }
    }

    public void Set_Scale(float new_scale)
    {
        m_Instance.transform.localScale = new Vector3(new_scale, new_scale, new_scale);
    }
    private void Set_State(Agent_State new_state)
    {
        Leave_State();
        m_State = new_state;
        switch(m_State)
        {
            case Agent_State.Active:
                {
                    Unfreeze_Agent();
                    Unlock_A_Button();
                    Unlock_B_Button();
                    Unlock_X_Button();
                    Unlock_Analog();
                    if (isPlayer())
                    {
                        Set_Move_Speed(GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED);
                    }
                    else if (isSecurityAI())
                    {
                        Set_Move_Speed(GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED * 0.9f);
                    }
                    break;
                }
            case Agent_State.Stunned:
                {
                    // Notify
                    NotifySubscribers(AgentAction.GetStunned);
                    // Drop item that youre carrying
                    m_Actions.Drop(false);
                    // Stop character
                    m_Body.velocity = Vector3.zero;
                    // Change the drag and physics settings of the agent
                    m_Body.drag = GLOBAL_VALUES.PLAYER_STUNNED_DRAG;
                    m_Body.angularDrag = GLOBAL_VALUES.PLAYER_STUNNED_ANGULARDRAG;
                    m_Body.mass = GLOBAL_VALUES.PLAYER_STUNNED_MASS;
                    // Set stun animations
                    m_Animator.SetTrigger(GLOBAL_VALUES.ANIM_STUN_START);
                    m_Animator.SetBool(GLOBAL_VALUES.ANIM_STUN_LOOP, true);
                    if (m_Type == Agent_Type.Player)
                    {
                        //m_Actions.FindColourEffect("stun", m_Colour);
                        m_UI_Stun.SetStunned();
                    }
                    if(isPlayer() || isSecurityAI())
                    {
                        SetStunStars(true);
                        m_Stun_Stars_Active = m_Stun_Stars.Count-1;
                        m_Stun_Star_Time = m_Stun_Timer.Get_Time() / (m_Stun_Stars_Active);
                    }
                    //m_Instance.transform.Find("PlayerHitParticles").GetComponent<ParticleSystem>().Play();
                    if (m_Instance.CompareTag(GLOBAL_VALUES.TAG_PLAYER))
                    {
                        foreach (ParticleSystem p in m_Instance.transform.Find("GroundSlam_Hold").GetComponentsInChildren<ParticleSystem>())
                        {
                            p.Stop();
                        }
                        foreach (ParticleSystem p in m_Instance.transform.Find("GroundSlam_Charge").GetComponentsInChildren<ParticleSystem>())
                        {
                            p.Stop();
                        }
                    }
                    break;
                }
            default:
                {
                    throw new System.NotImplementedException("State missing in AgentManager.Set_State(): [" + m_State + "]");
                }
        }
    }

    public Rigidbody Get_Body()
    {
        return m_Body;
    }

    public StunIndicatorUI Get_Stun_UI()
    {
        return m_UI_Stun;
    }
    public void Heal(float heal_amount)
    {
        m_Stun_Timer.Add(heal_amount * -1);
    }
    public AudioSource Get_Audio_Source()
    {
        return m_Sound_Source;
    }
    public Agent_State Get_State()
    {
        return m_State;
    }
    public void Set_Instance(GameObject newInstance)
    {
        m_Instance = newInstance;
    }
    public void Set_Spawn_Point(GameObject newSpawn)
    {
        m_SpawnPoint = newSpawn;
    }
    public GameObject Get_Spawn_Point()
    {
        return m_SpawnPoint;
    }

    public bool isCarrying()
    {
        return m_CarriedItem != null;
    }
    private void Leave_State()
    {
        switch (m_State)
        {
            case Agent_State.Active:
                {
                    break;
                }
            case Agent_State.Stunned:
                {
                    // Reset the physics settings on the agent
                    m_Body.drag = GLOBAL_VALUES.PLAYER_ACTIVE_DRAG;
                    m_Body.angularDrag = GLOBAL_VALUES.PLAYER_ACTIVE_ANGULARDRAG;
                    m_Body.mass = GLOBAL_VALUES.PLAYER_ACTIVE_MASS;
                    m_Body.velocity = Vector3.zero;
                    // Stop the stun animation
                    m_Animator.SetBool(GLOBAL_VALUES.ANIM_STUN_LOOP, false);
                    if(isPlayer() || isSecurityAI())
                    {
                        SetStunStars(false);
                    }
                    if (isPlayer())
                    {
                        m_UI_Stun.SetNotStunned();
                    }
                    break;
                }
            default:
                {
                    throw new System.NotImplementedException("State missing in AgentManager.Leave_State(): [" + m_State + "]");
                }
        }
    }
    // Public Functions
    public bool isStunned()
    {
        return m_State == Agent_State.Stunned;
    }
    public void Stun_Agent(float duration)
    {
        //if (!m_Powerup_State.isShielded())
        //{
            if (!isStunned())
            {
                m_Stun_Timer.Reset();
                m_Stun_Timer.Add(duration, true);
                Set_State(Agent_State.Stunned);
            }
            else
            {
                float rem_time = m_Stun_Timer.Get_Time();
                if (rem_time < duration)
                {
                    m_Stun_Timer.Reset();
                    m_Stun_Timer.Add(duration, true);
                }
            }
            NotifySubscribers(AgentAction.GetStunned);
        //}
    }
    public Vector3 Get_Position()
    {
        return m_Instance.transform.position;
    }
    public Quaternion Get_Rotation()
    {
        return m_Instance.transform.rotation;
    }

    public GameObject Get_Object()
    {
        return m_Instance;
    }
    public Animator Get_Animator()
    {
        return m_Animator;
    }
    public int Get_Color()
    {
        return m_Colour;
    }
    public float Get_Strength()
    {
        return m_Strength;
    }
    public void Set_Move_Speed(float newMoveSpeed)
    {
        m_MoveSpeed = newMoveSpeed;
    }

    public float Get_Move_Speed()
    {
        return m_MoveSpeed;
    }

    public void Set_Rotate_Speed(float newRotSpeed)
    {
        m_RotateSpeed = newRotSpeed;
    }

    public float Get_Rotate_Speed()
    {
        return m_RotateSpeed;
    }
    public Agent_Type Get_Type()
    {
        return m_Type;
    }
    /*
    public AgentPowerups Get_Powerup_Manager()
    {
        return m_Powerup_State;
    }

    public void Set_Powerup_State(Powerup_Type type)
    {
        m_Powerup_State.Activate(type);
    }*/
    
    public bool isPlayer()
    {
        // is this agent a Player
        return m_Type == Agent_Type.Player;
    }
    public bool isAI()
    {
        // is this agent an AI?
        return 
            (m_Type == Agent_Type.Base_AI) || 
            (m_Type == Agent_Type.Large_AI) || 
            (m_Type == Agent_Type.Security_AI);
    }
    public bool isSecurityAI()
    {
        return m_Type == Agent_Type.Security_AI;
    }
    public GameObject GetItemCarried()
    {
        // get the object being carried
        return m_CarriedItem;
    }
    public void SetItemCarried(GameObject newItem)
    {
        m_CarriedItem = newItem;
    }
    public bool Subscribe(AgentSubscriber sub)
    {
        // Subscribe to know when this agent does an action
        if (! m_Subscribers.Contains(sub))
        {
            m_Subscribers.Add(sub);
            return true;
        }
        return false;
    }
    public bool Unsubscribe(AgentSubscriber sub)
    {
        // Stop listening for when this agent does things
        if (m_Subscribers.Contains(sub))
        {
            m_Subscribers.Remove(sub);
            return true;
        }
        return false;
    }
    public void NotifySubscribers(AgentAction action)
    {
        // Notify all subscribers you did a thing
        foreach (AgentSubscriber sub in m_Subscribers)
        {
            sub.Notify(action);
        }
    }
    public void Add_Score(int points)
    {
        // Add the -points- value to the agents score!
        m_Round_Score += points;
        NotifySubscribers(AgentAction.Score);
    }

    public int Get_Controller_Number()
    {
        return m_ControllerNumber;
    }
    public void Initialise_Agent(Agent_Type type, int cont_num = 0)
    {
        /*if (type == Agent_Type.Player && cont_num == 0)
        {
            throw new System.ArgumentException("Players cannot have a cont_num of 0: Initialise_Agent");
        }*/
        // Initialise the AgentManager with all its default values based on type
        m_ControllerNumber = cont_num;
        m_StatCollector = new StatCollector(this);
        m_Type = type;
        //m_Powerup_State = null;
        switch (type)
        {
            case Agent_Type.Player:
                // Setup a player character
                m_Input = new Controller_Input(m_ControllerNumber);
                m_Strength = GLOBAL_VALUES.BASE_PLAYER_STRENGTH;
                m_MoveSpeed = GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED;
                m_RotateSpeed = GLOBAL_VALUES.BASE_PLAYER_ROTATION_SPEED;
                m_Player_Nameplate = m_Instance.transform.Find("Player_Nameplate").gameObject;
                m_Player_Name_Text = m_Player_Nameplate.GetComponent<Text>();
                m_Instance.GetComponent<AgentAnimations>().m_manager = this;    // Set the Animation Manager
                m_Instance.GetComponent<AgentAnimations>().Setup();
                m_Instance.GetComponent<AgentAnimations>().AnimEvent_TrailStop();
                Hide_Name();
                //m_Powerup_State = new AgentPowerups(this);
                m_Instance.transform.Find("Pickup_Box").gameObject.GetComponent<PickupBox>().Add_Agent(this);
                m_BadgeAllocation = "WINDOW_SHOPPER_" + m_Player_Number;
                break;
            case Agent_Type.Base_AI:
                // Base AI are calm, rotate slowly and move at random speeds
                m_Input = new BaseAI_Control(null, this);
                int random_speed = (int)(UnityEngine.Random.Range(GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED * 0.7f, GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED * 1.2f));
                m_Strength = GLOBAL_VALUES.BASE_PLAYER_STRENGTH;
                m_MoveSpeed = random_speed;
                m_RotateSpeed = 10.0f;
                break;
            case Agent_Type.Large_AI:
                // Large AI move VERY slowly and rotate even slower
                m_Input = new LargeAI_Control(null, this);
                m_Strength = GLOBAL_VALUES.BASE_PLAYER_STRENGTH;
                m_MoveSpeed = GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED * 0.3f;
                m_RotateSpeed = 2.0f;
                break;
            case Agent_Type.Security_AI:
                // Stronger than normal, rest is average
                m_Input = new SecurityAI_Control(null, this);
                m_Instance.GetComponentInChildren<SecurityItemVision>().m_Manager = this;
                m_Instance.GetComponent<SecurityAnimations>().m_manager = this;    // Set the Animation Manager
                m_Instance.GetComponent<SecurityAnimations>().Setup();
                m_Strength = GLOBAL_VALUES.BASE_PLAYER_STRENGTH * 1.1f;
                m_MoveSpeed = GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED * 0.9f;
                m_RotateSpeed = GLOBAL_VALUES.BASE_PLAYER_ROTATION_SPEED;
                break;
            default:
                // setup a none character
                m_Input = new Input_Methods(cont_num);
                break;
        }
        m_Round_Score = 0;
        m_Round_Wins = 0;
        m_Sound_Source = m_Instance.GetComponent<AudioSource>();
        m_Body = m_Instance.GetComponent<Rigidbody>();
        m_Collision_Detector = m_Instance.GetComponent<CollisionDetection>();
        m_Animator = m_Instance.GetComponent<Animator>();
        m_State = Agent_State.Active;
        m_Enabled_State = Agent_Movement_State.Disabled;
        m_Animator.SetBool(GLOBAL_VALUES.ANIM_STUN_LOOP, false);
        m_Collision_Detector.Add_Manager(this);
        m_Actions = new AgentActions(this, m_Input);
        m_Movement = new AgentMovement(this, m_Input);
        if (isPlayer())
        {
            // Players have extra things (UI and crown)
            GameObject StunCanvas = m_Instance.transform.Find("StunCanvas").gameObject;
            GameObject StunDuration = StunCanvas.transform.Find("StunDuration").gameObject;
            m_UI_Stun = StunCanvas.GetComponent<StunIndicatorUI>();
            m_Crown = m_Instance.transform.Find("Crown").gameObject;
            m_Crown.SetActive(false);
            m_Round_Score_Position = 0;
        }
        else
        {
            m_UI_Stun = null;
        }
        if(isPlayer() || isSecurityAI())
        {
            GetStunStars();
            SetStunStars(false);
        }
        m_punchImpact = m_Instance.transform.Find("Impact_Punch").gameObject;
        m_throwImpact = m_Instance.transform.Find("Impact_Throw").gameObject;
        m_dashImpact = m_Instance.transform.Find("Impact_Dash").gameObject;
        m_Sound_Stun = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_PLAYER_HIT);
        /* FIX
        //m_Effect_Impact = m_Instance.transform.Find("Impact").gameObject;
        //m_Effect_Stun_Lights = m_Instance.transform.Find("Player_Stun").gameObject;
         */
    }
    public void Put_On_Hat(int hat_num)
    {
        // Put hat number -hat_num- on head!
        // Load hat
        GameObject new_hat = Resources.Load("Hats/HAT_" + GLOBAL_VALUES.HAT_NAMES[hat_num]) as GameObject;
        //Find head
        Transform Torso = m_Instance.transform.Find("jntTorsoLowerMain");
        Transform Upper_Torso = Torso.Find("jntTorsoUpper");
        Transform Head = Upper_Torso.Find("jntHead");
        Transform HatPoint = Head.Find("HatPoint");
        // Remove any current hat
        if (m_PlayerHat != null)
        {
            RemoveHat();
        }
        m_HatNum = hat_num;
        // Put on new hat
        m_PlayerHat = GameObject.Instantiate(new_hat, HatPoint);
        m_PlayerHat.GetComponent<HatColorManagement>().SetColor(m_Colour);
        m_PlayerHat.transform.position = HatPoint.position;
        m_PlayerHat.transform.rotation = HatPoint.rotation;
        Update_Name();
    }
    public void RemoveHat()
    {
        // Remove your current hat
        if (m_PlayerHat != null)
        {
            GameObject.Destroy(m_PlayerHat);
            m_PlayerHat = null;
            m_HatNum = -1;
        }
    }

    public int Get_Score()
    {
        return m_Round_Score;
    }

    public int Get_Player_Number()
    {
        return m_Player_Number;
    }

    public void Set_Player_Number(int pNum)
    {
        m_Player_Number = pNum;
        m_BadgeAllocation = "WINDOW_SHOPPER_" + m_Player_Number;
    }
    public void Set_Position(Vector3 newPos)
    {
        m_Instance.transform.position = newPos;
    }
    public void Set_Rotation(Quaternion newRot)
    {
        m_Instance.transform.rotation = newRot;
    }
    public int Rounds_Won()
    {
        return m_Round_Wins;
    }
    public void Zero_Score()
    {
        m_Round_Score = 0;
        m_Round_Score_Position = 0;
    }
    public void Zero_Rounds_Won()
    {
        m_Round_Wins = 0;
    }
    public void Give_Round_Win()
    {
        m_Round_Wins += 1;
    }
    public void Update_Color(int new_col_num)
    {
        // Update agents materials to match the appropriate new color number
        m_Colour = new_col_num;
        Material t_mat_body = Resources.Load("Materials\\Players\\PlayerMat_" + GLOBAL_VALUES.COLOR_NAMES[m_Colour] + "_Cel") as Material;
        Material t_mat_head = Resources.Load("Materials\\Players\\PlayerMat_" + GLOBAL_VALUES.COLOR_NAMES[m_Colour] + "_Cel_Head") as Material;
        m_Instance.transform.Find("objPlayerCharacter").Find("objPlayerBody").GetComponent<SkinnedMeshRenderer>().material = t_mat_body;
        m_Instance.transform.Find("objPlayerCharacter").Find("objPlayerHead").GetComponent<SkinnedMeshRenderer>().material = t_mat_head;
        m_Instance.transform.Find("objPlayerCharacter").Find("Boxing_Glove_L_High1").GetComponent<SkinnedMeshRenderer>().material = t_mat_body;
        m_Instance.transform.Find("objPlayerCharacter").Find("Boxing_Glove_L_High2").GetComponent<SkinnedMeshRenderer>().material = t_mat_body;
        Update_Name();
    }
    
    public void Drop(bool dropped = true)
    {
        m_Actions.Drop(dropped);
    }

    public void Throw()
    {
        m_Actions.ThrowMechanic();
    }

    public void Give_Badge(string new_badge)
    {
        m_BadgeAllocation = new_badge;
    }

    public string Get_Badge()
    {
        return m_BadgeAllocation;
    }
    public bool isDashing()
    {
        return m_Actions.isDashing();
    }
    public Input_Methods Get_Input()
    {
        return m_Input;
    }
    public bool Enabled()
    {
        return m_Enabled_State == Agent_Movement_State.Enabled;
    }

    public void Enable()
    {
        m_Enabled_State = Agent_Movement_State.Enabled;
        m_Body.constraints = ENABLED_CONSTRAINTS;
    }

    public void Disable()
    {
        m_Enabled_State = Agent_Movement_State.Disabled;
        m_Body.constraints = DISABLED_CONSTRAINTS;
    }

    public void Reset()
    {
        m_StatCollector.Reset();
        Zero_Score();
        m_Actions.Drop(false);
        m_Instance.transform.position = m_SpawnPoint.GetComponent<SpawnZone>().Get_Unique_Random_Spawn_Point();
        m_Instance.transform.rotation = m_SpawnPoint.transform.rotation;
        GameObject pickup_box = m_Instance.transform.Find("Pickup_Box").gameObject;
        PickupBox box = pickup_box.GetComponent<PickupBox>();
        m_Body.velocity = Vector3.zero;
        box.items_in_range.Clear();
        m_BadgeAllocation = "WINDOW_SHOPPER_" + m_Player_Number;
    }

    public void Pause()
    {
        m_Animator.enabled = false;
        //m_Powerup_State.Pause();
        Disable();
    }

    public void Resume()
    {
        m_Animator.enabled = true;
        //m_Powerup_State.Resume();
        Enable();
    }

    public void Set_Score_Position(int pos)
    {
        m_Round_Score_Position = pos;
        if (m_Round_Score_Position == 1)
        {
            m_Crown.SetActive(true);
        }
        else
        {
            m_Crown.SetActive(false);
        }
    }

    public void Play_Impact_Particles(int colour)
    {
        //Create Background Color
        ParticleSystem bg = m_Effect_Impact.transform.Find("Background Explosion").GetComponent<ParticleSystem>();
        ParticleSystem.MinMaxGradient chosen_color = new ParticleSystem.MinMaxGradient();
        chosen_color.mode = ParticleSystemGradientMode.Color;
        chosen_color.color = GLOBAL_VALUES.IMPACT_COLOURS_BACKGROUND[colour];
        ParticleSystem.MainModule ps_m = bg.main;
        ps_m.startColor = chosen_color;

        //Create Foreground Color
        ParticleSystem fg = m_Effect_Impact.transform.Find("Foreground Explosion").GetComponent<ParticleSystem>();
        chosen_color = new ParticleSystem.MinMaxGradient();
        chosen_color.mode = ParticleSystemGradientMode.Color;
        chosen_color.color = GLOBAL_VALUES.IMPACT_COLOURS_FOREGROUND[colour];
        ps_m = fg.main;
        ps_m.startColor = chosen_color;


        foreach (ParticleSystem p in m_Effect_Impact.GetComponentsInChildren<ParticleSystem>())
        {
            p.Play();
        }
    }

    public void AddForce(float time, Vector3 newForce)
    {
        //if(m_Powerup_State != null && (! m_Powerup_State.isShielded()))
        //{
            m_Movement.Add_External_Force(new ExternalForce(time, newForce));
            GameObject.Find("GameManagerObject").GetComponent<GameManager>().Get_MatchManager().Get_RoundManager().Get_Camera_Manager().AddForce(time, newForce);
        //}
    }

    public void Wake_Up()
    {
        m_Stun_Timer.Reset();
        Set_State(Agent_State.Active);
    }

    private void SetStunStars(bool b)
    {
        foreach (GameObject g in m_Stun_Stars)
        {
            g.SetActive(b);
        }
    }

    private void GetStunStars()
    {
        m_Stun_Stars = new List<GameObject>();
        Transform t = m_Instance.transform.Find("StunContainer/StunStars");
        foreach (Transform child in t)
        {
            //child of transform
            m_Stun_Stars.Add(child.gameObject);
        }
    }

    /*
    public bool has_active_powerup()
    {
        return m_Powerup_State.Get_State() != AgentPowerups.PowerupState.None;
    }*/

    public void Setup_Item_Manager(ItemManager im)
    {
        m_Actions.Setup_Item_Manager(im);
    }

    public AgentPunchControl GetPunchControl()
    {
        return m_Actions.m_PunchControl;
    }

    public void ImpactParticles(string type, Vector3 angle)
    {

        switch (type)
        {
            case "punch":
                {
                    m_punchImpact.transform.eulerAngles = angle;
                    foreach (ParticleSystem p in m_punchImpact.GetComponentsInChildren<ParticleSystem>())
                    {
                        p.Play();
                    }
                    break;
                }
            case "dash":
                {
                    m_dashImpact.transform.eulerAngles = angle;
                    foreach (ParticleSystem p in m_dashImpact.GetComponentsInChildren<ParticleSystem>())
                    {
                        p.Play();
                    }
                    break;
                }
            case "throw":
                {
                    m_throwImpact.transform.eulerAngles = angle;
                    foreach (ParticleSystem p in m_throwImpact.GetComponentsInChildren<ParticleSystem>())
                    {
                        p.Play();
                    }
                    break;
                }
            default:
                {
                    m_punchImpact.transform.eulerAngles = angle;
                    foreach (ParticleSystem p in m_punchImpact.GetComponentsInChildren<ParticleSystem>())
                    {
                        p.Play();
                    }
                    break;
                }
		}
	}

    public void Freeze_Agent()
    {
        Debug.Log("Freezing [" + m_Instance.name + "]");
        m_Body.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        Lock_B_Button();
        Lock_A_Button();
        Lock_X_Button();
        Lock_Analog();
    }

    public void Unfreeze_Agent()
    {
        Debug.Log("Unfreezing [" + m_Instance.name + "]");
        m_Body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        Unlock_B_Button();
        Unlock_A_Button();
        Unlock_X_Button();
        Unlock_Analog();
    }

    public void Set_Default_State()
    {
        Get_Animator().Rebind();
        Get_Animator().Play("Idle", 0);

        foreach (ParticleSystem p in m_Instance.GetComponentsInChildren<ParticleSystem>())
        {
            p.Stop();
        }
    }

    public void PunchMechanic()
    {
        m_Actions.m_PunchControl.Punch_Mechanic(GLOBAL_VALUES.KNOCKBACK_PUNCH_TWO);
    }

    public bool isGroundSlamming()
    {
        return m_Actions.m_PunchControl.inGroundSlam();
    }
}