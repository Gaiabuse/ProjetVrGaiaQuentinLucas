using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NoteScript : MonoBehaviour
{
    public bool _canHit = false;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speedx = 0f;
    [SerializeField] private float speedy = 0f;
    [SerializeField] private float speedz = 0f;
    
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CanHitWall"))
        {
            _canHit = true;
        }
        if (other.CompareTag("Player"))
        {
            if (!_canHit)
            {
                FightManager.INSTANCE.AddAnxiety();
                Debug.Log("bad");
            }
            else
            {
                Debug.Log("good");
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CanHitWall"))
        {
            _canHit = false;
        }
    }

    private void Awake()
    {
        rb.linearVelocity = new Vector3(speedx, speedy, speedz);
    }
}
