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



    [System.Flags]
    public enum eItemType
    {
        // Decimal      // Binary
        none = 0,       // 00000000
        Heart = 1,   // 00000001
        Armor = 2,   // 00000010
        Cash = 4,      // 00000100
        Points = 8,  // 00001000
        Ammo = 16,  // 00010000
        Letter = 32, // 00100000
        all = 0xFFFFFFF // 11111111111111111111111111111111
    }


    [EnumFlags]
    public eItemType itemType = eItemType.none;
    public string itemID = "0000";
    public int value  = 100;

    // Hidden in Inspector
    private float speed = 25f;
    private readonly float rotationSpeed = 0.05f;

    void Start()
    {
        Warpaid.AddItem(this);
        transform.SetParent(ITEM_ANCHOR, true);
        transform.rotation = Quaternion.Euler(0f, 0f, Time.time * speed);
    }
    private void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            PlayerShip.AddItem(this);
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit(Collider coll)
    {
        if(coll.CompareTag("Boundary"))
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        Warpaid.RemoveItem(this);
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Time.time * speed);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z -rotationSpeed), rotationSpeed);
    }
    static public Transform GetTransform()
    {
        return ITEM_ANCHOR;
    }
}
public struct snafu
{
    public bool S;
    public bool N;
    public bool A;
    public bool F;
    public bool U;
    public bool R;
};
public struct orb
{
    public bool Red;
    public bool Orange;
    public bool Yellow;
    public bool Green;
    public bool Blue;
    public bool Purple;
    public bool White;
}
public struct boost
{
    public float bulletTime;
    public float reflex;
    public float speedBoost;
    public float energy;
    public float freezeTime;
}

