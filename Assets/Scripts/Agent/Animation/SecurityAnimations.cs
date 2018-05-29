using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityAnimations : MonoBehaviour
{
    
    public AgentManager m_manager;
    
    private ParticleSystem m_walk;
    private ParticleSystem m_dustRing;
    

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void Setup()
    {
        m_walk = m_manager.Get_Object().transform.Find("Player_Walk").gameObject.GetComponent<ParticleSystem>();
        m_dustRing = m_manager.Get_Object().transform.Find("Dust_Ring").gameObject.GetComponent<ParticleSystem>();
    }
    
    private void AnimEvent_Throw()
    {
        m_manager.Throw();
    }

    private void AnimEvent_GroundSlam()
    {
        SecurityAI_Control ai = m_manager.Get_Input() as SecurityAI_Control;
        ai.curr_attacking = false;
        m_manager.GetPunchControl().Groundslam_Mechanic();
        m_manager.Get_Animator().SetTrigger("GroundSlam_Idle");
    }

    private void AnimEvent_Punch()
    {
        SecurityAI_Control ai = m_manager.Get_Input() as SecurityAI_Control;
        ai.curr_attacking = false;
        m_manager.PunchMechanic();
    }
    void AnimEvent_PlayerWalk()
    {
        m_walk.Play();
    }
    
    void AnimEvent_DustRing()
    {
        m_dustRing.Play();
    }

    void AnimEvent_TrailStart()
    {
        // not needed
    }

    void AnimEvent_TrailStop()
    {
        // not needed
    }

    void AnimEvent_ReleaseButtons()
    {
        // not needed
    }

    void AnimEvent_bStart()
    {
        m_manager.bAction = true;
        //m_manager.Freeze_Agent();
        m_manager.Lock_B_Button();
        m_manager.Lock_A_Button();
        m_manager.Lock_X_Button();
        m_manager.Lock_Analog();
    }
    void AnimEvent_bEnd()
    {
        m_manager.bAction = false;
        //m_manager.Unfreeze_Agent();
        m_manager.Unlock_B_Button();
        m_manager.Unlock_A_Button();
        m_manager.Unlock_X_Button();
        m_manager.Unlock_Analog();
        //m_manager.Get_Animator().SetTrigger("GroundSlam_Idle");
    }

    void AnimEvent_bStart2()
    {
        m_manager.bAction = true;
        m_manager.Lock_B_Button();
        m_manager.Lock_A_Button();
        m_manager.Lock_X_Button();
    }
    void AnimEvent_bEnd2()
    {
        m_manager.bAction = false;
        m_manager.Unlock_B_Button();
        m_manager.Unlock_A_Button();
        m_manager.Unlock_X_Button();
    }
}