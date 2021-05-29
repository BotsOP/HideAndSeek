using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Observer : MonoBehaviour
{
    public List<GameObject> alivePlayers = new List<GameObject>();
    private int playerSpectateIndex;
    void Start()
    {
        Debug.Log("observer");
        if (FindObjectsOfType<FPSController>() == null)
        {
            Debug.LogError("Couldnt find any players");
            return;
        }
            
        
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
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GoNextPlayer();
        }
        if (Input.GetMouseButtonDown(1))
        {
            GoPreviousPlayer();
        }
    }

    public void GoNextPlayer()
    {
        playerSpectateIndex++;
        if (playerSpectateIndex == alivePlayers.Count)
            playerSpectateIndex = 0;
            
        transform.SetParent(alivePlayers[playerSpectateIndex].transform.GetChild(0));
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void GoPreviousPlayer()
    {
        playerSpectateIndex--;
        if (playerSpectateIndex == -1)
            playerSpectateIndex = alivePlayers.Count - 1;
            
        transform.SetParent(alivePlayers[playerSpectateIndex].transform.GetChild(0));
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
