using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSounds : MonoBehaviour
{
    public AudioClip[] footsteps;
    public AudioClip hitPunch;
    public AudioClip missedPunch;
    public AudioClip fallSound;
    private int flip;
    private bool hasGrounded;
    private bool isRunning;
    
    private CharacterController controller;
    private AudioSource audioSource;
    private PhotonView pv;
    private FPSController player;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();
        player = GetComponent<FPSController>();
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

    private void Update()
    {
        if (!pv.IsMine)
            return;
        
        if (player.isGrounded && hasGrounded)
        {
            pv.RPC("RPC_FallSound", RpcTarget.All);
            hasGrounded = false;
        }
        if (!player.isGrounded && !isRunning)
        {
            Debug.Log(player.isGrounded);
            StartCoroutine("HasGrounded");
        }
    }

    private IEnumerator HasGrounded()
    {
        isRunning = true;
        yield return new WaitForSeconds(0.4f);
        if (!player.isGrounded)
            hasGrounded = true;
        isRunning = false;
    }
    
    [PunRPC]
    private void RPC_FallSound()
    {
        audioSource.PlayOneShot(RandomAudioClip());
    }

    public void SoundPunch(bool didHit)
    {
        pv.RPC("RPC_Punch", RpcTarget.All, didHit);
    }
    
    [PunRPC]
    private void RPC_Punch(bool didHit)
    {
        if (didHit)
        {
            audioSource.PlayOneShot(hitPunch);
            return;
        }
        audioSource.PlayOneShot(missedPunch);
    }

    [PunRPC]
    private void RPC_Step()
    {
        audioSource.PlayOneShot(RandomAudioClip());
    }
}
