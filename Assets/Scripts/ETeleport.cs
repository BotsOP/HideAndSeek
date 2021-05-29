using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ETeleport : MonoBehaviour, IInteractable
{
    public Transform targetTeleport;
    private FPSController player;

    public void Interact()
    {
        if(!player)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPSController>();
        
        player.transform.position = targetTeleport.position;
        player.TeleportSyncer();
    }
    
    
}
