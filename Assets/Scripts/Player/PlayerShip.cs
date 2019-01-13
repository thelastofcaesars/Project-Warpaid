using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour
{
    static private PlayerShip _S;
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
    private new AudioSource audio;
    private Rigidbody rigid;
    private Animator anim;

    private int idleStateHash = Animator.StringToHash("Base Layer.Idle");
    public static bool canI = true;
    private float nextFire;
    [Header("Set in Inspector")]
    public float shipSpeed = 1.0f;
    public float tilt;
    public int lifes;

    public float bulletRate;
    public GameObject shot;
    public Transform[] shotSpawns;

    public Boundary boundary;
    private static int rotateHash = Animator.StringToHash("Rotate");
    //To do: Management, Movement, Firing, Equipment and Customizing;

    void Awake()
    {
        S = this;

        // NOTE: We don't need to check whether or not rigid is null because of [RequireComponent()] above
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        canI = true;
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            Fire();
        }

        Skill();
        
    }
    void FixedUpdate()
    {
        Move();
    }
    void Skill()
    {
    
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (Input.GetKeyDown(KeyCode.K))
        {
            canI = false;
            Debug.Log(stateInfo.ToString());
            IERotateAroundCenter(anim);
          //  SkillManagement.RotateAroundCenter();
            Debug.Log(anim.GetCurrentAnimatorStateInfo(0).ToString());
        }

    }
    void Move()
    {
        // Using Horizontal and Vertical axes to set velocity
        float aX = CrossPlatformInputManager.GetAxis("Horizontal");
        float aZ = CrossPlatformInputManager.GetAxis("Vertical");

        Vector3 vel = new Vector3(aX, 0.0f, aZ);
        if (vel.magnitude > 1)
        {
            // Avoid speed multiplying by 1.414 when moving at a diagonal
            vel.Normalize();
        }
        rigid.velocity = vel * shipSpeed;
        rigid.position = new Vector3
        (
            Mathf.Clamp(rigid.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rigid.position.z, boundary.zMin, boundary.zMax)
        );
        rigid.rotation = Quaternion.Euler(0.0f, 0.0f, rigid.velocity.x * -tilt);
    }
    void Fire()
    {
        if (Time.time > nextFire)
        {
            // Instantiate the Bullet and set its direction
            nextFire = Time.time + bulletRate;
            foreach (var shotSpawn in shotSpawns)
            {
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            }
            audio.Play();
        }
    }

    static public float MAX_SPEED
    {
        get
        {
            return S.shipSpeed;
        }
    }
    static public Animator GetAnimator()
    {
        return S.anim;
    }
    static public PlayerShip GetPlayerShip()
    {
        return S;
    }
    public IEnumerator IERotateAroundCenter(Animator anim)
    {
        anim.GetComponent<Animator>().SetFloat(rotateHash, 1f);
        Debug.Log("wtf" + anim.GetCurrentAnimatorStateInfo(0).ToString());
        yield return new WaitForSeconds(2);
        anim.GetComponent<Animator>().SetFloat(rotateHash, 0f);
        PlayerShip.canI = true;
    }
}
