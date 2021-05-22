using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public Transform topLadder;
    public Transform bottomLadder;
    public float ladderMovementSpeed;
    
    private CharacterController controller;
    private Vector3 ladderMovement;
    void Start()
    {
        ladderMovement = topLadder.position - bottomLadder.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetParentComponent<CharacterController>() && other.CompareTag("Player"))
        {
            other.transform.GetParentComponent<FPSController>().onLadder = true;
            other.transform.GetParentComponent<FPSController>().verticalVelocity = 0;
            controller = other.transform.GetParentComponent<CharacterController>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        if (Input.GetKey(KeyCode.W))
        {
            controller.Move(ladderMovement.normalized * Time.deltaTime * ladderMovementSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            controller.Move(-ladderMovement.normalized * Time.deltaTime * ladderMovementSpeed);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetParentComponent<CharacterController>() && other.CompareTag("Player"))
        {
            other.transform.GetParentComponent<FPSController>().onLadder = false;
        }
    }
}
