using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityAI_Control : Input_Methods {
    private Pathing_Point m_target = null;
    public Item m_Last_Collided_Item = null;
    public AgentManager m_Last_Collided_Agent = null;
    public GameObject m_Target_Item = null;
    private SecurityItemVision m_ItemVision;
    private PickupBox m_PickupBox;
    private AgentManager m_Manager;
    private Pathing_Manager m_Path_Manager;
    private Path m_Path;
    private Vector3 previous_position;
    private Timer movement_timer;
    private Timer heal_timer;
    private GameObject m_Alert_Object; 
    private bool m_Agro_Flag = false;
    private bool A_Pressed = false;
    private bool B_Pressed = false;
    private bool B_Released = false;
    private bool X_Pressed = false;
    private bool Y_Pressed = false;
    private bool B_InUse = false;
    private Timer m_PunchTimer = null;
    public bool curr_attacking = false;

    private AgentManager m_Enemy_Agent;
    private Transform m_Enemy_Position;
    private ParticleSystem m_ChaseEffect;

    private enum Security_State { Patrol, Catch, Chase, Attack };
    private Security_State m_State;
    private bool Movement_Watchdog = false;
    public bool Stun_Flag = false;
    private void Check_Pathing()
    {
        if (m_target == null)
        {
            Get_Target();
        }
        if (m_Path == null)
        {
            Calculate_Path();
        }
    }
    public void Agro(AgentManager target)
    {
        m_Agro_Flag = true;
        m_Last_Collided_Agent = target;
    }
    private void Calculate_Path()
    {
        if (m_target != null)
        {
            m_Path = m_Path_Manager.Find_Path(m_Manager.Get_Position(), m_target.m_Position.position);
        }
    }

    private void Check_State()
    {
        if (m_PunchTimer != null)
        {
            m_PunchTimer.Update();
        }
        Vector3 my_pos = m_Manager.Get_Position();
        // in any state
        // check for being stunned
        if ((m_Manager.isStunned() || m_Agro_Flag == true) && Stun_Flag == false)
        {
            if(m_Last_Collided_Agent != null)
            {
                m_Enemy_Agent = m_Last_Collided_Agent;
            }
            else if (m_Last_Collided_Item != null)
            {
                m_Enemy_Agent = m_Last_Collided_Item.prev_owner;
            }
            if (m_Enemy_Agent == m_Manager)
            {
                m_Enemy_Agent = null;
            }
            if (m_Enemy_Agent != null)
            {
                Stun_Flag = true;
                m_Enemy_Position = m_Enemy_Agent.Get_Object().transform;
                Set_State(Security_State.Chase);
            }
        }

        switch (m_State)
        {
            // check for transition conditions
            case Security_State.Patrol:
                {
                    // check for incoming items
                    if(m_ItemVision.NeedToCatch())
                    {
                        Set_State(Security_State.Catch);
                    }
                    break;
                }
            case Security_State.Catch:
                {
                    // If unsure which item is most dangerous
                    if(m_Target_Item == null)
                    {
                        m_Target_Item = m_ItemVision.Get_Best_Item();
                    }
                    // if we no longer need to catch anything go back to patroling
                    if ((! m_ItemVision.NeedToCatch()) && (m_Manager.GetItemCarried() != m_Target_Item))
                    {
                        Set_State(Security_State.Patrol);
                    }
                    // if we are currently holding something
                    if(m_Manager.isCarrying())
                    {
                        // currently carrying item which isnt the danger
                        if(m_Manager.GetItemCarried() != m_Target_Item)
                        {
                            A_Pressed = true; // throw current item to make room for new item
                        }
                        else
                        {
                            // item we are carrying is the dangered item, chase the aggressor
                            AgentManager prev_owner = m_Manager.GetItemCarried().GetComponent<Item>().prev_owner;
                            if (prev_owner != m_Manager && prev_owner != null)
                            {
                                m_Enemy_Agent = m_Manager.GetItemCarried().GetComponent<Item>().prev_owner;
                                m_Enemy_Position = m_Enemy_Agent.Get_Object().transform;
                                Set_State(Security_State.Chase);
                            }
                            else
                            {
                                // Drop item and patrol
                                A_Pressed = true;
                                Set_State(Security_State.Patrol);
                            }
                            
                        }
                    }
                    break;
                }
            case Security_State.Chase:
                {
                    if(m_Enemy_Agent != null)
                    {
                        m_Enemy_Agent.m_StatCollector.Count_Security_Chase_Time(Time.deltaTime);
                        GameObject l_Enemy_Obj = m_Enemy_Agent.Get_Object();
                        Vector3 l_Enemy_Position = m_Enemy_Agent.Get_Position();
                        // if enemy is within range and LOS
                        m_Enemy_Agent.Get_Object().layer = 0;
                        RaycastHit hit;
                        if ((
                            (Physics.Raycast(my_pos, l_Enemy_Position - my_pos, out hit)) &&
                            (hit.transform.gameObject == l_Enemy_Obj)) &&
                            ((Vector3.Distance(my_pos,l_Enemy_Position) <= GLOBAL_VALUES.SECURITY_THROWBACK_DISTANCE))
                            )
                        {
                            Set_State(Security_State.Attack);
                        }
                        l_Enemy_Obj.layer = 2;
                        // if enemy gets stunned go back to patrol
                        if (m_Enemy_Agent.isStunned())
                        {
                            m_Enemy_Agent = null;
                            m_Enemy_Position = null;
                            Set_State(Security_State.Patrol);
                        }
                    }
                    else
                    {
                        Set_State(Security_State.Patrol);
                    }
                    break;
                }
            case Security_State.Attack:
                {
                    m_Enemy_Agent.m_StatCollector.Count_Security_Chase_Time(Time.deltaTime);
                    GameObject l_Enemy_Obj = m_Enemy_Agent.Get_Object();
                    Vector3 l_Enemy_Position = m_Enemy_Agent.Get_Position();
                    // if enemy is too far away or no longer in LOS return to state
                    l_Enemy_Obj.layer = 0;
                    RaycastHit hit;
                    if (!(
                        (Physics.Raycast(my_pos, l_Enemy_Position - my_pos, out hit)) &&
                        (hit.transform.gameObject == l_Enemy_Obj)) ||
                        (!(Vector3.Distance(my_pos, m_Enemy_Position.position) <= GLOBAL_VALUES.SECURITY_THROWBACK_DISTANCE))
                        )
                    {
                        Set_State(Security_State.Chase);
                    }
                    l_Enemy_Obj.layer = 2;
                    // if enemy gets stunned go back to patrol
                    if (m_Enemy_Agent.isStunned())
                    {
                        m_Enemy_Agent = null;
                        m_Enemy_Position = null;
                        Set_State(Security_State.Patrol);
                    }
                    break;
                }
        }
    }

    private void Leave_State()
    {
        switch (m_State)
        {
            // code transitions out of current state
            case Security_State.Patrol:
                {
                    m_Path = null;
                    m_target = null;
                    break;
                }
            case Security_State.Catch:
                {
                    m_Target_Item = null;
                    break;
                }
            case Security_State.Chase:
                {
                    m_Alert_Object.SetActive(false);
                    break;
                }
            case Security_State.Attack:
                {
                    m_Alert_Object.SetActive(false);
                    m_PunchTimer = null;
                    B_InUse = false;
                    B_Pressed = false;
                    B_Released = true;
                    curr_attacking = false;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    private void Set_State(Security_State new_state)
    {
        Leave_State();
        m_State = new_state;
        switch (m_State)
        {
            // code transitions out of current state
            case Security_State.Patrol:
                {
                    m_Enemy_Agent = null;
                    m_Enemy_Position = null;
                    m_Alert_Object.SetActive(false);
                    m_ChaseEffect.Stop();
                    break;
                }
            case Security_State.Catch:
                {
                    m_Enemy_Agent = null;
                    m_Enemy_Position = null;
                    m_Alert_Object.SetActive(false);
                    m_ChaseEffect.Stop();
                    break;
                }
            case Security_State.Chase:
                {
                    m_Alert_Object.SetActive(true);
                    m_Alert_Object.transform.Find("Security_Alert_Background").GetComponent<SpriteRenderer>().color = GLOBAL_VALUES.COLOR_NUMBERS[m_Enemy_Agent.Get_Color()];
                    m_ChaseEffect.Play();
                    break;
                }
            case Security_State.Attack:
                {
                    m_Alert_Object.SetActive(true);
                    m_Alert_Object.transform.Find("Security_Alert_Background").GetComponent<SpriteRenderer>().color = GLOBAL_VALUES.COLOR_NUMBERS[m_Enemy_Agent.Get_Color()];
                    break;
                }
        }
    }
    public void Recalculate()
    {
        m_target = null;
        m_Path = null;
        Movement_Watchdog = false;
        switch (m_State)
        {
            case Security_State.Patrol:
                {
                    m_target = m_Path_Manager.Get_Random_Node(null);
                    break;
                }
            case Security_State.Chase:
                {
                    if (m_Enemy_Agent != null && m_Enemy_Position != null)
                    {
                        m_target = m_Path_Manager.Get_Nearest_Node(m_Enemy_Position.position);
                    }
                    break;
                }
        }
        if(m_target != null)
        {
            Calculate_Path();
            Check_Pathing();
        }
    }

    public override float Get_Requested_Rotation() {
        float ret = 0.0f;
        Vector3 my_pos = m_Manager.Get_Position();
        float Joystick_X = 0;
        float Joystick_Y = 0;
        switch (m_State)
        {
            case Security_State.Patrol:
                {
                    try
                    {
                        Vector3 node_target = m_Path.Current_Point().m_Position.transform.position;
                        Joystick_X = node_target.x - my_pos.x;
                        Joystick_Y = node_target.z - my_pos.z;
                    }
                    catch
                    { Check_Pathing(); }
                    break;
                }
            case Security_State.Catch:
                {
                    try
                    {
                        if (m_Target_Item != null)
                        {
                            Joystick_X = m_Target_Item.transform.position.x - my_pos.x;
                            Joystick_Y = m_Target_Item.transform.position.z - my_pos.z;
                        }
                        else
                        {
                            if (m_ItemVision.NeedToCatch())
                            {
                                m_Target_Item = m_ItemVision.Get_Best_Item();
                            }
                        }
                    }
                    catch
                    { Debug.Log("State: CATCH || Rotation Calculation"); }
                    break;
                }
            case Security_State.Chase:
                {
                    try
                    {
                        Vector3 node_target = m_Path.Current_Point().m_Position.transform.position;
                        Joystick_X = node_target.x - my_pos.x;
                        Joystick_Y = node_target.z - my_pos.z;
                    }
                    catch
                    { Check_Pathing(); }
                    break;
                }
            case Security_State.Attack:
                {
                    try
                    {
                        if (m_Enemy_Agent != null && m_Enemy_Position != null)
                        {
                            Joystick_X = m_Enemy_Position.position.x - my_pos.x;
                            Joystick_Y = m_Enemy_Position.position.z - my_pos.z;
                        }
                        else
                        {
                            if (m_Last_Collided_Agent != null)
                            {
                                m_Enemy_Agent = m_Last_Collided_Agent;
                                m_Enemy_Position = m_Enemy_Agent.Get_Object().transform;
                                m_Last_Collided_Agent = null;
                            }
                            else if (m_Last_Collided_Item != null)
                            {
                                m_Enemy_Agent = m_Last_Collided_Item.prev_owner;
                                m_Enemy_Position = m_Enemy_Agent.Get_Object().transform;
                                m_Last_Collided_Item = null;
                            }
                        }
                    }
                    catch
                    { Debug.Log("State: ATTACK || Rotation Calculation"); }
                    break;
                }
        }
        if ((Joystick_X != 0) || (Joystick_Y != 0))
        {
            ret = (Custom_Math_Utils.nfmod(-(((Mathf.Atan2(Joystick_Y, Joystick_X)) * Mathf.Rad2Deg) - 90), 360));
            ret = (ret < 0) ? ret + 360 : ret;
        }
        return ret;
    }
    public override float Get_Requested_Magnitude() {
        switch (m_State)
        {
            case Security_State.Patrol: return 0.1f;
            case Security_State.Catch: return 0.01f;
            case Security_State.Chase: return 1f;
            case Security_State.Attack: return 1f;
            default: return 1f;
        }
    }

    private void Update_Movement_Watchdog()
    {
        Vector3 curr_pos = m_Manager.Get_Position();
        if (movement_timer.isComplete())
        {
            if ((previous_position != new Vector3()) && (Vector3.Distance(curr_pos, previous_position) < GLOBAL_VALUES.AI_TARGET_RADIUS))
            {
                Movement_Watchdog = true;
            }
            previous_position = curr_pos;
            movement_timer.Add(1, true);
        }
    }
    private void Calculate_Actions()
    {
        switch (m_Manager.Get_State())
        {
            case Agent_State.Active:
                {
                    switch (m_State)
                    {
                        case Security_State.Catch:
                            {
                                if (m_Target_Item != null)
                                {
                                    if (! m_Manager.isCarrying())
                                    {
                                        if (m_PickupBox.items_in_range.Contains(m_Target_Item))
                                        {
                                            A_Pressed = true; // Pickup item
                                        }
                                    }
                                    else
                                    {
                                        // already carrying something
                                        A_Pressed = true; // Throw current item
                                    }
                                }
                                else
                                {
                                    m_Target_Item = m_ItemVision.Get_Best_Item();
                                }
                                break;
                            }
                        case Security_State.Attack:
                            {
                                if (!curr_attacking)
                                {
                                    // If facing enemy attack
                                    RaycastHit hit;
                                    m_Enemy_Agent.Get_Object().layer = 0;
                                    if (// raycast forward to see if we hit the enemy
                                        (Physics.Raycast(m_Manager.Get_Position(), m_Manager.Get_Object().transform.forward, out hit)) &&
                                        (hit.transform.gameObject == m_Enemy_Agent.Get_Object().transform.gameObject)
                                        )
                                    {
                                        // enemy is forward
                                        if (m_Manager.isCarrying())
                                        {
                                            // throw item at enemy
                                            A_Pressed = true;
                                        }
                                        else
                                        {
                                            float dist = Vector3.Distance(m_Manager.Get_Position(), m_Enemy_Agent.Get_Position());
                                            if (dist <= GLOBAL_VALUES.SECURITY_PUNCH_DISTANCE)
                                            {
                                                if (m_PunchTimer != null && m_PunchTimer.isComplete())
                                                {
                                                    B_InUse = false;
                                                    B_Released = true;
                                                    m_PunchTimer = null;
                                                    curr_attacking = true;
                                                }
                                                else
                                                {
                                                    if (m_PunchTimer == null)
                                                    {
                                                        // the timer goes for slightly longer than the charge time, to ensure that ordering is not necessary
                                                        m_PunchTimer = new global::Timer(GLOBAL_VALUES.GROUNDSLAM_CHARGE_TIME / 5, true);
                                                        B_Pressed = true;
                                                    }
                                                    B_InUse = true;
                                                }
                                            }
                                        }
                                    }
                                    m_Enemy_Agent.Get_Object().layer = 2;
                                }
                                break;
                            }
                    }
                    break;
                }
            case Agent_State.Stunned:
                {
                    if (heal_timer.isComplete())
                    {
                        A_Pressed = true;
                        heal_timer.Add(GLOBAL_VALUES.SECURITY_HEAL_DELAY, true);
                    }
                    break;
                }
        }
        
    }

    private void Calculate_Movement()
    {
        switch (m_State)
        {
            case Security_State.Patrol:
                {
                    try
                    {
                        if (Vector3.Distance(m_Manager.Get_Position(), m_Path.Current_Point().m_Position.position) < 1)
                        {
                            // have arrived at the current target point
                            m_Path.Increment_Point();
                            if (m_Path.isFinished())
                            {
                                Get_Target();
                            }
                        }
                        if (Movement_Watchdog == true)
                        {
                            // stopped moving so recalculate path
                            Recalculate();
                        }
                    }
                    catch
                    {
                        Check_Pathing();
                    }
                    break;
                }
            case Security_State.Chase:
                {
                    if (m_Path != null && m_Path.Current_Point() != null)
                    {
                        if (Vector3.Distance(m_Manager.Get_Position(), m_Path.Current_Point().m_Position.position) < 1)
                        {
                            // have arrived at the current target point
                            m_Path.Increment_Point();
                            if (m_Path.isFinished())
                            {
                                Get_Target();
                            }
                        }
                        if (Movement_Watchdog == true)
                        {
                            // stopped moving so recalculate path
                            Recalculate();
                        }
                    }
                    else
                    {
                        Check_Pathing();
                    }
                    break;
                }
        }
    }

    private void Get_Target()
    {
        if (m_target == null || m_Path == null || Movement_Watchdog == true || m_Path.isFinished())
        {
            Recalculate();
        }
    }

    public SecurityAI_Control(int? p_num, AgentManager manager) : base(p_num)
    {
        movement_timer = new global::Timer(1, true);
        heal_timer = new Timer(GLOBAL_VALUES.SECURITY_HEAL_DELAY, true);
        m_Path_Manager = GameObject.FindGameObjectWithTag(GLOBAL_VALUES.TAG_AI_MANAGER).GetComponent<Pathing_Manager>();
        m_Manager = manager;
        m_ItemVision = m_Manager.Get_Object().transform.Find("Item_Vision").GetComponent<SecurityItemVision>();
        m_PickupBox = m_Manager.Get_Object().GetComponentInChildren<PickupBox>();
        m_State = Security_State.Patrol;
        m_Alert_Object = m_Manager.Get_Object().transform.Find("Security_Alert").gameObject;
        m_ChaseEffect = m_Manager.Get_Object().transform.Find("Angry").gameObject.GetComponent<ParticleSystem>();
        m_Alert_Object.SetActive(false);
    }

    public override void Update()
    {
        movement_timer.Update();
        heal_timer.Update();
        Update_Movement_Watchdog();
        Check_State();
        Calculate_Actions();
        Calculate_Movement();
    }

    public override void Reset_Pressed() {
        A_Pressed = false;
        B_Pressed = false;
        X_Pressed = false;
        Y_Pressed = false;
    }

    public override bool Get_A_Pressed() {
        if(A_Pressed)
        {
            A_Pressed = false;
            return true;
        }
        return false;
    }

    public override bool Get_B_Pressed()
    {
        if (B_Pressed)
        {
            B_Pressed = false;
            return true;
        }
        return false;
    }

    public override bool Get_B_Held()
    {
        return B_InUse;
    }

    public override bool Get_B_Released()
    {
        if (B_Released)
        {
            B_Released = false;
            return true;
        }
        return false;
    }

    public override bool Get_X_Pressed()
    {
        if (X_Pressed)
        {
            X_Pressed = false;
            return true;
        }
        return false;
    }

    public override bool Get_Y_Pressed()
    {
        if (Y_Pressed)
        {
            Y_Pressed = false;
            return true;
        }
        return false;
    }
}
