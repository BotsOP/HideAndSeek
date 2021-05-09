using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView pv;
    GameObject controller;
    
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (pv.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Debug.Log("instantied player controller");
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.up, Quaternion.identity, 0, new object[] { pv.ViewID });
    }

    public void Die()
    {
        Camera.main.transform.SetParent(null);
        PhotonNetwork.Destroy(controller);
        StartCoroutine("Respawn");
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);
        CreateController();
    }
}
