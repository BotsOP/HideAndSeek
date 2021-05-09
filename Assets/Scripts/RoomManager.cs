using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    public GameObject[] allCharacters;
    public int selectedCharacter;
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
            Debug.Log("test2");
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
}
