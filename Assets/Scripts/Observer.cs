using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Observer : MonoBehaviour
{
    private List<GameObject> alivePlayers = new List<GameObject>();
    private int playerSpectateIndex;
    void Start()
    {
        Debug.Log("observer");
        foreach (FPSController player in FindObjectsOfType<FPSController>())
        {
            if(!player.isSeeker)
                alivePlayers.Add(player.gameObject);
        }
        if (alivePlayers == null)
        {
            Debug.Log("last player died");
            alivePlayers.Add(FindObjectOfType<FPSController>().gameObject);
        }
        
        transform.SetParent(alivePlayers[playerSpectateIndex].transform.GetChild(0));
        transform.localRotation = Quaternion.Euler(90, 0, 0);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerSpectateIndex++;
            if (playerSpectateIndex == alivePlayers.Count)
                playerSpectateIndex = 0;
            
            transform.SetParent(alivePlayers[playerSpectateIndex].transform.GetChild(0));
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerSpectateIndex--;
            if (playerSpectateIndex == -1)
                playerSpectateIndex = alivePlayers.Count - 1;
            
            transform.SetParent(alivePlayers[playerSpectateIndex].transform.GetChild(0));
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
