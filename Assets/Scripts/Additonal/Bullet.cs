using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OffScreenWrapper))]
public class Bullet : MonoBehaviour {
    static private Transform _BULLET_ANCHOR;
    static Transform BULLET_ANCHOR {
        get {
            if (_BULLET_ANCHOR == null) {
                GameObject go = new GameObject("BulletAnchor");
                _BULLET_ANCHOR = go.transform;
            }
            return _BULLET_ANCHOR;
        }
    }

    public ParticleSystem bulletExhaustPrefab;

    public float    bulletSpeed = 20;
    public float    lifeTime = 2;
    public bool     bDidWrap = false;

    void Start()
    {
        transform.SetParent(BULLET_ANCHOR, true);

        // Set Bullet to self-destruct in lifeTime seconds
        Invoke("DestroyMe", lifeTime);

        // Set the velocity of the Bullet
        GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
    }

    void Update()
    {
        if (Particle.GetParticlePrefab(bulletExhaustPrefab) != null)
        { 
            //Initializing particle //new
            ParticleSystem particles = Instantiate<ParticleSystem>(Particle.GetParticlePrefab(bulletExhaustPrefab));
            particles.transform.position = new Vector3(GetComponent<Rigidbody>().position.x, GetComponent<Rigidbody>().position.y, 0.0f);
            particles.transform.SetParent(transform, true);
            particles.Play();
            //
            //particles.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity; //new
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!coll.CompareTag("Enemy"))
        {
            return;
        }
        Warpaid.AddScore(coll.GetComponent<Enemy>().score);
        Destroy(coll.gameObject);
        Destroy(gameObject);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
    
}
