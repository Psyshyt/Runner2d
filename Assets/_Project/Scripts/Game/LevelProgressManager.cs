using TMPro;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class LevelProgressManager : MonoBehaviour
    {
        public static LevelProgressManager Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text levelText;

        [Header("Level Settings")]
        [SerializeField] private int scoreToNextLevel = 10;
        [SerializeField] private int maxLevel = 5;

        [Header("Speed Settings")]
        [SerializeField] private float speedIncreasePerLevel = 1f;

        [Header("Links")]
        [SerializeField] private BackgroundLooper backgroundLooper;

        private int currentLevel = 1;
        private int currentScore;

        public float CurrentGameSpeed
        {
            get
            {
                if (backgroundLooper == null)
                    return 5f;

                return backgroundLooper.CurrentSpeed;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            currentLevel = 1;
            currentScore = 0;

            if (backgroundLooper != null)
            {
                backgroundLooper.SetLevelBackground(currentLevel - 1);
            }

            UpdateUI();
        }

        public void AddScore(int amount)
        {
            if (amount <= 0)
                return;

            currentScore += amount;

            if (currentScore >= scoreToNextLevel)
            {
                GoToNextLevel();
                scoreToNextLevel += 20;
            }

            UpdateUI();
        }

        private void GoToNextLevel()
        {
            currentScore = 0;

            if (currentLevel >= maxLevel)
            {
                Debug.Log("Максимальный уровень уже достигнут");
                return;
            }

            currentLevel++;

            Debug.Log("Переход на уровень: " + currentLevel);

            if (backgroundLooper != null)
            {
                backgroundLooper.SetLevelBackground(currentLevel - 1);
                backgroundLooper.IncreaseSpeed(speedIncreasePerLevel);
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (scoreText != null)
            {
                scoreText.text = currentScore + " / " + scoreToNextLevel;
            }

            if (levelText != null)
            {
                levelText.text = "Уровень " + currentLevel;
            }
        }
    }
}