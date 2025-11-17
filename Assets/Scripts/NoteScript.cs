using System;
using System.Collections;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    [SerializeField] private float damages = 2f;
    [SerializeField] private float hitSpeed = 1f;
    [SerializeField] private int maxHit = 5;
    private Coroutine _damagesCoroutine;

    private void Awake()
    {
        _damagesCoroutine = StartCoroutine(WaitForDamages());
    }

    private void OnTriggerEnter(Collider other)
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
        for (int i = 0; i < maxHit; i++)
        {
            yield return new WaitForSeconds(hitSpeed);
            FightManager.INSTANCE.AddAnxiety(damages);
            Debug.Log("bad");
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        StopCoroutine(_damagesCoroutine);
    }
}
