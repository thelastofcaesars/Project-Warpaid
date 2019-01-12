using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManagement : MonoBehaviour
{
    private static SkillManagement _S;

    public static SkillManagement S
    {
        get
        {
            if (S == null)
            {
                Debug.LogWarning("SkillManagement:S - attempt to get value before it has been set!");
            }
            return S;
        }
        set
        {
            if (S != null)
            {
                Debug.LogWarning("SkillManagement:S - attempt to set value after it has been set!");
            }
            else
            {
                S = _S;
            }
        }
    }

    private void Awake()
    {
        _S = this;
    }

    public static void RotateAroundCenter()
    {
        S.StartCoroutine(S.IERotateAroundCenter(PlayerShip.GetPlayerShip()));
    }

    public IEnumerator IERotateAroundCenter(PlayerShip playerShip)
    {
        playerShip.GetComponent<Animator>().SetBool("isRotating", true);
        yield return new WaitForSeconds(4);
        playerShip.GetComponent<Animator>().SetBool("isRotating", false);
    }
}
