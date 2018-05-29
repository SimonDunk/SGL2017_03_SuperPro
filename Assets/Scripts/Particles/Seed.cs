using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour {
    ParticleSystem big;
    ParticleSystem small;
    ParticleSystem.Particle[] sParticles;
   
    uint seed;
    bool changed = false;
	// Use this for initialization
	void Start () {
        big = transform.Find("Background Explosion").GetComponent<ParticleSystem>();
        small = transform.Find("Foreground Explosion").GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!changed)
        {
            seed = big.randomSeed;
            sParticles = new ParticleSystem.Particle[small.particleCount];
            small.GetParticles(sParticles);
            for (int i = 0; i < small.particleCount; i++)
            {
                sParticles[i].randomSeed = seed;
            }

            small.SetParticles(sParticles, small.particleCount);
        }
	}
}
