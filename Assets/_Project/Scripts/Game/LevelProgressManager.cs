using System.Collections;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class LevelProgressManager : MonoBehaviour
    {
        public static LevelProgressManager Instance { get; private set; }

        [System.Serializable]
        public class LevelData
        {
            [Header("Progress")]
            public int screwsToComplete = 20;

            [Header("Speed")]
            public float speedIncreaseOnLevelStart = 0f;
        }

        [Header("UI")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text levelText;

        [Header("Levels")]
        [SerializeField] private LevelData[] levels;

        [Header("Links")]
        [SerializeField] private BackgroundLooper backgroundLooper;
        [SerializeField] private LevelTransitionUI levelTransitionUI;

        [Header("Hit Knockback")]
        [SerializeField] private float worldKnockbackDuration = 1f;
        [SerializeField] private float worldKnockbackSpeedMultiplier = 1.5f;

        private int currentLevelIndex;
        private int currentScore;

        private bool isTransitioning;
        private bool isWorldKnockback;
        private bool isGameCompleted;

        private Coroutine knockbackCoroutine;

        public int CurrentLevel => currentLevelIndex + 1;
        public bool IsTransitioning => isTransitioning;
        
        public int CurrentScore => currentScore;
        public int CurrentTargetScore => GetCurrentLevelScrewsNeed();

        public float CurrentGameSpeed
        {
            get
            {
                float speed = 5f;

                if (backgroundLooper != null)
                {
                    speed = backgroundLooper.CurrentSpeed;
                }

                if (isTransitioning || isGameCompleted)
                {
                    return 0f;
                }

                if (isWorldKnockback)
                {
                    return -speed * worldKnockbackSpeedMultiplier;
                }

                return speed;
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
            int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
            currentLevelIndex = Mathf.Clamp(selectedLevel - 1, 0, levels.Length - 1);
            currentScore = 0;

            if (backgroundLooper != null)
            {
                backgroundLooper.SetLevelBackground(currentLevelIndex);
            }

            UpdateUI();
        }

        public void AddScore(int amount)
        {
            if (isTransitioning || isGameCompleted)
                return;

            if (amount <= 0)
                return;

            currentScore += amount;

            int screwsNeed = GetCurrentLevelScrewsNeed();

            if (currentScore >= screwsNeed)
            {
                currentScore = screwsNeed;
                UpdateUI();

                StartCoroutine(CompleteCurrentLevelRoutine());
                return;
            }

            UpdateUI();
        }

        private IEnumerator CompleteCurrentLevelRoutine()
        {
            if (isTransitioning)
                yield break;

            isTransitioning = true;

            bool isLastLevel = currentLevelIndex >= levels.Length - 1;

            if (isLastLevel)
            {
                CompleteGame();
                isTransitioning = false;
                yield break;
            }

            int nextLevelIndex = currentLevelIndex + 1;
            int nextLevelNumber = nextLevelIndex + 1;

            if (levelTransitionUI != null)
            {
                yield return StartCoroutine(
                    levelTransitionUI.PlayTransition(
                        nextLevelNumber,
                        () => ApplyNextLevel(nextLevelIndex)
                    )
                );
            }
            else
            {
                ApplyNextLevel(nextLevelIndex);
                yield return new WaitForSeconds(0.5f);
            }

            isTransitioning = false;
        }

        private void ApplyNextLevel(int nextLevelIndex)
        {
            
            currentLevelIndex = nextLevelIndex;
            currentScore = 0;

            if (backgroundLooper != null)
            {
                backgroundLooper.SetLevelBackground(currentLevelIndex);

                float speedIncrease = GetCurrentLevelSpeedIncrease();

                if (speedIncrease > 0f)
                {
                    backgroundLooper.IncreaseSpeed(speedIncrease);
                }
            }

            SaveUnlockedLevel(CurrentLevel);
            UpdateUI();

            Debug.Log("Переход на уровень: " + CurrentLevel);
        }

        private void CompleteGame()
        {
            isGameCompleted = true;

            Debug.Log("Все уровни пройдены");
            
        }

        private int GetCurrentLevelScrewsNeed()
        {
            if (levels == null || levels.Length == 0)
                return 10;

            if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length)
                return 10;

            return levels[currentLevelIndex].screwsToComplete;
        }

        private float GetCurrentLevelSpeedIncrease()
        {
            if (levels == null || levels.Length == 0)
                return 0f;

            if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length)
                return 0f;

            return levels[currentLevelIndex].speedIncreaseOnLevelStart;
        }

        public void StartWorldKnockback()
        {
            if (isTransitioning || isGameCompleted)
                return;

            if (knockbackCoroutine != null)
            {
                StopCoroutine(knockbackCoroutine);
            }

            knockbackCoroutine = StartCoroutine(WorldKnockbackRoutine());
        }

        private IEnumerator WorldKnockbackRoutine()
        {
            isWorldKnockback = true;

            yield return new WaitForSeconds(worldKnockbackDuration);

            isWorldKnockback = false;
            knockbackCoroutine = null;
        }

        private void UpdateUI()
        {
            int screwsNeed = GetCurrentLevelScrewsNeed();

            if (scoreText != null)
            {
                scoreText.text = currentScore + " / " + screwsNeed;
            }

            if (levelText != null)
            {
                levelText.text = "Уровень " + CurrentLevel;
            }
        }
        
        private void SaveUnlockedLevel(int level)
        {
            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

            if (level > unlockedLevel)
            {
                PlayerPrefs.SetInt("UnlockedLevel", level);
                PlayerPrefs.Save();
            }
        }
    }
}