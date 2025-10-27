using System;
using UnityEngine;
public class NoteScript : MonoBehaviour
{
    private bool isHit = false;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            isHit = true;
            Debug.Log("good");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (!isHit)
        {
            Debug.Log("bad");
            FightManager.INSTANCE.AddAnxiety();
        }
    }
}
