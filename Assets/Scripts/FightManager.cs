using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightManager : MonoBehaviour
{
    public static FightManager INSTANCE;
    public float damages = 5f;
    
    [SerializeField] private float maxAnxiety = 100f;

    private float _anxiety = 0f;
    
    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddAnxiety()
    {
        _anxiety += damages;
        Debug.Log(("anxiety +"));
        CheckLoose();
    }

    void CheckLoose()
    {
        if (_anxiety >= maxAnxiety)
        {
            Debug.Log("You loose"); // changer la plupart des trucs ici c'est uniquement pour le rendu du 13 a 16h c'est pas definitif
            EndFight();
        }
    }

    void EndFight()
    {
        _anxiety = 0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // changer ça aussi a terme
    }
}
