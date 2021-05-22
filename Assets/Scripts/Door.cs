using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private Animator animator;
    private bool isNotMoving;
    private bool open;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Interact()
    {
        if (!isNotMoving)
        {
            open = !open;
            animator.SetBool("DoorOpen", open);
            //pv.RPC("RPC_PlaySound", RpcTarget.All);
        }
    }
}
