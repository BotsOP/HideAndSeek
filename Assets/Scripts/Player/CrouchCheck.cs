using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchCheck : MonoBehaviour
{
    public bool shouldCrouch;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Crouchable"))
        {
            shouldCrouch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Crouchable"))
        {
            shouldCrouch = false;
        }
    }
}
