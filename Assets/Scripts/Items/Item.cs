using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private float speed = 30f;
    private float zSpeed = 0.05f;
    private void Start()
    {
        transform.rotation = Quaternion.Euler(Time.time * speed, -90f, -90f);
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(Time.time * speed, -90f, -90f);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z -zSpeed), zSpeed);
    }
}
