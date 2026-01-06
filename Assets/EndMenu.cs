using System.Collections;
using DG.Tweening;
using Exploration;
using Menu;
using UnityEngine;

public class EndMenu : MonoBehaviour
{
    [SerializeField] private GameObject endMenu;
    [SerializeField] private Vector3 positionForEndGame;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private float timeOnEndGame;
    [SerializeField] private CanvasGroup cvg;
    [SerializeField] private float durationFade = 0.5f;
    void Start()
    {
        endMenu.SetActive(false);
    }
    

    public IEnumerator GoToEndMenu()
    {
        bool fadeIsFinished = false;
        cvg.DOKill();
        cvg.alpha = 1f;
        cvg.DOFade(0f, durationFade).SetEase(Ease.OutBounce).OnComplete(() =>fadeIsFinished = true);
        yield return new WaitUntil(() => fadeIsFinished);
        PlayerManager.INSTANCE.TeleportPlayer(positionForEndGame);
        endMenu.SetActive(true);
        PlayerManager.INSTANCE.SetCanMove(false);
        GameManager.INSTANCE.SetState(GameManager.GameState.End);
        yield return new WaitForSeconds(timeOnEndGame);
        mainMenu.ReturnToMainMenu();
    }
    
}
