using System;
using System.Collections;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    [SerializeField] private float damages = 2f;
    [SerializeField] private float hitSpeed = 1f;
    [SerializeField] private int maxHit = 5;
    private bool canHit = true;
    private bool _inTrigger = false;

    private void Awake()
    {
        StartCoroutine(WaitForDamages());
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _inTrigger = true;
            StartCoroutine(PlayerInTrigger());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _inTrigger = false;
    }

    IEnumerator PlayerInTrigger()
    {
        while (_inTrigger)
        {
            if (true) // mettre les input de la manette
            {
                FightManager.INSTANCE.AddAnxiety(- damages);
                Destroy(gameObject);
            }

            yield return null;
        }
    }

    IEnumerator WaitForDamages()
    {
        if (canHit)
        {
            for (int i = 0; i < maxHit; i++)
            {
                yield return new WaitForSeconds(hitSpeed);
                FightManager.INSTANCE.AddAnxiety(damages);
            }
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
