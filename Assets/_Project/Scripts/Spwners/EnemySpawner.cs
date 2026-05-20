using _Project.Scripts.ObjScripts;
using UnityEngine;

namespace _Project.Scripts.Spawners
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Enemy Prefabs")]
        [SerializeField] private GameObject[] enemyPrefabs;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;

        [Header("Spawn Settings")]
        [SerializeField] private float minSpawnInterval = 1.5f;
        [SerializeField] private float maxSpawnInterval = 3f;
        [SerializeField] private int maxEnemiesAlive = 5;

        [Header("Overlap Check")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float minDistanceBetweenEnemies = 2f;
        [SerializeField] private Vector2 checkPadding = new Vector2(0.3f, 0.3f);
        [SerializeField] private int spawnAttempts = 10;

        private float spawnTimer;
        private float nextSpawnTime;
        private int currentEnemiesAlive;

        private void Start()
        {
            SetNextSpawnTime();
        }

        private void Update()
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0)
                return;

            if (spawnPoints == null || spawnPoints.Length == 0)
                return;

            if (currentEnemiesAlive >= maxEnemiesAlive)
                return;

            spawnTimer += Time.deltaTime;

            if (spawnTimer < nextSpawnTime)
                return;

            spawnTimer = 0f;
            SetNextSpawnTime();

            TrySpawnEnemy();
        }

        private void TrySpawnEnemy()
        {
            for (int i = 0; i < spawnAttempts; i++)
            {
                GameObject enemyPrefab = GetRandomEnemyPrefab();
                Transform spawnPoint = GetRandomSpawnPoint();

                if (enemyPrefab == null || spawnPoint == null)
                    continue;

                Vector2 spawnPosition = spawnPoint.position;

                if (IsSpawnPlaceFree(enemyPrefab, spawnPosition))
                {
                    SpawnEnemy(enemyPrefab, spawnPosition);
                    return;
                }
            }

            Debug.Log("Не удалось найти свободное место для врага");
        }

        private void SpawnEnemy(GameObject enemyPrefab, Vector2 spawnPosition)
        {
            GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            if (enemyObject.TryGetComponent(out EnemyMb enemyMb))
            {
                currentEnemiesAlive++;
                enemyMb.Destroyed += OnEnemyDestroyed;
            }
            else
            {
                Debug.LogWarning("На префабе врага нет EnemyMb");
            }
        }

        private bool IsSpawnPlaceFree(GameObject enemyPrefab, Vector2 spawnPosition)
        {
            Vector2 checkSize = GetPrefabCheckSize(enemyPrefab);

            Collider2D boxHit = Physics2D.OverlapBox(
                spawnPosition,
                checkSize,
                0f,
                enemyLayer
            );

            if (boxHit != null)
                return false;

            Collider2D circleHit = Physics2D.OverlapCircle(
                spawnPosition,
                minDistanceBetweenEnemies,
                enemyLayer
            );

            if (circleHit != null)
                return false;

            return true;
        }

        private Vector2 GetPrefabCheckSize(GameObject prefab)
        {
            Collider2D collider = prefab.GetComponentInChildren<Collider2D>();

            if (collider == null)
            {
                return Vector2.one + checkPadding;
            }

            Vector2 size = Vector2.one;

            if (collider is BoxCollider2D boxCollider)
            {
                size = boxCollider.size;
            }
            else if (collider is CapsuleCollider2D capsuleCollider)
            {
                size = capsuleCollider.size;
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                float diameter = circleCollider.radius * 2f;
                size = new Vector2(diameter, diameter);
            }

            size.x *= Mathf.Abs(collider.transform.lossyScale.x);
            size.y *= Mathf.Abs(collider.transform.lossyScale.y);

            return size + checkPadding;
        }

        private GameObject GetRandomEnemyPrefab()
        {
            int index = Random.Range(0, enemyPrefabs.Length);
            return enemyPrefabs[index];
        }

        private Transform GetRandomSpawnPoint()
        {
            int index = Random.Range(0, spawnPoints.Length);
            return spawnPoints[index];
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

            currentEnemiesAlive--;

            if (currentEnemiesAlive < 0)
            {
                currentEnemiesAlive = 0;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (spawnPoints == null)
                return;

            Gizmos.color = Color.red;

            foreach (Transform point in spawnPoints)
            {
                if (point == null)
                    continue;

                Gizmos.DrawWireSphere(point.position, minDistanceBetweenEnemies);
            }
        }
    }
}