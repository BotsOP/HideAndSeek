using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTeleport : MonoBehaviour
{
    public Transform targetTeleport;
    private FPSController player;
    private bool shouldTeleport;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(player == null)
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPSController>();
            
            player.transform.position = targetTeleport.position;
            player.TeleportSyncer();
            shouldTeleport = true;
        }
    }

    private void Update()
    {
        if (shouldTeleport)
        {
            if (Vector3.Distance(player.transform.position, targetTeleport.position) > 1f)
            {
                player.transform.position = targetTeleport.position;
            }
            else
            {
                shouldTeleport = false;
            }
        }
    }
}
