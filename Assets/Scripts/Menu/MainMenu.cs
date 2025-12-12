using Exploration;
using Scenario;
using UnityEngine;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuGameObject;
        [SerializeField]private Vector3 positionMainMenu;
        [SerializeField] private Vector3 positionForStartGame;
        void Start()
        {
            mainMenuGameObject.SetActive(true);
            PlayerManager.INSTANCE.TeleportPlayer(positionMainMenu);
        }

        private void OnEnable()
        {
            GameManager.ReturnToMainMenu += ReturnToMainMenu;
        }

        private void OnDisable()
        {
            GameManager.ReturnToMainMenu -= ReturnToMainMenu;
        }

        public void Play()
        {
            GameManager.INSTANCE.SetState(GameManager.GameState.Playing);
            mainMenuGameObject.SetActive(false);
            PlayerManager.INSTANCE.SetCanMove(true);
            GameManager.INSTANCE.DayLoop = StartCoroutine(DayManager.INSTANCE.DayLoop());
            PlayerManager.INSTANCE.TeleportPlayer(positionForStartGame);
            Time.timeScale = 1;
        }

        private void ReturnToMainMenu()
        {
            mainMenuGameObject.SetActive(true);
            PlayerManager.INSTANCE.TeleportPlayer(positionMainMenu);
        }
    }
}
