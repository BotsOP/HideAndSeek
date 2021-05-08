using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher Instance;
    
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_InputField nickName;
    [SerializeField] private TMP_Text displayNickname;
    [SerializeField] private TMP_Text teamText;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject startGameButton;

    private ExitGames.Client.Photon.Hashtable playerCustomProperties = new ExitGames.Client.Photon.Hashtable();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        displayNickname.text = "Nickname: " + PhotonNetwork.NickName;
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined lobby");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        RoomOptions options = new RoomOptions();
        options.BroadcastPropsChangeToAll = true;
        
        PhotonNetwork.CreateRoom(roomNameInputField.text, options);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterCleint)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform transform in roomListContent)
        {
            Destroy(transform.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("player joined: " + newPlayer.NickName);
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void ChangeNickName()
    {
        if (!string.IsNullOrEmpty(nickName.text))
        {
            Debug.Log(nickName.text);
            PhotonNetwork.NickName = nickName.text;
            displayNickname.text = "Nickname: " + nickName.text;
        }
    }

    public void TeamPicker(int whichCharacter)
    {
        if (RoomManager.Instance != null)
        {
            playerCustomProperties["Team"] = whichCharacter;
            PhotonNetwork.SetPlayerCustomProperties(playerCustomProperties);
            
            RoomManager.Instance.selectedCharacter = whichCharacter;
            PlayerPrefs.SetInt("MyCharacter", whichCharacter);
        }
        if (whichCharacter == 0)
        {
            teamText.text = "Team hiders";
            teamText.color = Color.blue;
        }
        else if (whichCharacter == 1)
        {
            teamText.text = "Team seekers";
            teamText.color = Color.red;
        }
    }
}
