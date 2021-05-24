using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Door : MonoBehaviour, IInteractable
{
    private Animator animator;
    private AudioSource audio;
    private PhotonView pv;
    private bool isNotMoving;
    private bool open;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();
    }
    public void Interact()
    {
        pv.RPC("RPC_PlaySound", RpcTarget.All);
        open = !open;
        animator.SetBool("DoorOpen", open);
        //pv.RPC("RPC_PlaySound", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_PlaySound()
    {
        audio.Play();
    }
}
