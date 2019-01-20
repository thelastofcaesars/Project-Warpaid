//#define DEBUG_Warpaid_LogMethods

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class Warpaid : MonoBehaviour
{

    // Private Singleton-style instance. Accessed by static property S later in script
    static private Warpaid _S;
    static public List<LevelInfo> LEVEL_LIST;
    static List<PlayerShip> PLAYERS;
    static List<Enemy>  ENEMIES;
    static List<Bullet> BULLETS;
    static List<Item> ITEMS;
    static private bool _PAUSED = false;
    static private eGameState _GAME_STATE = eGameState.mainMenu;
    static public bool GOT_HIGH_SCORE = false;


    // Game Controller
    static Text GAMEOVER_GT;
    //static Text RESTART_GT;
    static Text CASH_GT;
    static Text SCORE_GT;
    public GameObject quitButton;
    // This is an automatic property
    public static int SCORE { get; private set; }
    public static int CASH { get; private set; } // next step is cash for 2 players

    const float DELAY_BEFORE_RELOADING_SCENE = 4;

    public delegate void CallbackDelegate(); // Set up a generic delegate type.
    static public event CallbackDelegate GAME_STATE_CHANGE_DELEGATE;
    static public event CallbackDelegate PAUSED_CHANGE_DELEGATE;

    public delegate void CallbackDelegateV3(Vector3 v); // Set up a Vector3 delegate type.

    // System.Flags changes how eGameStates are viewed in the Inspector and lets multiple 
    //  values be selected simultaneously (similar to how Physics Layers are selected).
    // It's only valid for the game to ever be in one state, but I've added System.Flags
    //  here to demonstrate it and to make the ActiveOnlyDuringSomeGameStates script easier
    //  to view and modify in the Inspector.
    // When you use System.Flags, you still need to set each enum value so that it aligns 
    //  with a power of 2. You can also define enums that combine two or more values,
    //  for example the all value below that combines all other possible values.
    [System.Flags]
    public enum eGameState
    {
        // Decimal      // Binary
        none = 0,       // 00000000
        mainMenu = 1,   // 00000001
        preLevel = 2,   // 00000010
        level = 4,      // 00000100
        postLevel = 8,  // 00001000
        gameOver = 16,  // 00010000
        all = 0xFFFFFFF // 11111111111111111111111111111111
    }

    [Header("Set in Inspector")]
    [Tooltip("This sets the PlayerScriptableObject to be used throughout the game.")]
    public PlayerScriptableObject playersSO;
    [Tooltip("This sets the EnemyScriptableObject to be used throughout the game.")]
    public EnemyScriptableObject enemiesSO;

    // [Header("This will be set by Remote Settings")]
    // public string levelProgression = "1:3/2,2:4/2,3:3/3,4:4/3,5:5/3,6:3/4,7:4/4,8:5/4,9:6/4,10:3/5";


    [Header("These reflect static fields and are otherwise unused")]
    [SerializeField]
    [Tooltip("This private field shows the game state in the Inspector and is set by the "
        + "GAME_STATE_CHANGE_DELEGATE whenever GAME_STATE changes.")]
    protected eGameState _gameState;
    [SerializeField]
    [Tooltip("This private field shows the game state in the Inspector and is set by the "
    + "PAUSED_CHANGE_DELEGATE whenever PAUSED changes.")]
    protected bool _paused;

    private void Awake()
    {
#if DEBUG_Warpaid_LogMethods
        Debug.Log("Warpaid:Awake()");
#endif

        S = this;

        // Rather than use the anonymous delegate that was here previously, I've instead
        //  created a method that conforms to the GAME_STATE_CHANGE_DELEGATE, thereby 
        //  avoiding the memory issues that can be caused by closures.
        GAME_STATE_CHANGE_DELEGATE += GameStateChanged;
        PAUSED_CHANGE_DELEGATE += PauseChanged;

        // Below is another way of doing this that used a C# anonymous delegate.
        // Anonymous delegates are disliked by Unity because of potential memory
        //  leak issues, so I've removed it, though I still wanted to discuss them
        //  here, hence this message and commented code.
        //GAME_STATE_CHANGE_DELEGATE += delegate ()
        //{
        //    // This is an example of a C# anonymous delegate. It's used to set the state of
        //    //  _gameState every time GAME_STATE changes.
        //    // Anonymous delegates like this do create "closures" like "this" below, which 
        //    //  stores the value of this when the anonymous delegate was created. Closures
        //    //  can be slow, but in this case, it is so rarely used that it doesn't matter.
        //    this._gameState = Warpaid.GAME_STATE;
        //};
        //PAUSED_CHANGE_DELEGATE += delegate ()
        //{
        //    this._paused = Warpaid.PAUSED;
        //};

        // This strange use of _gameState and _paused as an intermediary in the following 
        //  lines is solely to stop the Warning from popping up in the Console telling you 
        //  that _gameState was assigned but not used.
        _gameState = eGameState.level;
        GAME_STATE = _gameState;
        _paused = false;
        PauseGame(_paused);
    }

    private void OnDestroy()
    {
        GAME_STATE_CHANGE_DELEGATE -= GameStateChanged;
        PAUSED_CHANGE_DELEGATE -= PauseChanged;
        Warpaid.GAME_STATE = Warpaid.eGameState.none;
    }

    void Start()
    {
#if DEBUG_Warpaid_LogMethods
        Debug.Log("Warpaid:Start()");
#endif

        PLAYERS = new List<PlayerShip>();
        ENEMIES = new List<Enemy>();
        AddScore(0);
        AddCash(0);
        GameObject player = Instantiate(playersSO.partPrefabs[0], new Vector3(0, 0, 0), new Quaternion(0,0,0,0));
        StartCoroutine(SpawnWaves());

        // Loading data needed
    }

    public static string sceneName = "Scene_01";

    void GameStateChanged()
    {
        this._gameState = Warpaid.GAME_STATE;
    }

    void PauseChanged()
    {
        this._paused = Warpaid.PAUSED;
    }


    public void PauseGameToggle()
    {
        PauseGame(!PAUSED);
    }

    public void PauseGame(bool toPaused)
    {
        PAUSED = toPaused;
        if (PAUSED)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }


    private void Update()
    {
        if (GAME_STATE == eGameState.level && ENEMIES.Count == 0)
        {
            // The player has destroyed all the Enemies and completed this level
            if (_S != null)
            {
                // _S.EndLevel(); needed
            }
        }
    }


    public void EndGame()
    {
        GAME_STATE = eGameState.gameOver;
        Invoke("ReloadScene", DELAY_BEFORE_RELOADING_SCENE);
    }


    void ReloadScene()
    {
        // Reload the scene to restart the game
        // Note: This exposes a long-time Unity bug where reloading the scene 
        //  during gameplay within the Editor causes the lighting to all go 
        //  dark and the engine to think that it needs to rebuild the lighting.
        //  This bug does not cause any issues outside of the Editor.
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    static public void AddEnemy(Enemy enemy)
    {
        if (ENEMIES.IndexOf(enemy) == -1)
        {
            ENEMIES.Add(enemy);
            enemy.score = EnemiesSO.pointsForEnemyLevel[1];
        }
    }
    static public void RemoveEnemy(Enemy enemy)
    {
        if (GAME_STATE != eGameState.level)
        {
            // If this is not in the middle of a level, don't do anything. RemoveEnemy is called
            //  by Enemy:OnDestroy(), so this prevents removal from happening if the game is in
            //  any state other than level, which avoids modifying the ENEMIES List in the for 
            //  loop of ClearEnemies().
            return;
        }
        if (ENEMIES.IndexOf(enemy) != -1)
        {
            ENEMIES.Remove(enemy);
        }
    }

    static public void AddBullet(Bullet bullet)
    {
        if (BULLETS == null)
        {
            BULLETS = new List<Bullet>();
        }
        if (BULLETS.IndexOf(bullet) == -1)
        {
            BULLETS.Add(bullet);
        }
    }

    static public void RemoveBullet(Bullet bullet)
    {
        if (BULLETS == null)
        {
            return;
        }
        BULLETS.Remove(bullet);
    }
    static public void AddItem(Item item)
    {
        if (ITEMS == null)
        {
            ITEMS = new List<Item>();
        }
        if (ITEMS.IndexOf(item) == -1)
        {
            ITEMS.Add(item);
        }
    }

    static public void RemoveItem(Item item)
    {
        if (ITEMS == null)
        {
            return;
        }
        ITEMS.Remove(item);
    }


    // ---------------- Static Section ---------------- //

    /// <summary>
    /// <para>This static private property provides some protection for the Singleton _S.</para>
    /// <para>get {} does return null, but throws an error first.</para>
    /// <para>set {} allows overwrite of _S by a 2nd instance, but throws an error first.</para>
    /// <para>Another advantage of using a property here is that it allows you to place
    /// a breakpoint in the set clause and then look at the call stack if you fear that 
    /// something random is setting your _S value.</para>
    /// </summary>
    static private Warpaid S
    {
        get
        {
            if (_S == null)
            {
                Debug.LogError("Warpaid:S getter - Attempt to get value of S before it has been set.");
                return null;
            }
            return _S;
        }
        set
        {
            if (_S != null)
            {
                Debug.LogError("Warpaid:S setter - Attempt to set S when it has already been set.");
            }
            _S = value;
        }
    }

    static public PlayerScriptableObject PlayersSO
    {
        get
        {
            if (S!= null)
            {
                return S.playersSO;
            }
            return null;
        }
    }

    static public EnemyScriptableObject EnemiesSO
    {
        get
        {
            if (S != null)
            {
                return S.enemiesSO;
            }
            return null;
        }
    }

    static public bool PAUSED
    {
        get
        {
            return _PAUSED;
        }
        private set
        {
            if (value != _PAUSED)
            {
                _PAUSED = value;
                // Need to update all of the handlers
                // Any time you use a delegate, you run the risk of it not having any handlers
                //  assigned to it. In that case, it is null and will throw a null reference
                //  exception if you try to call it. So *any* time you call a delegate, you 
                //  should check beforehand to make sure it's not null.
                if (PAUSED_CHANGE_DELEGATE != null)
                {
                    PAUSED_CHANGE_DELEGATE();
                }
            }

        }
    }

    static public eGameState GAME_STATE
    {
        get
        {
            return _GAME_STATE;
        }
        set
        {
            if (value != _GAME_STATE)
            {
                _GAME_STATE = value;
                // Need to update all of the handlers
                // Any time you use a delegate, you run the risk of it not having any handlers
                //  assigned to it. In that case, it is null and will throw a null reference
                //  exception if you try to call it. So *any* time you call a delegate, you 
                //  should check beforehand to make sure it's not null.
                if (GAME_STATE_CHANGE_DELEGATE != null)
                {
                    GAME_STATE_CHANGE_DELEGATE();
                }
            }
        }
    }

    // This is called an automatic property. It allows protection of a static field and automatically
    //  generates a static field that it reads and writes.
    static public int GAME_LEVEL
    {
        get; private set;
    }

    static public void StartGame()
    {
        GOT_HIGH_SCORE = false;
        GAME_LEVEL = 0;
    }

    static public void GameOver()
    {
        _S.EndGame();
    }


    [System.Serializable]
    public struct LevelInfo
    {
        public int levelNum;
        public string levelName;
        public int numInitialEnemy;
        public int numSubEnemy;

        public LevelInfo(int lNum, string name, int initial, int sub)
        {
            levelNum = lNum;
            levelName = name;
            numInitialEnemy = initial;
            numSubEnemy = sub;
        }
    }


    static public LevelInfo GetLevelInfo(int lNum = -1)
    {
        if (lNum == -1)
        {
            lNum = GAME_LEVEL;
        }
        // lNum is 1-based where LEVEL_LIST is 0-based, so LEVEL_LIST[0] is lNum 1
        if (lNum < 1 || lNum > LEVEL_LIST.Count)
        {
            Debug.LogError("Warpaid:GetLevelInfo() - Requested level number of " + lNum + " does not exist.");
            return new LevelInfo(-1, "NULL", 1, 1);
        }
        return (LEVEL_LIST[lNum - 1]);
    }


    static public void AddScore(int num)
    {
        // Find the ScoreGT Text field only once.
        if (SCORE_GT == null)
        {
            GameObject go = GameObject.Find("ScoreGT");
            if (go != null)
            {
                SCORE_GT = go.GetComponent<Text>();
            }
            else
            {
                Debug.LogError("Warpaid:AddScore() - Could not find a GameObject named ScoreGT.");
                return;
            }
            SCORE = 0;
        }
        // SCORE holds the definitive score for the game.
        SCORE += num;

        if (!GOT_HIGH_SCORE)
        {
            // We just got the high score
            GOT_HIGH_SCORE = true;
        }

        SCORE_GT.text = SCORE.ToString("N0");

    }

    static public void AddCash(int num)
    {
        // Find the ScoreGT Text field only once.
        if (CASH_GT == null)
        {
            GameObject go = GameObject.Find("CashGT");
            if (go != null)
            {
                CASH_GT = go.GetComponent<Text>();
            }
            else
            {
                Debug.LogError("Warpaid:AddCash() - Could not find a GameObject named CashGT.");
                return;
            }
            CASH = 0;
        }
        // CASH holds the definitive cash for the player.
        CASH += num;

        CASH_GT.text = "$" + CASH.ToString();

    }

    static public void InitDrop(float probability, Transform trans) // need to add drop from special one SO, this
    {
        if(S == null)
        {
            Debug.Log("Warpaid:InitDrop - attempt to get value before it has been set!");
            return;
        }
        if (S.enemiesSO.enemyDropPrefabs.Length == 0)
        {
            Debug.Log("There is no drop from this enemy!");
            return;
        }
        if (Random.Range(0, 100) < probability)
        {    
            GameObject go = Instantiate(EnemiesSO.GetEnemyDropPrefab(), trans.position, trans.rotation);
            Debug.Log("Inited " + go.name);
        }
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(3);
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject enemy = EnemiesSO.GetEnemyPrefab();
                Vector3 spawnPosition = new Vector3(Random.Range(-14, 14), 0, 20);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(enemy, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(1);
            }
            if (_GAME_STATE == eGameState.gameOver)
            {
                break;
            }
            yield return new WaitForSeconds(3);
        }
    }
}
