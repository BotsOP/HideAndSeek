using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheck : MonoBehaviour
{
    private FPSController playerController;
    private void Awake()
    {
        playerController = transform.parent.GetComponent<FPSController>();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
        {
            return;
        }
        playerController.isGrounded = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
        {
            return;
        }
        playerController.isGrounded = true;
    }
}
