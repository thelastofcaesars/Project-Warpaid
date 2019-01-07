using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagement : MonoBehaviour
{
    public GameObject shot;
    public Transform[] shotSpawns;
    public float bulletTime;
    public float delay;

    private new AudioSource audio;

    //To do: Management, Movement and Firing;
    void Start()
    {
        audio = GetComponent<AudioSource>();
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
}
