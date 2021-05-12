using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int matchTime;
    public TMP_Text timeText;
    private int currentMatchTime;
    private PhotonView pv;
    public bool nextSeeker;
    public bool nextSeekerFound;
    public int playersDead;
    public int playersAlive;

    private RoomManager roomManager;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        pv = GetComponent<PhotonView>();
        PhotonNetwork.AutomaticallySyncScene = true;
        playersAlive = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void Update()
    {
        SetTime();
    }

    public void UpdatePlayerCount(bool playerDied)
    {
        if (playerDied)
            playersDead++;
        else
            playersDead--;
        
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount - playersDead;
        playersAlive = playerCount;
        if (playerCount == 1 && PhotonNetwork.IsMasterClient)
        {
            StartCoroutine("StartNextRound");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount(FindObjectOfType<FPSController>());
    }

    private IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(0.1f);
        NextRound();
    }

    private void SetTime()
    {
        int time = roomManager.time - roomManager.startTime;
        int minutes = (matchTime - time) / 60;
        int seconds = (matchTime - time) % 60;
        timeText.text = minutes + ":" + seconds;
        if (minutes == 0 && seconds == 0)
        {
            NextRound();
        }
    }

    public void NextRound()
    {
        pv.RPC("RPC_Reset", RpcTarget.All);

        roomManager.SetTime();
    }

    [PunRPC]
    private void RPC_Reset()
    {
        SceneManager.LoadScene(1);
        
        if (nextSeekerFound)
        {
            if (nextSeeker)
            {
                roomManager.selectedCharacter = 1;
            }
            else
            {
                roomManager.selectedCharacter = 0;
            }
        }
        
        Camera.main.transform.SetParent(null);
    }


}
