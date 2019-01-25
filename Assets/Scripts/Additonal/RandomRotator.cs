using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    private Rigidbody rigid;
    public float tumble;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.angularVelocity = Random.insideUnitSphere * tumble;
    }
}
