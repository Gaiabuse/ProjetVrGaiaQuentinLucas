using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NoteScript : MonoBehaviour
{
    private bool _canHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_canHit)
            {
                // ajouter un systeme de perfect 
                
            }
            else
            {
                // faire monter anxieté
            }
            Destroy(gameObject);
        }
    }
}
