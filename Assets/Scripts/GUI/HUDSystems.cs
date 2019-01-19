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

    // Wanted in future
    static Dictionary<string, Image> DICT_GUI_ARMORS;
    static Dictionary<string, Image> DICT_GUI_LIFES;
    static Dictionary<string, Image> DICT_GUI_LETTERS;
    //

    public Image[] lifes;
    public Image[] armors;
    public Image[] letters;
    static public int theLifes = 2;
    static public int theArmors = 0;
    static public int theLetters = 0;
    private snafu snafuSystem;

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
        //UpdateInventory();
        UpdateLife();
        UpdateArmor();
        UpdateSnafu();
    }
    static public void UpdateInventory()
    {
        S.UpdateLife();
        S.UpdateArmor();
        S.UpdateSnafu();
    }
    void UpdateLife()
    {
        if (PlayerShip.LIFES != null)
            theLifes = PlayerShip.LIFES.Count;
        else
            theLifes = 2;

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
        // Debug.Log("HUDSystems:Update Life - Life updating " + theLifes);
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
        // Debug.Log("HUDSystems:Update Armor - Armor updating " + theArmors);
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
            }
            letter.gameObject.SetActive(toEnable);
            // letter.enabled = toEnable;
            i++;
        }
        // Debug.Log("HUDSystems:Update Snafu - Snafu updating " + theLetters);
    }
}