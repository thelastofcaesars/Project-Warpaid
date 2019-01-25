using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDSystems : MonoBehaviour
{
    static private HUDSystems _S;
    static public HUDSystems S
    {
        get
        {
            return _S;
        }
        private set
        {
            if (_S != null)
            {
                Debug.LogWarning("Second attempt to set HUDSystems singleton _S.");
            }
            _S = value;
        }
    }

    /*
    // Wanted in future
    static Dictionary<string, Image> DICT_GUI_ARMORS;
    static Dictionary<string, Image> DICT_GUI_LIFES;
    static Dictionary<string, Image> DICT_GUI_LETTERS;
    static Dictionary<string, Image> DICT_GUI_ORBS;
    static Dictionary<string, Image> DICT_GUI_BOOSTS;
    //
    */

    public Image[] lifes;
    public Image[] armors;
    public Image[] letters;
    public Image[] orbs;
    public Image[] boosts;

    static public int theLifes = 2;
    static public int theArmors = 0;
    static public int theLetters = 0;
    static public int theOrbs = 0;
    static public int theBoosts = 3; // refri to add, energy etc;

    private snafu snafuSystem;
    private orb orbSystem;
    private boost boostSystem;


    void Awake()
    {
        S = this;
        /* Wanted in future
        foreach(Image life in lifes)
        {
            DICT_GUI_LIFES.Add(life.name, life);
        }
        foreach (Image armor in armors)
        {
            DICT_GUI_LIFES.Add(armor.name, armor);
        }
        */
        UpdateInventory();
    }
    static public void UpdateInventory()
    {
        S.UpdateLife();
        S.UpdateArmor();
        S.UpdateSnafu();
        S.UpdateOrbs();
        S.UpdateBoost();
    }
    void UpdateLife()
    {
        if (PlayerShip.LIFES != null)
            theLifes = PlayerShip.LIFES.Count;
        else
            theLifes = 0;

        string str = "Heart" + theLifes.ToString();
        // or string str = lifes[theLifes - 1].name;
        bool toEnable = true;
        if (theLifes == 0)
        {
            toEnable = false;
        }
        foreach (Image life in lifes)
        {
            life.enabled = toEnable;
            if(life.name == str)
            {
                toEnable = false;
            }
        }
        // Debug.Log("HUDSystems:UpdateLife - Life updating " + theLifes);
    }
    void UpdateArmor()
    {
        if (PlayerShip.ARMORS != null)
            theArmors = PlayerShip.ARMORS.Count;
        else
            theArmors = 0;

        string str = "Armor" + theArmors.ToString();
        // or string str = armors[theArmors - 1].name;
        bool toEnable = true;
        if(theArmors == 0)
        {
            toEnable = false;
        }
        foreach (Image armor in armors)
        {
            armor.enabled = toEnable;
            if (armor.name == str)
            {
                toEnable = false;
            }
        }
        // Debug.Log("HUDSystems:UpdateArmor - Armor updating " + theArmors);
    }
    void UpdateSnafu()
    {
        snafuSystem = PlayerShip.snafuSystem;
        bool toEnable = true;
        int i = 0;
        foreach (Image letter in letters)
        {
            switch (i)
            {
                case 0:
                    toEnable = snafuSystem.S;
                    break;
                case 1:
                    toEnable = snafuSystem.N;
                    break;
                case 2:
                    toEnable = snafuSystem.A;
                    break;
                case 3:
                    toEnable = snafuSystem.F;
                    break;
                case 4:
                    toEnable = snafuSystem.U;
                    break;
                default:
                    Debug.Log("HUDSystems:UpdateSnafu - letter has not been set in good way!");
                    break;
            }
            letter.gameObject.SetActive(toEnable);
            // letter.enabled = toEnable;
            i++;
        }
        // Debug.Log("HUDSystems:UpdateSnafu - Snafu updating " + theLetters);
    }
    void UpdateOrbs()
    {
        orbSystem = PlayerShip.orbSystem;
        bool toEnable = true;
        int i = 0;
        foreach (Image orb in orbs)
        {
            switch (i)
            {
                case 0:
                    toEnable = orbSystem.Red;
                    break;
                case 1:
                    toEnable = orbSystem.Orange;
                    break;
                case 2:
                    toEnable = orbSystem.Yellow;
                    break;
                case 3:
                    toEnable = orbSystem.Green;
                    break;
                case 4:
                    toEnable = orbSystem.Blue;
                    break;
                case 5:
                    toEnable = orbSystem.Purple;
                    break;
                case 6:
                    toEnable = orbSystem.White;
                    break;
                default:
                    Debug.Log("HUDSystems:UpdateOrbs - orb has not been set in good way!");
                    break;
            }
            orb.gameObject.SetActive(toEnable);
            i++;
        }
        // Debug.Log("HUDSystems:UpdateOrbs - Orbs updating " + theOrbs);
    }
    void UpdateBoost()
    {
        boostSystem = PlayerShip.GetPlayerBoosts();
        float barSize = 0f;
        int i = 0;
        foreach (Image boost in boosts)
        {
            switch (i)
            {
                case 0:
                    barSize = boostSystem.bulletTime;
                    break;
                case 1:
                    barSize = boostSystem.reflex;
                    break;
                case 2:
                    barSize = boostSystem.speedBoost;
                    break;
                case 3:
                    barSize = boostSystem.energy;
                    break;
                case 4:
                    barSize = boostSystem.freezeTime;
                    break;
                default:
                    Debug.Log("HUDSystems:UpdateBoost - boost has not been set in good way!");
                    break;
            }
            // Debug.Log("boost anchor + " + barSize);
            float Ay = boost.transform.GetComponent<RectTransform>().anchorMax.y;
            boost.transform.GetComponent<RectTransform>().anchorMax = new Vector2(barSize, Ay);
            i++;
        }
        // Debug.Log("HUDSystems:UpdateBoost - Boosts updating");
    }
}