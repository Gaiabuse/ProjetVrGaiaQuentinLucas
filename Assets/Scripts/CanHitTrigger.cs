using System;
using UnityEngine;

public class CanHitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            other.GetComponent<NoteScript>()._canHit = true; // changer ça c'est pas propre mais c'est pour test
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            other.GetComponent<NoteScript>()._canHit = false; // pareil qu'au dessus
        }
    }
}
