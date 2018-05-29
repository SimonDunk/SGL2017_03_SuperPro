using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPunchControl
{
    private Timer m_PunchCooldown = new Timer();
    private Timer m_GroundslamCooldown = new Timer();
    private Timer m_ComboTimer = new Timer();
    private Timer m_HoldTimer = new Timer();
    private AgentManager m_Agent;
    private float m_ComboTimeout = 0.5f;
    private float m_FullComboCooldown = 1f;
    private int m_ComboCounter = 0;
    private bool m_OnCooldown = false;
    private bool m_Button_Down = false;
    private Item_And_Agent_Tracker m_PunchHitbox = null;
    private Item_And_Agent_Tracker m_GroundslamHitbox = null;
    private GameObject m_Effect = null;
    private List<Animator> m_anims;
    private AudioSource m_AudioSource;
    private AudioClip m_PunchSound;
    private AudioClip m_GroundSlamSound;
    private ItemManager m_ItemManager;
    private GameObject m_slamCharge_Init;
    private GameObject m_slamCharge_Hold;
    private float rumble = GLOBAL_VALUES.CONTROLLER_RUMBLE_INTENSITY;
    private bool groundSlam = false;

    public AgentPunchControl(AgentManager agent)
    {
        m_Agent = agent;
        m_PunchHitbox = m_Agent.Get_Object().transform.Find("Punch_Hitbox").GetComponent<Item_And_Agent_Tracker>();
        m_GroundslamHitbox = m_Agent.Get_Object().transform.Find("GroundSlam_Hitbox").GetComponent<Item_And_Agent_Tracker>();
        m_Effect = m_GroundslamHitbox.gameObject.transform.Find("Impact_GroundSlam").gameObject;

        if (m_Agent.Get_Object().CompareTag(GLOBAL_VALUES.TAG_PLAYER))
        {
            m_slamCharge_Init = m_Agent.Get_Object().transform.Find("GroundSlam_Charge").gameObject;
            m_slamCharge_Hold = m_Agent.Get_Object().transform.Find("GroundSlam_Hold").gameObject;
        }
        m_anims = GetAnimators();
        m_AudioSource = m_Agent.Get_Audio_Source();
        m_PunchSound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_PUNCH);
        m_GroundSlamSound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_GROUNDSLAM);
    }

    public void Report_Button_State(bool pressed, bool held, bool released)
    {
        if (pressed)
        {
            // button pressed
            foreach (Animator a in m_anims)
            {
                a.SetTrigger(GLOBAL_VALUES.ANIM_PUNCH_CHARGE);
            }

            if (m_Agent.Get_Object().CompareTag(GLOBAL_VALUES.TAG_PLAYER))
            {
                foreach (ParticleSystem p in m_slamCharge_Init.GetComponentsInChildren<ParticleSystem>())
                {
                    p.Play();
                }
                foreach (ParticleSystem p in m_slamCharge_Hold.GetComponentsInChildren<ParticleSystem>())
                {
                    p.Play();
                }
            }
            m_Button_Down = true;
            m_HoldTimer.Reset();
            m_HoldTimer.Add(GLOBAL_VALUES.GROUNDSLAM_CHARGE_TIME, true);
            if (m_Agent.isPlayer())
            {
                m_Agent.Set_Move_Speed(GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED * 0.5f);
            }
            else if (m_Agent.isSecurityAI())
            {
                m_Agent.Set_Move_Speed(GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED * 0.8f);
            }
        }
        else if(held)
        {
            m_HoldTimer.Update();
        }
        else if(released && m_Button_Down)
        {
            // button released
            m_Button_Down = false;
            if (m_Agent.Get_Object().CompareTag(GLOBAL_VALUES.TAG_PLAYER))
            {
                foreach (ParticleSystem p in m_slamCharge_Init.GetComponentsInChildren<ParticleSystem>())
                {
                    p.Stop();
                    p.Clear();
                }
                foreach (ParticleSystem p in m_slamCharge_Hold.GetComponentsInChildren<ParticleSystem>())
                {
                    p.Stop();
                    p.Clear();
                }
            }

            if (m_HoldTimer.isComplete() || m_Agent.isSecurityAI())
            {
                if (m_GroundslamCooldown.isComplete() || m_Agent.isSecurityAI())
                {
                    if(m_Agent.isPlayer())
                        m_Agent.Freeze_Agent();
                    Punch_Three();
                    groundSlam = true;
                    m_GroundslamCooldown.Add(GLOBAL_VALUES.COOLDOWN_GROUNDSLAM, true);
                }
                else if (!m_Agent.bAction)
                {
                    Punch_One();
                    m_PunchCooldown.Add(GLOBAL_VALUES.COOLDOWN_PUNCH, true);
                }
                else
                {
                    foreach (Animator a in m_anims)
                    {
                        a.SetTrigger("Cancel_Charge");
                    }
                    m_Agent.Unfreeze_Agent();
                    Debug.Log("top bs");
                }

            }
            else
            {
                if (!m_Agent.bAction)
                {
                    Punch_One();
                    m_PunchCooldown.Add(GLOBAL_VALUES.COOLDOWN_PUNCH, true);
                }
                else
                {
                    foreach(Animator a in m_anims)
                    {
                        a.SetTrigger("Cancel_Charge");
                    }
                    m_Agent.Unfreeze_Agent();
                    Debug.Log("bottom bs");
                }
            }
            if (m_Agent.isPlayer())
            {
                m_Agent.Set_Move_Speed(GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED);
            }
            else if (m_Agent.isSecurityAI())
            {
                m_Agent.Set_Move_Speed(GLOBAL_VALUES.BASE_PLAYER_MOVEMENT_SPEED * 0.9f);
            }
        }
    }
    public void Update()
    {
        m_ComboTimer.Update();
        m_GroundslamCooldown.Update();
        m_PunchCooldown.Update();
        if (m_ComboTimer.isComplete())
        {
            if (m_OnCooldown)
            {
                m_ComboCounter = 0;
                m_OnCooldown = false;
            }
            else
            {
                if (m_ComboCounter != 0)
                {
                    m_ComboCounter = 0;
                }
            }
        }
    }

    public bool PerformPunch()
    {
        // For use in the combo!
        if (!m_OnCooldown)
        {
            if (m_Agent.isSecurityAI())
            {
                // security just goes straight for ground slam, no punching combo necessary
                Punch_Three();
                m_ComboCounter = 0;
                m_ComboTimer.Reset();
                m_ComboTimer.Add(m_FullComboCooldown, true);
                m_OnCooldown = true;
            }
            else
            {
                switch (m_ComboCounter)
                {
                    case 0:
                        {
                            Punch_One();
                            m_ComboCounter++;
                            m_ComboTimer.Reset();
                            m_ComboTimer.Add(m_ComboTimeout, true);
                            break;
                        }
                    case 1:
                        {
                            Punch_Two();
                            m_ComboTimer.Reset();
                            m_ComboTimer.Add(m_ComboTimeout, true);
                            m_ComboCounter++;
                            break;
                        }
                    case 2:
                        {
                            Punch_Three();
                            m_ComboCounter = 0;
                            m_ComboTimer.Reset();
                            m_ComboTimer.Add(m_FullComboCooldown, true);
                            m_OnCooldown = true;
                            break;
                        }
                    default:
                        {
                            m_ComboCounter = 0;
                            m_ComboTimer.Reset();
                            return false;
                        }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Setup_Item_Manager(ItemManager newItemManager)
    {
        m_ItemManager = newItemManager;
    }
    // PUNCH FUNCTIONS
    private void Punch_One()
    {
        m_Agent.Lock_B_Button();
        foreach (Animator a in m_anims)
        {
            a.SetTrigger(GLOBAL_VALUES.ANIM_PUNCH_1);
        }
    }

    private void Punch_Two()
    {
        foreach (Animator a in m_anims)
        {
            a.SetTrigger(GLOBAL_VALUES.ANIM_PUNCH_2);
        }
    }

    private void Punch_Three()
    {
        foreach (Animator a in m_anims)
        {
            a.SetTrigger(GLOBAL_VALUES.ANIM_PUNCH_3);
        }
    }

    public void Groundslam_Mechanic()
    {
        m_Agent.Unfreeze_Agent();
        m_Agent.NotifySubscribers(AgentAction.GroundSlam);
        groundSlam = false;
        GameObject.Find("GameManagerObject").GetComponent<GameManager>().Get_MatchManager().Get_RoundManager().Get_Camera_Manager().AddForce(GLOBAL_VALUES.CAMERA_FORCE_TIME_DELAY);
        m_Agent.m_StatCollector.Count_Groundslam();
        m_AudioSource.PlayOneShot(m_GroundSlamSound);
        foreach (ParticleSystem p in m_Effect.GetComponentsInChildren<ParticleSystem>())
        {
            p.Play();
        }
        // turn off item and agent raycasts
        foreach (GameObject item in m_GroundslamHitbox.m_Items)
        {
            item.layer = GLOBAL_VALUES.LAYER_IGNORE_RAYCAST;
        }
        foreach (GameObject agent in m_GroundslamHitbox.m_Agents)
        {
            agent.layer = GLOBAL_VALUES.LAYER_IGNORE_RAYCAST;
        }
        // push back items
        foreach (GameObject item in m_GroundslamHitbox.m_Items)
        {
            // turn raycast for this item back on
            item.layer = GLOBAL_VALUES.LAYER_DEFAULT;
            // raycast to see if we have a los
            RaycastHit hit;
            Vector3 l_pos = m_Agent.Get_Position();
            // if raycast hit returns and you can hit the object
            if ((
                (Physics.Raycast(l_pos, item.transform.position - l_pos, out hit)) &&
                (hit.transform.gameObject == item))
                )
            {
                // Get the items rigidbody
                Rigidbody rb = item.GetComponent<Rigidbody>();
                // Punch it at a force depending on the punch
                // direction is the direction away from the point of ground slam and up a bit
                Vector3 dir = Custom_Math_Utils.FindTargetAngle(item.transform.position, m_Agent.Get_Position());
                Vector3 f = dir * (GLOBAL_VALUES.KNOCKBACK_GROUNDSLAM * GLOBAL_VALUES.KNOCKBACK_PUNCH_ITEM_MULTIPLIER);
                rb.AddForce(f);
                
            }
        }

        // Jump all items
        if (m_ItemManager != null)
        {
            float base_up_force = 200;
            foreach (Item i in m_ItemManager.m_Items)
            {
                if (!i.BeingCarried())
                {
                    i.itemBody.AddForce(new Vector3(0, base_up_force - Vector3.Distance(i.transform.position, m_Agent.Get_Position()), 0));
                }
            }
        }

        // push back agents
        foreach (GameObject agent in m_GroundslamHitbox.m_Agents)
        {// turn raycast for this item back on
            agent.layer = GLOBAL_VALUES.LAYER_DEFAULT;
            // raycast to see if we have a los
            RaycastHit hit;
            Vector3 l_pos = m_Agent.Get_Position();
            // if raycast hit returns and you can hit the object
            if ((
                (Physics.Raycast(l_pos, agent.transform.position - l_pos, out hit)) &&
                (hit.transform.gameObject == agent))
                )
            {
                // Get the agentmanager
                AgentManager vic = agent.GetComponent<CollisionDetection>().m_Manager;
                vic.Drop(false);
                Vector3 dir = Custom_Math_Utils.FindTargetAngle(agent.transform.position, m_Agent.Get_Position());
                Vector3 f = dir * GLOBAL_VALUES.KNOCKBACK_GROUNDSLAM;
                vic.AddForce(0.2f, f);
                

                // Add in after agent refactor
                vic.Stun_Agent(GLOBAL_VALUES.STUN_TIME_GROUND_SLAM);
                if (vic.isPlayer())
                {
                    m_Agent.m_StatCollector.Count_Player_Stun();
                    vic.Get_Input().Controller_Rumble(rumble, rumble);
                }
                else if (vic.isAI())
                {
                    if (vic.isSecurityAI())
                    { 
                        SecurityAI_Control sec_brain = vic.Get_Input() as SecurityAI_Control;
                        sec_brain.Agro(m_Agent);
                        sec_brain.Stun_Flag = false;
                    }
                    m_Agent.m_StatCollector.Count_AI_Stunned();
                }
            }
        }
        // turn on item and agent raycasts
        foreach (GameObject item in m_GroundslamHitbox.m_Items)
        {
            item.layer = GLOBAL_VALUES.LAYER_DEFAULT;
        }
        foreach (GameObject agent in m_GroundslamHitbox.m_Agents)
        {
            agent.layer = GLOBAL_VALUES.LAYER_DEFAULT;
        }
        foreach (GameObject destruct in m_PunchHitbox.m_Desctructables)
        {
            destruct.GetComponent<Destructable>().DealDamage(2, m_Agent);
        }
    }

    public void Punch_Mechanic(float force)
    {
        m_Agent.m_StatCollector.Count_Punch();
        m_AudioSource.PlayOneShot(m_PunchSound);
        foreach (GameObject item in m_PunchHitbox.m_Items)
        {
            // Get the items rigidbody
            Rigidbody rb = item.GetComponent<Rigidbody>();
            // Punch it at a force depending on the punch
            Vector3 dir = Custom_Math_Utils.FindTargetAngle(item.transform.position, m_Agent.Get_Position());
            Vector3 f = dir * (force * GLOBAL_VALUES.KNOCKBACK_PUNCH_ITEM_MULTIPLIER);
            rb.AddForce(f);
            //GameObject.Find("GameManagerObject").GetComponent<GameManager>().Get_MatchManager().Get_RoundManager().Get_Camera_Manager().AddForce(GLOBAL_VALUES.CAMERA_FORCE_TIME_DELAY, f);
        }
        foreach (GameObject agent in m_PunchHitbox.m_Agents)
        {
            // Get the agentmanager
            AgentManager vic = agent.GetComponent<CollisionDetection>().m_Manager;
            if (!vic.GetPunchControl().inGroundSlam())
            {
                vic.Drop(false);
                Vector3 dir = Custom_Math_Utils.FindTargetAngle(agent.transform.position, m_Agent.Get_Position());
                Vector3 f = dir * force;
                vic.AddForce(0.2f, f);

                Vector3 punchImpact = agent.transform.Find("Impact_Punch").transform.rotation.eulerAngles;
                punchImpact.y = m_Agent.Get_Object().transform.rotation.eulerAngles.y;
                vic.ImpactParticles("punch", punchImpact);
                vic.Stun_Agent(GLOBAL_VALUES.STUN_TIME_PUNCH);

                if (vic.isSecurityAI())
                {
                    SecurityAI_Control sec_brain = vic.Get_Input() as SecurityAI_Control;
                    sec_brain.Agro(m_Agent);
                    sec_brain.Stun_Flag = false;
                    m_Agent.m_StatCollector.Count_AI_Stunned();
                }
                if (vic.isPlayer())
                {
                    m_Agent.m_StatCollector.Count_Player_Stun();
                    vic.Get_Input().Controller_Rumble(rumble, rumble);
                }
            }           
        }
        foreach (GameObject destruct in m_PunchHitbox.m_Desctructables)
        {
            destruct.GetComponent<Destructable>().DealDamage(1, m_Agent);
        }
        m_Agent.NotifySubscribers(AgentAction.Punch);
    }

    private List<Animator> GetAnimators()
    {
        List<Animator> returnList = new List<Animator>();

        foreach (Animator a in m_Agent.Get_Object().GetComponentsInChildren<Animator>())
        {
            for (int i = 0; i < a.parameterCount; i++)
            {
                if (a.parameters[i].name == GLOBAL_VALUES.ANIM_PUNCH_1)
                {
                    returnList.Add(a);
                    break;
                }
            }
        }

        return returnList;
    }

    public void SetEffectColour()
    {
        
        var col = m_slamCharge_Init.transform.Find("InwardLines").GetComponent<ParticleSystem>().colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(GLOBAL_VALUES.GROUNDSLAM_INWARD_LINES_COLOUR[m_Agent.Get_Color()], GLOBAL_VALUES.GROUNDSLAM_INWARD_LINES_ALPHA);
        col.color = grad;

        ParticleSystem.MainModule ps = m_slamCharge_Init.transform.Find("OuterPulse").GetComponent<ParticleSystem>().main;
        ps.startColor = new ParticleSystem.MinMaxGradient(GLOBAL_VALUES.GROUNDSLAM_OUTWARD_PULSE[m_Agent.Get_Color()]);

        col = m_slamCharge_Hold.transform.Find("Circles").GetComponent<ParticleSystem>().colorOverLifetime;
        col.enabled = true;
        grad = new Gradient();
        grad.SetKeys(GLOBAL_VALUES.GROUNDSLAM_CIRCLES_COLOUR[m_Agent.Get_Color()], GLOBAL_VALUES.GROUNDSLAM_CIRCLES_ALPHA);
        col.color = grad;

        ps = m_slamCharge_Hold.transform.Find("OuterPulse").GetComponent<ParticleSystem>().main;
        ps.startColor = new ParticleSystem.MinMaxGradient(GLOBAL_VALUES.GROUNDSLAM_OUTWARD_PULSE[m_Agent.Get_Color()]);
    }

    public bool inGroundSlam()
    {
        return groundSlam;
    }
}
