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

    // to private in future, in editor only
    public string playerName = "Player";
    public  int lifes = 2;
    public  int armors = 0;
    public float energy = 0f;
    public float bulletRate = 1f;
    public float nextFire = 0f;
    public float playerReflex = 2f;
    //

    [Header("Set in Inspector")]
    public float shipSpeed = 1.0f;
    public float tilt;

    static List <Item> LIFES;
    static List <Item> ARMORS;

    public Transform[] shotSpawns;

    public Boundary boundary;

    public GameObject coreRotator;
    public ParticleSystem[] particleSystems; // to change get from pSO

    //To do: Management, Movement, Firing, Equipment and Customizing;

    void Awake()
    {
        S = this;

        // NOTE: We don't need to check whether or not rigid is null because of [RequireComponent()] above
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        GetAllDataFromSO();
        
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            Fire();
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (CrossPlatformInputManager.GetButtonDown("Skill") && stateInfo.fullPathHash == idleStateHash)
        {
            Skill();
        }
    }
    void FixedUpdate()
    {
        Move();
    }
    void Skill()
    {
        StartCoroutine(SkillManagement.IERotateAroundCenter(anim));
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
        coreRotator.transform.Rotate(new Vector3(0f, 0f, aZ));
        foreach(ParticleSystem ps in particleSystems)
        {
            ParticleSystem.MainModule main = ps.main;
            if (aZ > 0) main.startSpeed = aZ * aZ * 0.5f;
            else if (aZ <= 0) main.startSpeed = 0.3f;
        }
    }
    void Fire()
    {
        if (Time.time > nextFire)
        {
            // Instantiate the Bullet and set its direction
            nextFire = Time.time + bulletRate;
            foreach (var shotSpawn in shotSpawns)
            {
                Instantiate(Warpaid.PlayersSO.playerBullet, shotSpawn.position, shotSpawn.rotation);
            }
            audio.Play();
        }
    }

    void GetAllDataFromSO()
    {
        // changable data in-game
        bulletRate = Warpaid.PlayersSO.bulletDelta;
        playerReflex = Warpaid.PlayersSO.playerReflex;
        lifes = Warpaid.PlayersSO.playerLifes;
        armors = Warpaid.PlayersSO.playerArmors;
        energy = Warpaid.PlayersSO.playerEnergy;

        // innocent
        playerName = Warpaid.PlayersSO.playerName;
    }
    #region Adding Items/Lifes
    static public void AddLife(Item life)
    {
        if (LIFES == null)
        {
            LIFES = new List<Item>();
        }
        if (LIFES.IndexOf(life) == -1)
        {
            LIFES.Add(life);
        }
    }

    static public void RemoveLife(Item life)
    {
        if (LIFES == null)
        {
            return;
        }
        LIFES.Remove(life);
    }

    static public void AddArmor(Item armor)
    {
        if (ARMORS == null)
        {
            ARMORS = new List<Item>();
        }
        if (ARMORS.IndexOf(armor) == -1)
        {
            ARMORS.Add(armor);
        }
    }

    static public void RemoveArmor(Item armor)
    {
        if (ARMORS == null)
        {
            return;
        }
        ARMORS.Remove(armor);
    }
    #endregion

#region Get
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
#endregion
}
