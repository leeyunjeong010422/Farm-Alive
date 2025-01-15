using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void PlayAuido()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}