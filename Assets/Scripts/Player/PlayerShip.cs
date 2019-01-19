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
    public int lifes = 2;
    public int armors = 0;
    public float energy = 0f;
    public float bulletRate = 1f;
    public float nextFire = 0f;
    public float playerReflex = 2f;
    public float speedBoost = 1.5f;
    public float freezeTime = 0f;
    //

    [Header("Set in Inspector")]
    public float shipSpeed = 1.0f;
    public float tilt;

    static public List<Item> LIFES;
    static public List<Item> ARMORS;
    static public snafu snafuSystem;
    static public orb orbSystem;
    static public boost boostSystem;

    public Transform[] shotSpawns;

    public Boundary boundary;

    public GameObject coreRotator;
    public ParticleSystem[] particleSystems; // to change get from pSO

    //To do: Management, Movement, Firing, Equipment and Customizing;

    public static int staticLifes= 2;
    public static int staticArmors = 0;

    void Awake()
    {
        S = this;

        // NOTE: We don't need to check whether or not rigid is null because of [RequireComponent()] above
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        //GetAllDataFromSO();
        
        //  | - Needed to change Item life = new Item()!
        //  V 
        for (int i = 0; i < lifes; i++)
        {
            Item life = new Item();
            life.itemType = Item.eItemType.Heart;
            life.itemID = "00H1";
            AddItem(life);
        }
        for (int i = 0; i < armors; i++)
        {
            Item armor = new Item();
            armor.itemType = Item.eItemType.Armor;
            armor.itemID = "00A1";
            AddItem(armor);
        }
       
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
        staticArmors = armors;
        staticLifes = lifes;
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
        foreach (ParticleSystem ps in particleSystems)
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
        for (int i = 0; i < lifes; i++)
        {
            LIFES.Add(new Item());
        }
        for (int i = 0; i < armors; i++)
        {
            ARMORS.Add(new Item());
        }
        // innocent
        playerName = Warpaid.PlayersSO.playerName;
    }

    #region Adding Items/Lifes etc.

    // static metod to adding whatever you want to

    static public void AddItem(Item item)
    {
        //Debug.Log("Item " + item + " ID " + item.itemID);
        switch (item.itemType)
        {
            case Item.eItemType.Heart:
                if (item.itemID == "00H1")
                {
                    AddLife(item);                 
                }
                else
                {
                    AddOrb(item);
                }
                break;

            case Item.eItemType.Armor:
                if (item.itemID == "00A1")
                {
                    AddArmor(item);
                }
                break;

            case Item.eItemType.Cash:
                Debug.Log("Added cash: " + item.value);
                Warpaid.AddCash(item.value);
                break;

            case Item.eItemType.Points:
                Debug.Log("Added points: " + item.value);
                Warpaid.AddScore(item.value);
                // need to create text-value particle;-> method
                // GameObject particle = Instantiate(Particle.GetParticlePrefab(pa0))
                break;

            case Item.eItemType.Letter:
                AddLetter(item);
                break;   

            case Item.eItemType.none:
            case Item.eItemType.all:
                Debug.LogWarning("Type of item has not been set!");
                break;
        }
        HUDSystems.UpdateInventory();
    }
    // maybe one bool to change nor?

    static public void AddOrb(Item orb)
    {
        switch (orb.itemID)
        {
            // orbs
            case "00HA":
                if (!orbSystem.Red)
                    orbSystem.Red = true;
                else
                {
                    Warpaid.AddScore(orb.value);
                }
                break;
            case "00HB":
                if (!orbSystem.Orange)
                    orbSystem.Orange = true;
                else
                {
                    Warpaid.AddScore(orb.value);
                }
                break;
            case "00HC":
                if (!orbSystem.Yellow)
                    orbSystem.Yellow = true;
                else
                {
                    Warpaid.AddScore(orb.value);
                }
                break;
            case "00HD":
                if (!orbSystem.Green)
                    orbSystem.Green = true;
                else
                {
                    Warpaid.AddScore(orb.value);
                }
                break;
            case "00HE":
                if (!orbSystem.Blue)
                    orbSystem.Blue = true;
                else
                {
                    Warpaid.AddScore(orb.value);
                }
                break;
            case "00HF":
                if (!orbSystem.Purple)
                    orbSystem.Purple = true;
                else
                    Warpaid.AddScore(orb.value);
                break;
            case "00HH":
                if (!orbSystem.White)
                    orbSystem.White = true;
                else
                {
                    Warpaid.AddScore(orb.value);
                }
                break;
            default:
                Debug.Log("Playership:AddOrb - ID of item has not been set");
                break;
        }
    }

    static public void RemoveOrb(Item orb)
    {
        switch (orb.itemID)
        {
            // orbs
            case "00HA":
                orbSystem.Red = false;
                break;
            case "00HB":
                orbSystem.Orange = false;
                break;
            case "00HC":
                orbSystem.Yellow = false;
                break;
            case "00HD":
                orbSystem.Green = false;
                break;
            case "00HE":
                orbSystem.Blue = false;
                break;
            case "00HF":
                orbSystem.Purple = false;
                break;
            case "00HH":
                orbSystem.White = false;
                break;
            default:
                Debug.Log("Playership:AddOrb - ID of item has not been set");
                break;
        }
        HUDSystems.UpdateInventory();
    }

    static public void AddLetter(Item letter)
    {
        switch (letter.itemID)
        {
            // snafu
            case "00LS":
                snafuSystem.S = true;
                break;
            case "00LN":
                snafuSystem.N = true;
                break;
            case "00LA":
                snafuSystem.A = true;
                break;
            case "00LF":
                snafuSystem.F = true;
                break;
            case "00LU":
                snafuSystem.U = true;
                break;
            
            // other letters, bullet time etc.

            case "00LR":
                snafuSystem.R = true;
                S.freezeTime += 0.1f;
                break;
            case "00LT":
                S.playerReflex += 0.05f;
                break;
            case "00LB":
                S.bulletRate += 0.05f;
                break;
            default:
                Debug.Log("Playership:AddLetter - ID of item has not been set");
                break;
        }
    }

    static public void RemoveLetter(Item letter)
    {
        switch (letter.itemID)
        {
            // snafu
            case "00LS":
                snafuSystem.S = false;
                break;
            case "00LN":
                snafuSystem.N = false;
                break;
            case "00LA":
                snafuSystem.A = false;
                break;
            case "00LF":
                snafuSystem.F = false;
                break;
            case "00LU":
                snafuSystem.U = false;
                break;

            // other letters, bullet time etc.

            case "00LR":
                snafuSystem.R = false;
                //RemoveRefrigerator(item); // name in progress;
                break;
            case "00LT":
                //RemoveShipTime(item); // name in progress
                break;
            case "00LB":
                //RemoveBulletTime(item); // name in progress
                break;
            default:
                Debug.Log("Playership:AddLetter - ID of item has not been set");
                break;
        }
        HUDSystems.UpdateInventory();
    }

    static public void AddLife(Item life)
    {
        if (LIFES == null)
        {
            LIFES = new List<Item>();
        }
        if (LIFES.IndexOf(life) == -1)
        {
            if(LIFES.Count == 3)
            {
                Warpaid.AddScore(life.value);
                // need to create particle with value; another method you know
                return;
            }
            LIFES.Add(life);
            S.lifes = LIFES.Count;
        }
    }

    static public void RemoveLife(Item life)
    {
        if (LIFES == null)
        {
            return;
        }
        LIFES.Remove(life);
        S.lifes = LIFES.Count;
        HUDSystems.UpdateInventory();
    }

    static public void AddArmor(Item armor)
    {
        if (ARMORS == null)
        {
            ARMORS = new List<Item>();
        }
        if (ARMORS.IndexOf(armor) == -1)
        {
            if (ARMORS.Count == 2)
            {
                Warpaid.AddScore(armor.value);
                // need to create particle with value; another method you know
                return;
            }
            ARMORS.Add(armor);
            S.armors = ARMORS.Count;
        }
    }

    static public void RemoveArmor(Item armor)
    {
        if (ARMORS == null)
        {
            return;
        }
        ARMORS.Remove(armor);
        S.armors = ARMORS.Count;
        HUDSystems.UpdateInventory();
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

    static public boost GetPlayerBoosts()
    {
        boostSystem.bulletTime = S.bulletRate;
        boostSystem.speedBoost = S.speedBoost;
        boostSystem.time = S.playerReflex; // to change
        boostSystem.energy = S.energy;
        boostSystem.freezeTime = S.freezeTime;
        return boostSystem;
    }
    #endregion
}
