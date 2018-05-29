using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
    ParticleSystem.Particle[] particles;
    ParticleSystem fire;

	// Use this for initialization
	void Start () {
        fire = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        particles = new ParticleSystem.Particle[fire.particleCount];
        fire.GetParticles(particles);

        for (int i = 0; i < fire.particleCount; i++)
        {
            if ((particles[i].remainingLifetime < 0.8) && (particles[i].remainingLifetime > 0.4))
            {
                if (particles[i].position.x < 0)
                {
                    particles[i].velocity = new Vector3(0.2f, particles[i].velocity.y, particles[i].velocity.z);
                }
                else if (particles[i].position.x > 0)
                {
                    particles[i].velocity = new Vector3(-0.2f, particles[i].velocity.y, particles[i].velocity.z);
                }

                if (particles[i].position.y < 0)
                {
                    particles[i].velocity = new Vector3(particles[i].velocity.x, 0.2f, particles[i].velocity.z);
                }
                else if (particles[i].position.y > 0)
                {
                    particles[i].velocity = new Vector3(particles[i].velocity.x, -0.2f, particles[i].velocity.z);
                }
            }
            else if (particles[i].remainingLifetime < 0.4)
            {
                particles[i].velocity = new Vector3(0, 0, particles[i].velocity.z);
            }
        }
        fire.SetParticles(particles, fire.particleCount);
	}
}
