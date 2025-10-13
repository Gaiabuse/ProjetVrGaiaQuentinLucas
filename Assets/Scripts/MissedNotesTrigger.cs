using System;
using UnityEngine;

public class MissedNotesTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            FightManager.INSTANCE.AddAnxiety();
            Destroy(other.gameObject);
        }
    }
}
