using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public Material changeMat;
    public MeshRenderer[] wallsToHide;
    public Material[] previousMats;

    private void Start()
    {
        previousMats = new Material[wallsToHide.Length];
    }
}
