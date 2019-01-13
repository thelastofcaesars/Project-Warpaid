using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    static private Transform _PARTICLE_ANCHOR;
    static Transform PARTICLE_ANCHOR
    {
        get
        {
            if (_PARTICLE_ANCHOR == null)
            {
                GameObject go = new GameObject("ParticleAnchor");
                _PARTICLE_ANCHOR = go.transform;
            }
            return _PARTICLE_ANCHOR;
        }
    }
 
    // public float lifeTime = 2;
    static public float particleLifeTime = 2.0f;
    
    void Start()
    {
        transform.SetParent(PARTICLE_ANCHOR, true);

        // Set Particle to self-destruct in lifeTime seconds
        Invoke("DestroyMe", particleLifeTime);
        particleLifeTime = 2.0f;
    }
    void NewParticleLifeTime(float newLifeTime)
    {
        particleLifeTime = newLifeTime;
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    static public ParticleSystem GetParticlePrefab(ParticleSystem particleEffect)
    {
        return particleEffect;
    }
}
