using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillManagement
{
    private static int rotateHash = Animator.StringToHash("RotateShip");

    public static IEnumerator IERotateAroundCenter(Animator anim)
    {
        Quaternion q = PlayerShip.GetPlayerShip().transform.localRotation;
        anim.GetComponent<Animator>().SetTrigger(rotateHash);
        yield return new WaitForSeconds(5);
        PlayerShip.GetPlayerShip().transform.localRotation = q;
        anim.GetComponent<Animator>().SetTrigger(rotateHash);
    }
}
