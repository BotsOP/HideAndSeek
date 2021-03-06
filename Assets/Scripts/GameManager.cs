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
    public Door seekerDoor;
    public TMP_Text timeText;
    public TMP_Text fpsText;
    private int currentMatchTime;
    private PhotonView pv;
    public bool nextSeeker;
    public bool nextSeekerFound;
    public int playersDead;
    public int playersAlive;
    private bool openedSeekerDoor;
    private Transform playerTransform;

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
        int current = (int)(1f / Time.unscaledDeltaTime);
        fpsText.text = current.ToString();
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
        Debug.Log("player left room");
        UpdatePlayerCount(FindObjectOfType<FPSController>());
    }

    private IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(0.1f);
        NextRound();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(2);
    }

    private void SetTime()
    {
        int time = roomManager.time - roomManager.startTime;
        int minutes = (matchTime - time) / 60;
        int seconds = (matchTime - time) % 60;
        timeText.text = minutes + ":" + seconds;
        if (minutes == 12 && seconds == 0 && !openedSeekerDoor && PhotonNetwork.IsMasterClient)
        {
            OpenSeekerDoor();
        }
        if (minutes == 0 && seconds == 0 && PhotonNetwork.IsMasterClient)
        {
            StartCoroutine("StartNextRound");
        }
    }

    private void OpenSeekerDoor()
    {
        seekerDoor.Interact();
        openedSeekerDoor = true;
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
