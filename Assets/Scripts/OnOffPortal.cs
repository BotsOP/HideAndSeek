using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffPortal : MonoBehaviour
{
    public bool TurnOffPortal;
    public Portal[] portals;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            portals[0].portalOff = TurnOffPortal;
            portals[1].portalOff = TurnOffPortal;
        }
    }
}
