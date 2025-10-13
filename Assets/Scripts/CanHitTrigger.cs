using System;
using UnityEngine;

public class CanHitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            // met le canhit a true
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            // met le canhit a false
        }
    }
}
