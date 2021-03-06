﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour
{

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Boundary") || coll.CompareTag("Enemy") || coll.CompareTag("Item"))
        {
            return;
        }
        Destroy(coll.gameObject);
        Destroy(gameObject);
    }
}
