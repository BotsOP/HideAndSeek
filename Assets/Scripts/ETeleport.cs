using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ETeleport : MonoBehaviour, IInteractable
{
    public Transform targetTeleport;
    private FPSController player;

    void Start()
    {
        
    }

    public void Interact()
    {
        player = FindObjectOfType<CharacterController>().gameObject.GetComponent<FPSController>();
        player.transform.position = targetTeleport.position;
        player.TeleportSyncer();
    }
}
