using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    private AudioSource aud;
    public AudioClip hoverFX;
    public AudioClip clickFX;

    public void Start()
    {
        aud = GetComponent<AudioSource>();
    }
    public void HoverSound()
    {
        aud.PlayOneShot(hoverFX);
    }
    public void ClickSound()
    {
        aud.PlayOneShot(clickFX);
    }
}
