using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoubleDoor : MonoBehaviour, IInteractable
{
    public Animator otherAnimator;
    public DoubleDoor otherDoor;
    private Animator animator;
    private AudioSource audio;
    private PhotonView pv;
    private bool isNotMoving;
    public bool open;

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
        otherDoor.open = open;
        
        otherAnimator.SetBool("DoorOpen", open);
        animator.SetBool("DoorOpen", open);
    }

    [PunRPC]
    private void RPC_PlaySound()
    {
        audio.Play();
    }
}
