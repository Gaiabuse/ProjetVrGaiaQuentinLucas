using System;
using System.Collections;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    [SerializeField] private float damages = 2f;
    [SerializeField] private float hitSpeed = 1f;
    [SerializeField] private int maxHit = 5;
    private bool canHit = true;
    private Coroutine _damagesCoroutine;

    private void Awake()
    {
        _damagesCoroutine = StartCoroutine(WaitForDamages());
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FightManager.INSTANCE.AddAnxiety(- damages);
            Debug.Log("good");
            Destroy(gameObject);
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
        StopCoroutine(_damagesCoroutine);
    }
}
