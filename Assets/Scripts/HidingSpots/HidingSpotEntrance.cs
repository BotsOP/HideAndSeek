using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpotEntrance : MonoBehaviour
{
    private BoxCollider bc;
    public HidingSpot hidingSpot;
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        bc.isTrigger = true;
        hidingSpot = transform.parent.gameObject.GetComponent<HidingSpot>();
        if (hidingSpot == null)
        {
            Debug.LogError("Hiding spot script is not the parent of this gameobject");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hidingSpot.previousMats[0] == null)
            {
                for (int i = 0; i < hidingSpot.wallsToHide.Length; i++)
                {
                    hidingSpot.previousMats = hidingSpot.wallsToHide[i].materials;
                }
            }
            
            for (int i = 0; i < hidingSpot.wallsToHide.Length; i++)
            {
                hidingSpot.wallsToHide[i].material = hidingSpot.changeMat;
            }
        }
    }
}
