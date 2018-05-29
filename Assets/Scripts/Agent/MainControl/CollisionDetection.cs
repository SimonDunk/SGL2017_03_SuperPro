using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour {
    public AgentManager m_Manager = null;
    private float rumble = GLOBAL_VALUES.CONTROLLER_RUMBLE_INTENSITY;
    public void Add_Manager(AgentManager man)
    {
        m_Manager = man;
    }
    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            // get the object you collided with
            GameObject Other = collision.gameObject;
            //hit by item
            if ((Other.GetComponent<Rigidbody>() != null) &&
                (Other.CompareTag(GLOBAL_VALUES.PICKUP_ITEM)) &&
                (Other.GetComponent<Item>().prev_owner != m_Manager) &&
                (Other.GetComponent<Item>().GetItemState() == ItemState.thrown))
            {
                GameObject.Find("GameManagerObject").GetComponent<GameManager>().Get_MatchManager().Get_RoundManager().Get_Camera_Manager().AddForce(GLOBAL_VALUES.CAMERA_FORCE_TIME_DELAY, collision.relativeVelocity);
                // Collided with a thrown pickup item
                if (m_Manager.isPlayer())
                {
                    Other.GetComponent<Item>().prev_owner.m_StatCollector.Count_Player_Stun();
                    m_Manager.Get_Input().Controller_Rumble(rumble, rumble);
                }
                else if (m_Manager.isAI())
                {
                    Other.GetComponent<Item>().prev_owner.m_StatCollector.Count_AI_Stunned();
                    if (m_Manager.isSecurityAI())
                    {
                        SecurityAI_Control sec_brain = m_Manager.Get_Input() as SecurityAI_Control;
                        sec_brain.m_Last_Collided_Item = Other.GetComponent<Item>();
                        sec_brain.Stun_Flag = false;
                    }
                }
                Vector3 targetAngle = Custom_Math_Utils.FindTargetAngle(m_Manager.Get_Position(), Other.gameObject.transform.position);
                m_Manager.ImpactParticles("throw", targetAngle);
                m_Manager.m_StatCollector.Count_Stunned_By_Item();
                m_Manager.Stun_Agent(GLOBAL_VALUES.STUN_TIME_ITEM);
                m_Manager.Play_Impact_Particles(Other.GetComponent<Item>().prev_owner.Get_Color());
            }
            else if ((Other.GetComponent<Rigidbody>() != null) &&
                (Other.CompareTag(GLOBAL_VALUES.TAG_PLAYER) || Other.CompareTag(GLOBAL_VALUES.TAG_AI_SECURITY)) &&
                (Other.GetComponent<CollisionDetection>().m_Manager.isDashing()))
            {
                // Collided with dashing player
                if (m_Manager.isSecurityAI())
                {
                    SecurityAI_Control sec_brain = m_Manager.Get_Input() as SecurityAI_Control;
                    sec_brain.Agro(Other.GetComponent<CollisionDetection>().m_Manager);
                    sec_brain.Stun_Flag = false;
                }
                //player hit by another player
                if(m_Manager.isPlayer() || Other.CompareTag(GLOBAL_VALUES.TAG_DESTRUCTABLE_PARENT))
                {
                    m_Manager.Get_Input().Controller_Rumble(rumble, rumble);
                }
                m_Manager.Play_Impact_Particles(Other.GetComponent<CollisionDetection>().m_Manager.Get_Color());
                Vector3 targetAngle = Custom_Math_Utils.FindTargetAngle(m_Manager.Get_Position(), Other.GetComponent<CollisionDetection>().m_Manager.Get_Position());
                m_Manager.ImpactParticles("dash", targetAngle);
                m_Manager.AddForce(0.1f, (targetAngle * GLOBAL_VALUES.KNOCKBACK_DASH));
                Vector3 f = (targetAngle * GLOBAL_VALUES.KNOCKBACK_DASH);
                m_Manager.AddForce(0.1f, f);
                m_Manager.Drop();
                //Other.GetComponent<CollisionDetection>().m_Manager.Drop();
            }
        }
        catch
        {
            Debug.Log("Collision Failed on Agent [" + m_Manager.Get_Object().name + "]");
        }
    }
}
