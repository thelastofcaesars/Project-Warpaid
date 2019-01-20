﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OffScreenWrapper))]
public class Enemy : MonoBehaviour
{
    [Header("Set Dynamically")]
    public int size = 1;
    public bool immune = false;
    public float speed = 5f;
    public int score = 100;

    public GameObject shot;
    public Transform[] shotSpawns;
    public float bulletTime;
    public float delay;

    private new AudioSource audio;
    Rigidbody rigid; // protected

    OffScreenWrapper offScreenWrapper;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        offScreenWrapper = GetComponent<OffScreenWrapper>();
    }

    // Use this for initialization
    void Start()
    {
        Warpaid.AddEnemy(this);
        transform.localScale = Vector3.one * Warpaid.EnemiesSO.enemyScale;
        InitEnemy();
        //To do: Management, Movement and Firing;
        InvokeRepeating("Fire", delay, bulletTime);
    }

    void Fire()
    {
        foreach (var shotSpawn in shotSpawns)
        {
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        }
        audio.Play();
    }

    private void OnDestroy()
    {
        Warpaid.RemoveEnemy(this);
    }

    public void InitEnemy()
    {
        offScreenWrapper.enabled = false;
        rigid.isKinematic = false;
        // Snap this GameObject to the z=0 plane
        Vector3 pos = transform.position;
        pos.y= 0;
        transform.position = pos;
        // Initialize the velocity for this Enemy
        InitVelocity();
    }

    public void InitVelocity()
    {
        Vector3 vel;
        // If in bounds, choose a random direction, and make sure that when you Normalize it, it doesn't
        //  have a length of 0 (which might happen if Random.insideUnitCircle returned [0,0,0].
        do
        {
            vel = Random.insideUnitCircle;
            vel.Normalize();
        } while (Mathf.Approximately(vel.magnitude, 0f));

        // Multiply the unit length of vel by the correct speed (randomized) for this size of Asteroid
        vel = vel * Random.Range(Warpaid.EnemiesSO.minVel, Warpaid.EnemiesSO.maxVel) / (float)size;
        rigid.velocity = vel;
        
    }

    Enemy normalEnemy
    {
        get
        {
            if (transform.parent != null)
            {
                Enemy normalEnemy = transform.parent.GetComponent<Enemy>();
                if (normalEnemy != null)
                {
                    return normalEnemy;
                }
            }
            return null;
        }
    }

    void OnTriggerEnter(Collider coll)
    {    
        if (immune)
        {
            return;
        }
        GameObject otherGO = coll.gameObject;
        if (otherGO.tag == "Bullet" || otherGO.transform.root.gameObject.tag == "Player")
        {
            if (otherGO.tag == "Bullet")
            {
                Destroy(otherGO);
                Warpaid.InitDrop(score, transform);
                Warpaid.AddScore(score);
            }
            InstantiateParticleSystem();
            
            Destroy(gameObject);
        }
    }


    void InstantiateParticleSystem()
    {
        GameObject particleGO = Instantiate<GameObject>(Warpaid.EnemiesSO.GetEnemyParticlePrefab(), transform.position, Quaternion.identity);
        ParticleSystem particleSys = particleGO.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleSys.main;
        main.startLifetimeMultiplier = size * 0.5f;
        ParticleSystem.EmissionModule emitter = particleSys.emission;
        ParticleSystem.Burst burst = emitter.GetBurst(0);
        ParticleSystem.MinMaxCurve burstCount = burst.count;
        burstCount.constant = burstCount.constant * size;
        burst.count = burstCount;
        emitter.SetBurst(0, burst);
    }

    void InstantiateDrop()
    {
         Warpaid.InitDrop(score, transform);
    }

    private void Update()
    {
        immune = false;
    }

    static public Enemy SpawnEnemy()
    {
        GameObject eGO = Instantiate<GameObject>(Warpaid.EnemiesSO.GetEnemyPrefab());
        Enemy enemy = eGO.GetComponent<Enemy>();
        return enemy;
    }
}
