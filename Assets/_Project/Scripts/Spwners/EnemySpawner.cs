using System.Collections.Generic;
using _Project.Scripts.Game;
using _Project.Scripts.ObjScripts;
using UnityEngine;

namespace _Project.Scripts.Spawners
{
    public class EnemySpawner : MonoBehaviour
    {
        [System.Serializable]
        public class EnemySpawnData
        {
            public GameObject enemyPrefab;

            [Min(1)]
            public int unlockLevel = 1;

            [Header("Position")]
            public float spawnYOffset = 0f;
        }

        [Header("Enemy Prefabs")]
        [SerializeField] private EnemySpawnData[] enemies;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;

        [Header("Spawn Settings")]
        [SerializeField] private float minSpawnInterval = 1.5f;
        [SerializeField] private float maxSpawnInterval = 3f;

        [Header("Only One Enemy")]
        [SerializeField] private int maxEnemiesOnScene = 1;

        private readonly List<EnemyMb> activeEnemies = new List<EnemyMb>();

        private float spawnTimer;
        private float nextSpawnTime;

        private void Start()
        {
            SetNextSpawnTime();
        }

        private void Update()
        {
            CleanupDestroyedEnemies();

            if (LevelProgressManager.Instance != null &&
                LevelProgressManager.Instance.IsTransitioning)
                return;

            if (activeEnemies.Count >= maxEnemiesOnScene)
                return;

            if (enemies == null || enemies.Length == 0)
                return;

            if (spawnPoints == null || spawnPoints.Length == 0)
                return;

            spawnTimer += Time.deltaTime;

            if (spawnTimer < nextSpawnTime)
                return;

            spawnTimer = 0f;
            SetNextSpawnTime();

            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            EnemySpawnData enemyData = GetRandomAvailableEnemyData();

            if (enemyData == null || enemyData.enemyPrefab == null)
                return;

            Transform spawnPoint = GetRandomSpawnPoint();

            if (spawnPoint == null)
                return;

            Vector3 spawnPosition = spawnPoint.position;
            spawnPosition.y += enemyData.spawnYOffset;

            GameObject enemyObject = Instantiate(
                enemyData.enemyPrefab,
                spawnPosition,
                Quaternion.identity
            );

            EnemyMb enemyMb = enemyObject.GetComponent<EnemyMb>();

            if (enemyMb == null)
            {
                enemyMb = enemyObject.GetComponentInChildren<EnemyMb>();
            }

            if (enemyMb == null)
            {
                Debug.LogWarning("На префабе врага нет EnemyMb");
                Destroy(enemyObject);
                return;
            }

            activeEnemies.Add(enemyMb);
            enemyMb.Destroyed += OnEnemyDestroyed;

            Debug.Log("Враг создан: " + enemyObject.name + ". Активных врагов: " + activeEnemies.Count);
        }

        private EnemySpawnData GetRandomAvailableEnemyData()
        {
            int currentLevel = 1;

            if (LevelProgressManager.Instance != null)
            {
                currentLevel = LevelProgressManager.Instance.CurrentLevel;
            }

            List<EnemySpawnData> availableEnemies = new List<EnemySpawnData>();

            foreach (EnemySpawnData enemyData in enemies)
            {
                if (enemyData == null)
                    continue;

                if (enemyData.enemyPrefab == null)
                    continue;

                if (currentLevel >= enemyData.unlockLevel)
                {
                    availableEnemies.Add(enemyData);
                }
            }

            if (availableEnemies.Count == 0)
            {
                Debug.LogWarning("Нет доступных врагов для уровня: " + currentLevel);
                return null;
            }

            int randomIndex = Random.Range(0, availableEnemies.Count);
            return availableEnemies[randomIndex];
        }

        private Transform GetRandomSpawnPoint()
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex];
        }

        private void SetNextSpawnTime()
        {
            nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        private void OnEnemyDestroyed(EnemyMb enemyMb)
        {
            if (enemyMb != null)
            {
                enemyMb.Destroyed -= OnEnemyDestroyed;
            }

            activeEnemies.Remove(enemyMb);

            Debug.Log("Враг исчез. Активных врагов: " + activeEnemies.Count);
        }

        private void CleanupDestroyedEnemies()
        {
            activeEnemies.RemoveAll(enemy => enemy == null);
        }
    }
}