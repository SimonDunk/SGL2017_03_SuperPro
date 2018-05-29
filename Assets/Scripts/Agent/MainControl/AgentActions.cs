using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AgentActions
{
    // Dash
    private Timer m_DashCooldown = new Timer();
    private GameObject m_DashObject;
    private Timer m_DashEffectTimer = new Timer();
    private bool m_DashCooldown_Applied = false;
    private float m_AgentDefaultRotate;
    private float m_AgentDefaultMove;

    public AgentManager m_manager;
    public Input_Methods m_input;
    public Rigidbody m_body;

    //Sound
    public AudioClip pickup;
    public AudioClip drop;
    public AudioClip throwSound;
    public AudioClip m_DashSound;
    private AudioSource source;

    //UI
    public DashCooldownUI dashUI;
    private GameObject pickup_drop_effect;
    private GameObject colouredStunEffect;

    //Combat
    public AgentPunchControl m_PunchControl = null;

    public AgentActions(AgentManager manager, Input_Methods input)
    {
        m_manager = manager;
        m_input = input;
        m_body = m_manager.Get_Body();
        m_AgentDefaultRotate = m_manager.Get_Rotate_Speed();
        //Sound
        source = m_manager.Get_Audio_Source();
        pickup = Resources.Load<AudioClip>("Sound/snd_Pickup");
        drop = Resources.Load<AudioClip>("Sound/snd_Drop");
        throwSound = Resources.Load<AudioClip>("Sound/snd_PlayerThrow");

        //UI
        dashUI = m_manager.Get_Object().GetComponentInChildren<DashCooldownUI>();
        if (m_manager.isPlayer() || m_manager.isSecurityAI())
        {
            m_DashObject = m_manager.Get_Object().transform.Find("Player_Motion").gameObject;
            m_DashObject.SetActive(false);
        }
        pickup = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_PICKUP);
        drop = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_DROP);
        throwSound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_THROW);
        m_DashSound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_PLAYER_DASH);

        //Combat
        if (m_manager.isPlayer() || m_manager.isSecurityAI())
        {
            m_PunchControl = new AgentPunchControl(m_manager);
        }
    }

    public void Setup_Item_Manager(ItemManager im)
    {
        m_PunchControl.Setup_Item_Manager(im);
    }
    
    private void A_Action(Agent_State state)
    {
        if (!m_manager.isStunned())
        {
            if(!m_manager.isALocked())
            {
                if (!Pickup())
                {
                    if (Throw())
                    {
                        //Sound
                        m_manager.m_StatCollector.Count_Throw();
                        //source.PlayOneShot(throwSound);
                        m_manager.NotifySubscribers(AgentAction.Throw);
                    }
                    else
                    {
                        m_manager.m_StatCollector.Count_A_Miss();
                    }
                }
                else
                {
                    m_manager.m_StatCollector.Count_Pickup();
                }
            }
        }
        else
        {                
            m_manager.m_StatCollector.Count_Heal();
            if (m_manager.isPlayer())
            {
                m_manager.Get_Stun_UI().A_Pressed();
            }
            Heal();
        }
        
    }

    private void B_Action(Agent_State state)
    {
        if ((m_manager.isPlayer() && !m_manager.isBLocked()) && !m_manager.bAction || m_manager.isSecurityAI())
        {
            if ((!m_manager.isStunned()) && (!m_manager.isCarrying()))
            {
                m_PunchControl.Report_Button_State(m_input.Get_B_Pressed(), m_input.Get_B_Held(), m_input.Get_B_Released());
            }
        }
    }

    private void X_Action(Agent_State state)
    {
        if ((!m_manager.isStunned()) && (!m_manager.isXLocked()))
        {
            if (Dash())
            {
                m_manager.m_StatCollector.Count_Dash();
                m_manager.NotifySubscribers(AgentAction.Dash);
            }
            else
            {
                m_manager.m_StatCollector.Count_X_Miss();
            }
        }
        else
        {
            m_manager.m_StatCollector.Count_X_Miss();
        }
    }

    private void Y_Action(Agent_State state)
    {
        // WHY IS IT COMMENTED?
        // The decision has been made to no longer allow players to "Drop" items,
        // instead they must throw them. The extra ability to drop rather than throw
        // is taken out, if it is ever requested to happen again, then we can just uncomment this.
        // Leave this here incase that ever occurs.
        /*if (!m_manager.isStunned())
        {
            if (Drop())
            {
                m_manager.m_StatCollector.Count_Drop();
            }
            else
            {
                m_manager.m_StatCollector.Count_Y_Miss();
            }
        }
        else
        {
            m_manager.m_StatCollector.Count_Y_Miss();
        }*/
    }

    private void Start_Action(Agent_State state)
    {
        // Pause and unpause
        m_manager.m_StatCollector.Count_Pause();
        PauseMenu.pausedPlayer = m_manager;
        PauseMenu.isPaused = true;
    }
    public void Do_Actions()
    {
        if (m_PunchControl != null)
        {
            m_PunchControl.Update();
        }
        m_input.Update(); // get current button states
        UpdateTimers();
        Agent_State state = m_manager.Get_State();
        if (m_input.Get_A_Pressed())
        {
            A_Action(state);
        }
        else if (m_input.Get_A_Released())
        {
            if(m_manager.isStunned())
            {
                if (m_manager.isPlayer())
                {
                    m_manager.Get_Stun_UI().A_Released();
                }
            }
        }
        B_Action(state);
        if (m_input.Get_X_Pressed())
        {
            X_Action(state);
        }
        if (m_input.Get_Y_Pressed())
        {
            Y_Action(state);
        }
        if (m_input.Get_Start_Pressed())
        {
            Start_Action(state);
        }
    }

    private bool Heal()
    {
        try
        {
            m_manager.Heal(GLOBAL_VALUES.PLAYER_STUNNED_A_HEAL);
            m_manager.Get_Object().transform.Find("Heal_Effect").GetComponent<ParticleSystem>().Emit(1);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public bool Pickup()
    {
        GameObject pickup_box = m_manager.Get_Object().transform.Find("Pickup_Box").gameObject;
        PickupBox box = pickup_box.GetComponentInChildren<PickupBox>();
        GameObject closest = box.Get_Closest_Object();
        if ((!m_manager.isCarrying()) && (!m_input.Get_B_Held()) && (closest != null) && (closest.GetComponent<Item>().GetItemState() != ItemState.carried))
        {
            m_manager.Get_Animator().SetTrigger(GLOBAL_VALUES.ANIM_TRIGGER_PICKUP);

            //removing collisions
            foreach(Collider c in closest.GetComponents<Collider>())
            {
                c.isTrigger = true;
            }
            foreach (Rigidbody rb in closest.GetComponents<Rigidbody>())
            {
                rb.detectCollisions = true;
                rb.isKinematic = true;
            }

            //Find Carry Point
            GameObject carry_point = null;
            if (m_manager.Get_Object().CompareTag(GLOBAL_VALUES.TAG_PLAYER))
            {
                GameObject findObj = m_manager.Get_Object().transform.Find("jntTorsoLowerMain").gameObject;
                findObj = findObj.transform.Find("jntTorsoUpper").gameObject;
                findObj = findObj.transform.Find("jntHand_R").gameObject;
                findObj = findObj.transform.Find("Carry_Point").gameObject;
                carry_point = findObj;
            }
            else
            {
                carry_point = m_manager.Get_Object().transform.Find("Carry_Point").gameObject;
            }

            closest.transform.parent = carry_point.transform; //set pickup a child of player
            closest.transform.position = carry_point.transform.position;
            closest.transform.rotation = carry_point.transform.rotation;
            closest.transform.localScale = new Vector3(1, 1, 1);
            closest.GetComponent<Item>().SetState(ItemState.carried);
            closest.GetComponent<Item>().impactFirstHit = true;
            closest.transform.localScale = closest.GetComponent<Item>().scale;
            source.PlayOneShot(pickup);
            Animator a = closest.GetComponent<Animator>();
            if (a != null)
            {
                a.Play("Idle", -1, 0);
                a.StopPlayback();
            }
            //FindColourEffect("p/d", m_manager.Get_Color());
            m_manager.SetItemCarried(closest);
            return true;
        }
        return false;
    }
    
    public bool Drop(bool was_dropped = true)
    {
        GameObject carried = m_manager.GetItemCarried();
        if (carried != null)
        {
            carried.transform.parent = null;
            carried.GetComponent<Collider>().isTrigger = false;
            carried.GetComponent<Rigidbody>().detectCollisions = true;
            carried.GetComponent<Rigidbody>().isKinematic = false;
            carried.GetComponent<Item>().prev_owner = m_manager;
            carried.GetComponent<Item>().SetState(ItemState.dropped);
            carried.transform.localScale = carried.GetComponent<Item>().scale;
            if (was_dropped)
            {
                m_manager.Get_Animator().SetTrigger(GLOBAL_VALUES.ANIM_TRIGGER_DROP);
                m_manager.GetItemCarried().GetComponent<Item>().SetState(ItemState.dropped);
            }
            m_manager.SetItemCarried(null);
            return true;
         }
        return false;
    }

    public bool Throw()
    {
        if (m_manager.isCarrying())
        {
            m_manager.Get_Animator().SetTrigger(GLOBAL_VALUES.ANIM_THROW);
            return true;
        }
        return false;
    }

    public void ThrowMechanic()
    {
        // Throw an object being carried (if any)
        // in the direction forward of the agent with a force
        // equal to the agents strength * throw multiplier + agent speed
        if (m_manager.isCarrying())
        {
            // Get the item to be effected
            GameObject item = m_manager.GetItemCarried();
            // Drop it (so it is no longer being carried)
            Drop(false);
            // Apply thrown effects to it.
            item.GetComponent<Item>().SetState(ItemState.thrown);
            item.GetComponent<Item>().motionEffects.SetActive(true);
            SetMotionTrailColours(item.GetComponent<Item>().motionEffects, m_manager.Get_Color());
            Rigidbody rb = item.GetComponent<Rigidbody>();
            rb.AddForce(m_manager.Get_Object().transform.forward * 
                ((m_manager.Get_Strength() * GLOBAL_VALUES.DEFAULT_THROW_MULTIPLIER) + m_manager.Get_Body().velocity.magnitude));
        }
    }

    public bool isDashing()
    {
        return !m_DashEffectTimer.isComplete();
    }

    public bool Dash()
    {
        if (m_DashCooldown.isComplete() && m_DashEffectTimer.isComplete())
        {
            m_DashObject.SetActive(true);
            m_DashEffectTimer.Reset();
            m_DashEffectTimer.Add(GLOBAL_VALUES.DASH_DURATION, true);
            SetMotionTrailColours(m_DashObject, m_manager.Get_Color());
            m_manager.Set_Rotate_Speed(m_manager.Get_Rotate_Speed() * 0.000001F);
            ParticleSystem[] particles = m_DashObject.GetComponentsInChildren<ParticleSystem>();
            source.PlayOneShot(m_DashSound);
            foreach(ParticleSystem p in particles)
            {
                p.Play();
            }
            m_DashCooldown_Applied = false;
            return true;
        }
        return false;
    }

    public void UpdateTimers()
    {
        // Update all cooldown timers
        if (m_manager.isPlayer() || m_manager.isSecurityAI())
        {
            m_DashEffectTimer.Update();
            if (m_DashEffectTimer.isComplete() && (!m_DashCooldown_Applied))
            {
                m_DashCooldown.Add(GLOBAL_VALUES.DASH_COOLDOWN, true);
                m_DashCooldown_Applied = true;
                m_manager.Set_Rotate_Speed(m_AgentDefaultRotate);
                if (m_manager.isPlayer())
                {
                    dashUI.StartDashTimer();
                    ParticleSystem[] particles = m_DashObject.GetComponentsInChildren<ParticleSystem>();
                    foreach (ParticleSystem p in particles)
                    {
                        p.Stop();
                    }
                    dashUI.SetTimer(m_DashCooldown);
                }
                else
                {
                    m_DashCooldown.Add(GLOBAL_VALUES.DASH_COOLDOWN, true);
                }
                m_DashObject.SetActive(false);
            }
        }
        m_DashCooldown.Update();
    }

    private void SetMotionTrailColours(GameObject obj, int colour)
    {
        switch (colour)
        {
            case 0:
                {
                    obj.GetComponentInChildren<TrailRenderer>().startColor = GLOBAL_VALUES.MOTION_TRAIL_BLUE;
                    break;
                }
            case 1:
                {
                    obj.GetComponentInChildren<TrailRenderer>().startColor = GLOBAL_VALUES.MOTION_TRAIL_GREEN;
                    break;
                }
            case 2:
                {
                    obj.GetComponentInChildren<TrailRenderer>().startColor = GLOBAL_VALUES.MOTION_TRAIL_PINK;
                    break;
                }
            case 3:
                {
                    obj.GetComponentInChildren<TrailRenderer>().startColor = GLOBAL_VALUES.MOTION_TRAIL_RED;
                    break;
                }
            case 4:
                {
                    obj.GetComponentInChildren<TrailRenderer>().startColor = GLOBAL_VALUES.MOTION_TRAIL_YELLOW;
                    break;
                }
            default:
                {
                    // White Motion Trail
                    break;
                }
        }
    }
}