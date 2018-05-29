using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnimations : MonoBehaviour
{
    public AgentManager m_manager;

    private GameObject m_trail_L;
    private GameObject m_trail_R;
    private ParticleSystem m_walk;
    private ParticleSystem m_dustRing;

    private AudioSource m_soundSource;
    private AudioClip m_GroundslamSound;
    private AudioClip m_FootstepSound;
    private AudioClip m_ThrowSound;


    // Use this for initialization
    void Start()
    {
        m_soundSource = m_manager.Get_Audio_Source();
        m_GroundslamSound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_GROUNDSLAM);
        m_FootstepSound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_PLAYER_FOOTSTEP_01);
        m_ThrowSound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_THROW);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup()
    {
        m_trail_L = m_manager.Get_Object().transform.Find("jntTorsoLowerMain/jntTorsoUpper/jntHand_L/FistTrail_L").gameObject;
        m_trail_R = m_manager.Get_Object().transform.Find("jntTorsoLowerMain/jntTorsoUpper/jntHand_R/FistTrail_R").gameObject;
        m_walk = m_manager.Get_Object().transform.Find("Player_Walk").gameObject.GetComponent<ParticleSystem>();
        m_dustRing = m_manager.Get_Object().transform.Find("Dust_Ring").gameObject.GetComponent<ParticleSystem>();
    }

    void AnimEvent_Throw()
    {
        m_manager.Throw();
        m_soundSource.PlayOneShot(m_ThrowSound);
    }

    void AnimEvent_Punch()
    {
        m_manager.PunchMechanic();
    }

    void AnimEvent_GroundSlam()
    {
        m_manager.GetPunchControl().Groundslam_Mechanic();
        m_soundSource.PlayOneShot(m_GroundslamSound);
    }

    void AnimEvent_PlayerWalk()
    {
        m_walk.Play();
        m_soundSource.PlayOneShot(m_FootstepSound);
    }

    void AnimEvent_DustRing()
    {
        m_dustRing.Play();
    }

    public void AnimEvent_TrailStart()
    {
        m_trail_L.SetActive(true);
        m_trail_R.SetActive(true);
    }

    public void AnimEvent_TrailStop()
    {
        m_trail_L.SetActive(false);
        m_trail_R.SetActive(false);
    }

    void AnimEvent_ReleaseButtons()
    {
        //m_manager.Unlock_A_Button();
        //m_manager.Unlock_B_Button();
        //m_manager.Unlock_X_Button();
        //m_manager.Unfreeze_Agent();
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