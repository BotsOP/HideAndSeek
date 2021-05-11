using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSounds : MonoBehaviour
{
    public AudioClip[] footsteps;
    private AudioSource audioSource;
    private PhotonView pv;
    private CharacterController controller;
    private int flip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();
    }
    private void Step()
    {
        if(!pv.IsMine)
            return;
        
        flip++;
        if (flip % 2 == 0 & controller.isGrounded)
        {
            pv.RPC("RPC_Step", RpcTarget.All);
        }
    }

    private AudioClip RandomAudioClip()
    {
        return footsteps[Random.Range(0, footsteps.Length)];
    }

    [PunRPC]
    private void RPC_Step()
    {
        audioSource.PlayOneShot(RandomAudioClip());
    }
}
