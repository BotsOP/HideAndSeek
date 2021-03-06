using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView pv;
    public GameObject myCharacter;
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            pv.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, RoomManager.Instance.selectedCharacter);
            myCharacter.layer = 6;
        }
    }

    [PunRPC]
    void RPC_AddCharacter(int whichCharacter)
    {
        if(whichCharacter == 0)
        {
            transform.GetChild(1).transform.GetChild(5).gameObject.SetActive(false);
            myCharacter = transform.GetChild(1).transform.GetChild(8).gameObject;
            myCharacter.SetActive(true);
        }
        else
        {
            transform.GetChild(1).transform.GetChild(8).gameObject.SetActive(false);
            myCharacter = transform.GetChild(1).transform.GetChild(5).gameObject;
            FindObjectOfType<FPSController>().isSeeker = true;
            myCharacter.SetActive(true);
        }
    }
}
