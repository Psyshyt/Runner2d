using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Game
{
    public class GameOverManager : MonoBehaviour
    {
        public static GameOverManager Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private CanvasGroup gameOverCanvasGroup;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text statsText;

        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        [Header("Scenes")]
        [SerializeField] private string menuSceneName = "MainMenu";

        private bool isGameOver;

        public bool IsGameOver => isGameOver;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            HideInstant();

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }

            if (menuButton != null)
            {
                menuButton.onClick.AddListener(GoToMenu);
            }
        }

        public void ShowGameOver()
        {
            if (isGameOver)
                return;

            isGameOver = true;

            Time.timeScale = 0f;

            if (titleText != null)
            {
                titleText.text = "Игра окончена";
            }

            UpdateStatsText();

            if (gameOverCanvasGroup != null)
            {
                gameOverCanvasGroup.alpha = 1f;
                gameOverCanvasGroup.interactable = true;
                gameOverCanvasGroup.blocksRaycasts = true;
            }

            Debug.Log("Game Over");
        }

        private void UpdateStatsText()
        {
            if (statsText == null)
                return;

            if (LevelProgressManager.Instance == null)
            {
                statsText.text = "";
                return;
            }

            int level = LevelProgressManager.Instance.CurrentLevel;
            int score = LevelProgressManager.Instance.CurrentScore;
            int targetScore = LevelProgressManager.Instance.CurrentTargetScore;

            statsText.text =
                "Достигнут уровень: " + level + "\n" +
                "Собрано сюрикенов: " + score + " / " + targetScore;
        }

        private void RestartGame()
        {
            Time.timeScale = 1f;

            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }

        private void GoToMenu()
        {
            Time.timeScale = 1f;

            if (!string.IsNullOrEmpty(menuSceneName))
            {
                SceneManager.LoadScene(menuSceneName);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        private void HideInstant()
        {
            Time.timeScale = 1f;

            if (gameOverCanvasGroup == null)
                return;

            gameOverCanvasGroup.alpha = 0f;
            gameOverCanvasGroup.interactable = false;
            gameOverCanvasGroup.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(RestartGame);
            }

            if (menuButton != null)
            {
                menuButton.onClick.RemoveListener(GoToMenu);
            }
        }
    }
}