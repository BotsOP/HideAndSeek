using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView pv;

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
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.up, Quaternion.identity);
    }
}
