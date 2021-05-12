using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EHidingSpot : MonoBehaviour, IInteractable
{
    public Transform targetTransform;
    public float animSpeed = 3f;
    public float timeRemainOpen = 5f;
    private Vector3 originalPosition;
    private bool goToTarget;
    private bool goToOriginal;

    private AudioSource audio;
    private PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        audio = GetComponent<AudioSource>();
        targetTransform.gameObject.SetActive(false);
        originalPosition = transform.position;
    }
    public void Interact()
    {
        if (transform.position == originalPosition)
        {
            pv.RPC("RPC_PlaySound", RpcTarget.All);
            goToTarget = true;
        }
    }

    private void Update()
    {
        if (goToTarget)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * animSpeed);
            if (Vector3.Distance(transform.position, targetTransform.position) < 0.01f)
            {
                transform.position = targetTransform.position;
                StartCoroutine("GoBackToOriginal");
                goToTarget = false;
            }
        }
        
        if (goToOriginal)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * animSpeed);
            if (Vector3.Distance(transform.position, originalPosition) < 0.01f)
            {
                transform.position = originalPosition;
                goToOriginal = false;
            }
        }
    }

    private IEnumerator GoBackToOriginal()
    {
        yield return new WaitForSeconds(timeRemainOpen);
        pv.RPC("RPC_PlaySound", RpcTarget.All);
        goToOriginal = true;
    }

    [PunRPC]
    private void RPC_PlaySound()
    {
        audio.Play();
    }
}
