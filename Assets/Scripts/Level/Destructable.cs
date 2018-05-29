using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {

    public int m_customHealth = 0;
    private int m_objectHealth;
    private int m_currentState;
    private GameObject m_particles;
    private DecalManager m_DecalManager = null;
    private List<ParticleSystem.Particle> m_Decals = new List<ParticleSystem.Particle>();
    
    // Use this for initialization
	void Start () 
    {
        m_currentState = 0; // Default/Starting State

        if (m_customHealth == 0)
        {
            m_objectHealth = GLOBAL_VALUES.WALL_HEALTH;
        }
        else
        {
            m_objectHealth = m_customHealth;
        }

        m_particles = gameObject.transform.Find("Impact_Terrain").gameObject;
    }

    public void Add_Decal(ParticleSystem.Particle new_decal, DecalManager dm)
    {
        if (m_DecalManager == null)
        {
            m_DecalManager = dm;
        }
        m_Decals.Add(new_decal);
    }
    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        try
        {
            if ((other.CompareTag(GLOBAL_VALUES.TAG_PLAYER)) && (other.GetComponent<CollisionDetection>().m_Manager.isDashing()))
            {
                DealDamage(1, other.GetComponent<CollisionDetection>().m_Manager); // Set damage caused by dashing players
                
            }
            else if ((other.CompareTag(GLOBAL_VALUES.PICKUP_ITEM)) && (other.GetComponent<Item>().GetItemState() == ItemState.thrown))
            {
                DealDamage(1, other.GetComponent<Item>().prev_owner);    // Set damage caused by thrown items
            }
        }
        catch
        {
            Debug.Log("Collision Failed on Wall [" + this.gameObject.name + "]");
        }
    }

    void Update()
    {
        if ((m_currentState < 2) && (m_objectHealth <= 0))
        {
            ChangeState();
        }
    }

    private void ChangeState()
    {
        for (int i = m_Decals.Count - 1; i >= 0; i--)
        {
           m_DecalManager.Remove_Decal(m_Decals[i]);
            m_Decals.RemoveAt(i);
        }
        switch (m_currentState)
        {
            case 0:
                {
                    // Change mesh
                    FindChild(this.gameObject, GLOBAL_VALUES.TAG_DESTRUCTABLE_CLEAN).GetComponent<MeshRenderer>().enabled = false;
                    FindChild(this.gameObject, GLOBAL_VALUES.TAG_DESTRUCTABLE_DAMAGED).GetComponent<MeshRenderer>().enabled = true;

                    // Reset Health
                    m_objectHealth = GLOBAL_VALUES.WALL_HEALTH;

                    // Change State Value
                    m_currentState++;
                    break;
                }
            case 1:
                {
                    // Disable Main Collider
                    this.GetComponent<BoxCollider>().enabled = false;

                    // Hide Shelf Mesh
                    FindChild(this.gameObject, GLOBAL_VALUES.TAG_DESTRUCTABLE_DAMAGED).GetComponent<MeshRenderer>().enabled = false;

                    // Spawn Debris (From Parent Object)
                    foreach (Transform t in FindChild(this.gameObject, GLOBAL_VALUES.TAG_DESTRUCTABLE_DEBRIS).transform)
                    {
                        t.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        t.gameObject.GetComponent<Collider>().enabled = true;
                        t.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    }

                    // Change State Value
                    m_currentState++;
                    break;
                }
            default:
                {
                    Debug.Log("Error finding state on Wall [" + this.gameObject.name + "]");
                    break;
                }
        }
    }

    public void DealDamage(int damage, AgentManager damager)
    {
        if ((m_currentState < 2) && (m_objectHealth > 0))
        {
            ParticleSystem[] ps = m_particles.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in ps)
            {
                p.Play();
            }
            m_objectHealth -= damage;
            damager.m_StatCollector.Count_Environmental_Damage(damage);
        }
    }

    private GameObject FindChild(GameObject parent, string tag)
    {
        foreach (Transform t in parent.transform)
        {
            if (t.tag == tag)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
