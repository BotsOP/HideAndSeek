using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffHidingSpot : MonoBehaviour
{
    public Material changeMat;
    public MeshRenderer[] wallsToHide;
    public Material previousMats;
    public bool entranceOn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < wallsToHide.Length; i++)
            {
                if(entranceOn)
                    wallsToHide[i].material = changeMat;
                else
                    wallsToHide[i].material = previousMats;
            }
        }
    }
}
