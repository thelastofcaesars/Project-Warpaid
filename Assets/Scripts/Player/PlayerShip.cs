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

    public float bulletRate = 1.1f;
    public float nextFire = 0f;
    // boosts
    public float bulletTime = 0.1f;
    public float reflex = 0.1f;
    public float speedBoost = 0.05f;
    public float freezeTime = 0f;
    public float energy = 0f;
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

    public static int staticLifes = 2;
    public static int staticArmors = 0;

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
            nextFire = Time.time + bulletRate - bulletTime;
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
        lifes = Warpaid.PlayersSO.playerLifes;
        armors = Warpaid.PlayersSO.playerArmors;
        bulletRate = Warpaid.PlayersSO.bulletDelta;
        reflex = Warpaid.PlayersSO.reflex;
        speedBoost = Warpaid.PlayersSO.speedBoost;
        energy = Warpaid.PlayersSO.energy;

        //  | - Needed to change Item life = new Item()!
        for (int i = 0; i < lifes; i++)
        {
            Item life = new Item
            {
                itemType = Item.eItemType.Heart,
                itemID = "00H1"
            };
            AddLife(life);
        }
        for (int i = 0; i < armors; i++)
        {
            Item armor = new Item
            {
                itemType = Item.eItemType.Armor,
                itemID = "00A1"
            };
            AddLife(armor);
        }

        // innocent
        transform.name = playerName = Warpaid.PlayersSO.playerName;
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
        bool addScore = false;
        switch (orb.itemID)
        {
            // orbs
            case "00HA":
                if (!orbSystem.Red)
                    orbSystem.Red = true;
                else
                {
                    addScore = true;
                }
                break;
            case "00HB":
                if (!orbSystem.Orange)
                    orbSystem.Orange = true;
                else
                {
                    addScore = true;
                }
                break;
            case "00HC":
                if (!orbSystem.Yellow)
                    orbSystem.Yellow = true;
                else
                {
                    addScore = true;
                }
                break;
            case "00HD":
                if (!orbSystem.Green)
                    orbSystem.Green = true;
                else
                {
                    addScore = true;
                }
                break;
            case "00HE":
                if (!orbSystem.Blue)
                    orbSystem.Blue = true;
                else
                {
                    addScore = true;
                }
                break;
            case "00HF":
                if (!orbSystem.Purple)
                    orbSystem.Purple = true;
                else
                    addScore = true;
                break;
            case "00HH":
                if (!orbSystem.White)
                    orbSystem.White = true;
                else
                {
                    addScore = true;
                }
                break;
            default:
                Debug.Log("Playership:AddOrb - ID of item has not been set");
                break;
        }
        if (addScore)
        {
            Warpaid.AddScore(orb.value);
            RandomEffect();
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
        bool addScore = false;
        switch (letter.itemID)
        {
            // snafu
            case "00LS":
                if (!snafuSystem.S)
                {
                    snafuSystem.S = true;
                }
                else
                {
                    addScore = true;
                }
                break;
            case "00LN":
                if (!snafuSystem.N)
                {
                    snafuSystem.N = true;
                }
                else
                {
                    addScore = true;
                }
                break;
            case "00LA":
                if (!snafuSystem.A)
                {
                    snafuSystem.A = true;
                }
                else
                {
                    addScore = true;
                }
                break;
            case "00LF":
                if (!snafuSystem.F)
                {
                    snafuSystem.U = true;
                }
                else
                {
                    addScore = true;
                }
                break;
            case "00LU":
                if (!snafuSystem.U)
                {
                    snafuSystem.U = true;
                }
                else
                {
                    addScore = true;
                }
                break;

            // other letters, bullet time etc.

            case "00LR":
                snafuSystem.R = true;
                if (!(S.freezeTime >= 1f))
                {
                    S.freezeTime += 0.1f;
                }
                else
                {
                    S.freezeTime = 1f;
                    addScore = true;
                }
                break;
            case "00LT":
                if (!(S.reflex >= 1f))
                {
                    S.reflex += 0.05f;
                }
                else
                {
                    S.reflex = 1f;
                    addScore = true;
                }
                break;
            case "00LB":
                if (!(S.bulletTime >= 1f))
                {
                    S.bulletTime += 0.05f;
                }
                else
                {
                    S.bulletTime = 1f;
                    addScore = true;
                }
                break;
            case "00LV":
                if (!(S.speedBoost >= 1f))
                {
                    S.speedBoost += 0.05f;
                }
                else
                {
                    S.speedBoost = 1f;
                    addScore = true;
                }
                break;
            case "00LE":
                if (!(S.energy >= 1f))
                {
                    S.energy += 0.2f;
                }
                else
                {
                    S.energy = 1f;
                    addScore = true;
                }
                //adding some energy ?/!?
                break;
            case "00L?":
                RandomEffect();
                break;
            default:
                Debug.Log("Playership:AddLetter - ID of item has not been set");
                break;
        }
        if(addScore)
        {
            Warpaid.AddScore(letter.value);
            RandomEffect();
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
                RandomEffect();
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
                RandomEffect();
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
        if (S == null)
        {
            Debug.Log("PlayerShip:GetPlayerBoosts - attempt to get S value before it has been set!");
            return boostSystem; // GetPlayerBoosts();
        }
        boostSystem.bulletTime = S.bulletTime;
        boostSystem.speedBoost = S.speedBoost;
        boostSystem.reflex = S.reflex; // to change
        boostSystem.energy = S.energy;
        boostSystem.freezeTime = S.freezeTime;
        return boostSystem;
    }
    #endregion

    static public void RandomEffect()
    {
        Debug.Log("Some random effect to add");
    }
}
