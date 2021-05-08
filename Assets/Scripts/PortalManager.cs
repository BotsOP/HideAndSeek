using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public GameObject[] portals;
    public GameObject[] turnPortalOff;

    private void Start()
    {
        //SetUpPortals();
        StartCoroutine("DelayPortals");
        StartCoroutine("TurnOffPortals");
    }

    IEnumerator DelayPortals()
    {
        yield return new WaitForSeconds(0.1f);
        SetUpPortals();
    }

    public void SetUpPortals()
    {
        MainCamera mainCamera = FindObjectOfType<MainCamera>();
        mainCamera.portals = new Portal[portals.Length];
        
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].SetActive(true);
            mainCamera.portals[i] = portals[i].GetComponent<Portal>();
        }
    }
    
    IEnumerator TurnOffPortals()
    {
        yield return new WaitForSeconds(0.11f);
        foreach (GameObject portal in turnPortalOff)
        {
            portal.SetActive(false);
        }
    }
}
