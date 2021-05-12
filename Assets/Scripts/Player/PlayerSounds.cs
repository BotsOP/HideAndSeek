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
    
    private FPSController player;
    private CharacterController controller;
    private AudioSource audioSource;
    private PhotonView pv;

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
        if (controller.isGrounded && hasGrounded)
        {
            pv.RPC("RPC_FallSound", RpcTarget.All);
            hasGrounded = false;
        }
        if (!controller.isGrounded)
        {
            hasGrounded = true;
        }
    }
    
    [PunRPC]
    private void RPC_FallSound()
    {
        audioSource.PlayOneShot(fallSound);
    }

    //Could use animation event to check if the punch hit something
    private void PunchSound()
    {
        if(!pv.IsMine)
            return;
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 2f)) 
        {
            if (hit.transform.gameObject.GetComponent<IDamagable>() != null)
            {
                pv.RPC("RPC_Punch", RpcTarget.Others, true);
                hit.transform.gameObject.GetComponent<IDamagable>().TakeDamage(30);
                return;
            }
            pv.RPC("RPC_Punch", RpcTarget.Others, false);
            return;
        }
        pv.RPC("RPC_Punch", RpcTarget.Others, false);
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
