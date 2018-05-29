using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalManager : MonoBehaviour {

    public int m_maxDecals = 100;
    public float m_decalSize = 2.0f;

    private int m_decalIndex;
    private ParticleSystem m_decalParticleSystem;
    private List<ParticleSystem.Particle> m_particles = new List<ParticleSystem.Particle>();
    private ParticleSystem.EmitParams m_emitParams;

	void Start ()
    {
        m_emitParams = new ParticleSystem.EmitParams();

        m_decalParticleSystem = GetComponent<ParticleSystem>();
        
    }

    void Update()
    {
        EmitAtLocation();
    }



    public void Remove_Decal(ParticleSystem.Particle p)
    {
        if (m_particles.Contains(p))
        {
            m_particles.Remove(p);
            m_decalParticleSystem.Clear();
            m_decalParticleSystem.SetParticles(m_particles.ToArray(), m_particles.Count);
        }
    }
    public ParticleSystem.Particle AddParticle(Collision collision)
    {
        ParticleSystem.Particle ret = SetDecalData(collision);
        DisplayParticles();
        return ret;
    }
    private void DisplayParticles()
    {
        m_decalParticleSystem.SetParticles(m_particles.ToArray(), m_particles.Count);
    }
    ParticleSystem.Particle SetDecalData(Collision collision)
    {
        // Check if array has reached limit, if so loop back to start.
        Vector3 position = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        foreach (ContactPoint c in collision.contacts)
        {
            position += c.point;
            rotation += c.normal;
        }
        position = position / collision.contacts.Length;
        rotation = rotation / collision.contacts.Length;

        // Set Position
        Vector3 addPos = Vector3.zero;
        if (collision.collider.CompareTag(GLOBAL_VALUES.TAG_ENVIRONMENT_FLOOR))
        {
            addPos.y += 0.025f;
        }
        Decal new_Decal = new Decal();
        new_Decal.m_position = position + addPos;

        // Set Rotation
        Vector3 decalRotationEuler = Quaternion.LookRotation(rotation).eulerAngles;
        decalRotationEuler.z = Random.Range(0, 360); // Randomize Z rotation
        new_Decal.m_rotation = decalRotationEuler;

        //Set Size
        new_Decal.m_size = m_decalSize;
        ParticleSystem.Particle new_partical = new ParticleSystem.Particle();
        new_partical.position = new_Decal.m_position;
        new_partical.rotation3D = new_Decal.m_rotation;
        new_partical.startSize = new_Decal.m_size;
        new_partical.startLifetime = 1;
        new_partical.remainingLifetime = 1;
        m_particles.Add(new_partical);
        return new_partical;
    }

    public void EmitAtLocation()
    {
        for (int i = 0; i < m_particles.Count; i++)
        {
            m_emitParams.position = m_particles[i].position;
            m_emitParams.rotation3D = m_particles[i].rotation3D;
            m_emitParams.startSize = m_particles[i].startSize;
            m_emitParams.startLifetime = m_particles[i].startLifetime;

            m_decalParticleSystem.Emit(m_emitParams, 1);
         }
    }
}
