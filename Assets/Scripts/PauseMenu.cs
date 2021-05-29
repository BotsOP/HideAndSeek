using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private GameObject pauseMenu;
    private bool pauseMenuActive;
    private FPSController player;
    private Slider sensitivitySlider;
    private PhotonView pv;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        
        if (!pv.IsMine)
            Destroy(this);
        
        player = GetComponent<FPSController>();
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        sensitivitySlider = pauseMenu.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Slider>();
        pauseMenu.transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenuActive)
            {
                pauseMenu.transform.GetChild(0).gameObject.SetActive(true);
                pauseMenuActive = true;
                player.allowedToMove = false;
                Cursor.lockState = CursorLockMode.None;
                player.mouseSensitivity = sensitivitySlider.value;
            }
            else
            {
                pauseMenu.transform.GetChild(0).gameObject.SetActive(false);
                pauseMenuActive = false;
                player.allowedToMove = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // if (pauseMenuActive)
        // {
        //     player.mouseSensitivity = sensitivitySlider.value;
        // }
    }
}
