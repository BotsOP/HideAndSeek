using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static RoomManager Instance;
    public int selectedCharacter;
    public int time;
    private bool inGame;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("MyCharacter"))
        {
            Debug.Log("Already has mycharacter pref");
            selectedCharacter = 0;
            PlayerPrefs.SetInt("MyCharacter", selectedCharacter);
        }
        else
        {
            Debug.Log("doesnt have mycharacter pref");
            selectedCharacter = 0;
            PlayerPrefs.SetInt("MyCharacter", selectedCharacter);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("test");
        if (scene.buildIndex == 1)
        {
            time = (int)Time.time;
            inGame = true;
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            time = (int)Time.time;
        }
        Debug.Log(time);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("Streaming");
        if (stream.IsWriting && PhotonNetwork.IsMasterClient && inGame)
        {
            stream.SendNext(time);
        }
        else if (stream.IsReading && !PhotonNetwork.IsMasterClient && inGame)
        {
            time = (int)stream.ReceiveNext();
        }
    }
}
