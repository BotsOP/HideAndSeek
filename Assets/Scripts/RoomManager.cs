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
    public int startTime;
    private bool inGame;
    public GameObject PlayerManager;
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
            selectedCharacter = 0;
            PlayerPrefs.SetInt("MyCharacter", selectedCharacter);
        }
        else
        {
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
        if (scene.buildIndex == 1)
        {
            SetTime();
            StartCoroutine("DelayLoadScene");
            inGame = true;
        }
    }

    private IEnumerator DelayLoadScene()
    {
        //Delaying the player instantiation a little bit to stop weird desync issues
        yield return new WaitForSeconds(0.2f);
        PlayerManager = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    }

    public void SetTime()
    {
        time = (int)Time.time;
        startTime = time;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            time = (int)Time.time;
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && PhotonNetwork.IsMasterClient && inGame)
        {
            stream.SendNext(time);
            stream.SendNext(startTime);
        }
        else if (stream.IsReading && !PhotonNetwork.IsMasterClient && inGame)
        {
            time = (int)stream.ReceiveNext();
            startTime = (int)stream.ReceiveNext();
        }
    }
}
