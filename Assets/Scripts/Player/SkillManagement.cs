using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillManagement
{
    private static int rotateHash = Animator.StringToHash("Rotate");

    public static void RotateAroundCenter()
    {
        IERotateAroundCenter(PlayerShip.GetAnimator());
    }

    public static IEnumerator IERotateAroundCenter(Animator anim)
    {
        anim.GetComponent<Animator>().SetFloat(rotateHash, 1f);
        Debug.Log("wtf" + anim.GetCurrentAnimatorStateInfo(0).ToString());
        yield return new WaitForSeconds(4);
        anim.GetComponent<Animator>().SetFloat(rotateHash, 0f);
        PlayerShip.canI = true;
    }
}
