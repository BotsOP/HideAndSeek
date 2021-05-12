using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    private PhotonView pv;
    public GameObject controller;
    
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
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), GetRandomSpawn(), Quaternion.identity, 0, new object[] { pv.ViewID });
    }

    private Vector3 GetRandomSpawn()
    {
        return new Vector3(Random.Range(-5f, 5f), 1, Random.Range(-5f, 5f));
    }

    public void Die(bool respawn)
    {
        Camera.main.transform.SetParent(null);
        PhotonNetwork.Destroy(controller);
        StartCoroutine("Respawn", respawn);
    }

    private IEnumerator Respawn(bool respawn)
    {
        yield return new WaitForSeconds(1f);
        if(respawn)
            CreateController();
    }
}
