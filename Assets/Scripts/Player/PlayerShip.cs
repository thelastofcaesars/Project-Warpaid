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
    // standard position
    private Vector3 standardPos;
    private bool invulnearbility;
    private float size = 1;
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
    public ParticleSystem[] rotatorsParticles; // to change get from pSO
    private GameObject[] particleSystems;
    private bool snafuSystemCompletion = false;

    //To do: Management, Movement, Firing, Equipment and Customizing;


    void Awake()
    {
        S = this;

        // NOTE: We don't need to check whether or not rigid is null because of [RequireComponent()] above
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        GetAllDataFromSO();

        standardPos = transform.position;
        HUDSystems.UpdateInventory();
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && !Warpaid.PAUSED)
        {
            invulnearbility = false;
            Fire();
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (CrossPlatformInputManager.GetButtonDown("Skill") && stateInfo.fullPathHash == idleStateHash)
        {
            Skill();
        }

        if (CrossPlatformInputManager.GetButtonDown("PauseGame"))
        {
            Warpaid.PauseGameByKey();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter(Collider coll)
    {
        GameObject collGO = coll.gameObject;
        if (collGO.CompareTag("Enemy"))
        {
            if (!invulnearbility)
            {
                CheckLifeStatus();
            }
            Destroy(collGO);
        }
    }

    private void OnDestroy()
    {
        Warpaid.GAME_STATE = Warpaid.eGameState.gameOver;
    }

    void CheckLifeStatus()
    {
        if (armors > 0)
        {
            RemoveArmor();
            InstantiateParticleSystem(4); //armorParticle
            StartCoroutine(GetInvulnerability(2f));
        }
        else if(lifes > 0)
        {
            RemoveLife(); // can type heart instead
            InitShipRespawn();
            StartCoroutine(GetInvulnerability(2f));
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void InitShipRespawn()
    {
        InstantiateParticleSystem(3); //deathParticle
        transform.position = standardPos;
        InstantiateParticleSystem(2); //respawnParticle
    }
    void InstantiateParticleSystem(int ndx)
    {
        if (particleSystems.Length <= ndx)
            return;
        GameObject particleGO = Instantiate<GameObject>(particleSystems[ndx], transform.position, Quaternion.identity);
        ParticleSystem particleSys = particleGO.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleSys.main;
        main.startLifetimeMultiplier = size * 0.5f;
        ParticleSystem.EmissionModule emitter = particleSys.emission;
        ParticleSystem.Burst burst = emitter.GetBurst(0);
        ParticleSystem.MinMaxCurve burstCount = burst.count;
        burstCount.constant = burstCount.constant * size;
        burst.count = burstCount;
        emitter.SetBurst(0, burst);
        Destroy(particleGO, 4f);
    }
    IEnumerator GetInvulnerability(float sec)
    {
        invulnearbility = true;
        yield return new WaitForSeconds(sec);
        invulnearbility = false;
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
        rigid.velocity = vel * shipSpeed * (1 + speedBoost);
        rigid.position = new Vector3
        (
            Mathf.Clamp(rigid.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rigid.position.z, boundary.zMin, boundary.zMax)
        );
        rigid.rotation = Quaternion.Euler(0.0f, 0.0f, rigid.velocity.x * -tilt);
        coreRotator.transform.Rotate(new Vector3(0f, 0f, aZ));
        foreach (ParticleSystem ps in rotatorsParticles)
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

        particleSystems = Warpaid.PlayersSO.particlePrefabs;
        for (int i = 0; i < lifes; i++)
        {
            AddLife(DropScriptableObject.GetInventoryItem_SM("life").GetComponent<Item>());
        }
        for (int i = 0; i < armors; i++)
        {
            AddArmor(DropScriptableObject.GetInventoryItem_SM("armor").GetComponent<Item>());
        }

        // innocent
        transform.name = playerName = Warpaid.PlayersSO.playerName; // save's name
    }

    static public void DirectToReplace(PlayerInfo playerInfo)
    {
        S.lifes = playerInfo.lifes;
        S.armors = playerInfo.armors;
        S.bulletRate = playerInfo.bulletRate;
        S.bulletTime = playerInfo.bulletTime;
        S.reflex = playerInfo.reflex;
        S.speedBoost = playerInfo.speedBoost;
        S.freezeTime = playerInfo.freezeTime;
        S.energy = playerInfo.energy;
        for (int i = 0; i < S.lifes; i++)
        {
            AddLife(DropScriptableObject.GetInventoryItem_SM("life").GetComponent<Item>());
        }
        for (int i = 0; i < S.armors; i++)
        {
            AddArmor(DropScriptableObject.GetInventoryItem_SM("armor").GetComponent<Item>());
        }
        HUDSystems.UpdateInventory();
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
                // Debug.Log("Added cash: " + item.value);
                Warpaid.AddCash(item.value);
                break;

            case Item.eItemType.Points:
                // Debug.Log("Added points: " + item.value);
                Warpaid.AddScore(item.value);
                TextEffect(item.value, item.transform);
                break;

            case Item.eItemType.Letter:
                AddLetter(item);
                // Notify the AchievementManager that something has happened
                AchievementManager.NotifyAchievementManager(item);
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
            case "00HG":
                if (!orbSystem.White)
                    orbSystem.White = true;
                else
                {
                    addScore = true;
                }
                break;
            default:
                Debug.Log("Playership:AddOrb - ID of item has not been set. ID: " + orb.itemID);
                break;
        }
        if (addScore)
        {
            Warpaid.AddScore(orb.value);
            TextEffect(orb.value, orb.transform);
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
                    S.SnafuComplete();
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
                    S.SnafuComplete();
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
                    S.SnafuComplete();
                }
                else
                {
                    addScore = true;
                }
                break;
            case "00LF":
                if (!snafuSystem.F)
                {
                    snafuSystem.F = true;
                    S.SnafuComplete();
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
                    S.SnafuComplete();
                }
                else
                {
                    addScore = true;
                }
                break;

            // other letters, bullet time etc.

            case "00LR":
                if (S.freezeTime < 1f)
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
                if (S.reflex < 1f)
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
                if (S.bulletTime < 1f)
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
                if (S.speedBoost < 1f)
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
                if (S.energy < 1f)
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
            TextEffect(letter.value, letter.transform);
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
                TextEffect(life.value, life.transform);
                return;
            }
            LIFES.Add(life);
            S.lifes = LIFES.Count;
        }
    }

    static public void RemoveLife()
    {
        if (LIFES == null)
        {
            return;
        }
        LIFES.RemoveAt(LIFES.Count - 1);
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
                TextEffect(armor.value, armor.transform);
                return;
            }
            ARMORS.Add(armor);
            S.armors = ARMORS.Count;
        }
    }

    static public void RemoveArmor()
    {
        if (ARMORS == null)
        {
            return;
        }
        ARMORS.RemoveAt(ARMORS.Count - 1);
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

    static public void TextEffect(float num, Transform trans)
    {
        if (Warpaid.GetTextParticle() == null)
            return;
        GameObject particleGO = Instantiate<GameObject>(Warpaid.GetTextParticle(), S.transform.position, Quaternion.identity);
        ParticleSystem particleSys = particleGO.GetComponent<ParticleSystem>();
        Warpaid.PARTICLE_GT.text = num.ToString("N0");
        Destroy(particleGO, 2f);
    }
    static public void RandomEffect()
    {
        Debug.Log("Here is some random effect");
    }

    public void SnafuComplete()
    {
        if(!snafuSystemCompletion)
        {
            if(snafuSystem.S && snafuSystem.N && snafuSystem.A && snafuSystem.F && snafuSystem.U)
            {
                snafuSystemCompletion = true;
                AchievementManager.AchievementStep(Achievement.eStepType.snafuCompleted, 1);
                if (LIFES.Count < 3)
                {
                    AddLife(DropScriptableObject.GetInventoryItem_SM("life").GetComponent<Item>()); // can type heart instead
                    snafuSystem.S = snafuSystem.N = snafuSystem.A = snafuSystem.F = snafuSystem.U = false;
                    snafuSystemCompletion = false;
                    HUDSystems.UpdateInventory();
                }
                else StartCoroutine(WaitForSnafu());
            }
        }
    }

    IEnumerator WaitForSnafu()
    {
        while(snafuSystemCompletion)
        {
            if (LIFES.Count < 3)
            {
                AddLife(DropScriptableObject.GetInventoryItem_SM("life").GetComponent<Item>()); // can type heart instead
                snafuSystem.S = snafuSystem.N = snafuSystem.A = snafuSystem.F = snafuSystem.U = false;
                snafuSystemCompletion = false;
                HUDSystems.UpdateInventory();
            }
            yield return new WaitForSeconds(2);
        }
    }

}
[System.Serializable]
public class PlayerInfo
{
    public int lifes = 1;
    public int armors = 0;
    public float bulletRate = 1f;
    public float bulletTime = 0.1f;
    public float reflex = 0.1f;
    public float speedBoost = 0.05f;
    public float freezeTime = 0f;
    public float energy = 0f;

    public PlayerInfo(int lifes, int armors, float bulletRate, float bulletTime, float reflex, float speedBoost, float freezeTime, float energy)
    {
        this.lifes = lifes;
        this.armors = armors;
        this.bulletRate = bulletRate;
        this.bulletTime = bulletTime;
        this.reflex = reflex;
        this.speedBoost = speedBoost;
        this.freezeTime = freezeTime;
        this.energy = energy;
    }
}
