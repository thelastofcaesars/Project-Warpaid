using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour
{
    static private PlayerShip _S;
    public int theLifes;
    static public PlayerShip S
    {
        get
        {
            return _S;
        }
        private set
        {
            if (_S != null)
            {
                Debug.LogWarning("Second attempt to set PlayerShip singleton _S.");
            }
            _S = value;
        }
    }

    [Header("Set in Inspector")]
    public float shipSpeed = 10f;
    public GameObject bulletPrefab;

    public static bool jump = false;           // While hiting asteroid and still having jumps
    public static bool invulnerable = false;   // If jumps, then can not be destroyed for 2 sec 
    public Light spaceShipLight;         // Lights turn on while jumping
    public float lightIt = 0;

    Rigidbody rigid;


    //To do: Management, Movement, Firing, Equipment and Customizing;

    void Awake()
    {
        S = this;

        // NOTE: We don't need to check whether or not rigid is null because of [RequireComponent()] above
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Using Horizontal and Vertical axes to set velocity
        float aX = CrossPlatformInputManager.GetAxis("Horizontal");
        float aY = CrossPlatformInputManager.GetAxis("Vertical");

        Vector3 vel = new Vector3(aX, aY);
        if (vel.magnitude > 1)
        {
            // Avoid speed multiplying by 1.414 when moving at a diagonal
            vel.Normalize();
        }

        rigid.velocity = vel * shipSpeed;

        // Mouse input for firing
        // Ends teleport
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            Fire();
            invulnerable = false;
        }

        // Check if hited, then jump to another position
        if (jump == true)
        {
            StartCoroutine(Jump());
        }
    }
    void Fire()
    {
        // Get direction to the mouse
        Vector3 mPos = Input.mousePosition;
        mPos.z = -Camera.main.transform.position.z;
        Vector3 mPos3D = Camera.main.ScreenToWorldPoint(mPos);

        // Instantiate the Bullet and set its direction
        GameObject go = Instantiate<GameObject>(bulletPrefab);
        go.transform.position = transform.position;
        go.transform.LookAt(mPos3D);
    }

    IEnumerator Jump()
    {
        HUDSystems.theJumps -= 1;
        jump = false;
        invulnerable = true;
        enabled = false;

        // Find a good location for the SpaceShip to spawn
        // It's reverse to Asteroid's spawn
        // In fact, it is not good at all, sometimes I can see it too late,
        // so I think immunity and position 0,0 would be a good one
        /*Vector3 pos;
        pos = ScreenBounds.RANDOM_ON_SCREEN_LOC;
        transform.position = pos;
        */
        transform.position = new Vector3(0, 0, 0);
        enabled = true;

        // Changing the color of light
        spaceShipLight.enabled = true;
        lightIt = 0f;
        do
        {
            lightIt += 0.25f;
            spaceShipLight.color = new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f));
            spaceShipLight.range = Random.Range(0.75f, 2.0f);
            yield return new WaitForSeconds(0.25f);
        } while (lightIt != 2.0f && invulnerable == true);
        spaceShipLight.enabled = false;

        invulnerable = false;
    }

    static public float MAX_SPEED
    {
        get
        {
            return S.shipSpeed;
        }
    }
}
