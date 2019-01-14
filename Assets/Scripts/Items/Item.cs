using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    static private Transform _ITEM_ANCHOR;
    static Transform ITEM_ANCHOR
    {
        get
        {
            if (_ITEM_ANCHOR == null)
            {
                GameObject go = new GameObject("ItemAnchor");
                _ITEM_ANCHOR = go.transform;
            }
            return _ITEM_ANCHOR;
        }
    }

    private float speed = 25f;
    private float rotationSpeed = 0.05f;

    void Start()
    {
        Warpaid.AddItem(this);
        transform.SetParent(ITEM_ANCHOR, true);
        transform.rotation = Quaternion.Euler(Time.time * speed, -90f, -90f);
    }
    void OnDestroy()
    {
        Warpaid.RemoveItem(this);
        Destroy(gameObject);
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(Time.time * speed, -90f, -90f);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z -rotationSpeed), rotationSpeed);
    }
    static public Transform GetTransform()
    {
        return ITEM_ANCHOR;
    }
}
