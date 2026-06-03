using _Project.Scripts.Game;
using _Project.Scripts.ObjScripts;
using UnityEngine;

namespace _Project.Scripts.Spawners
{
    public class ScoreSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject scorePointPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private int maxScorePrefab = 3;
        [SerializeField] private float intervalSpawnGates = 1f;

        [Header("Spawn Check")]
        [SerializeField] private LayerMask blockedSpawnLayers;
        [SerializeField] private float checkRadius = 1.5f;

        private float spawnTimer;
        private int currentBonusesAlive;

        private void Update()
        {
            if (LevelProgressManager.Instance != null &&
                LevelProgressManager.Instance.IsTransitioning)
                return;

            if (scorePointPrefab == null)
                return;

            if (currentBonusesAlive >= maxScorePrefab)
                return;

            spawnTimer += Time.deltaTime;

            if (spawnTimer < intervalSpawnGates)
                return;

            spawnTimer = 0f;

            if (IsSpawnBlocked())
                return;

            SpawnPrefab();
        }

        private bool IsSpawnBlocked()
        {
            Collider2D hit = Physics2D.OverlapCircle(
                transform.position,
                checkRadius,
                blockedSpawnLayers
            );

            return hit != null;
        }

        private void SpawnPrefab()
        {
            GameObject bonusObject = Instantiate(
                scorePointPrefab,
                transform.position,
                Quaternion.identity
            );

            BonusMb bonusMb = bonusObject.GetComponent<BonusMb>();

            if (bonusMb == null)
            {
                bonusMb = bonusObject.GetComponentInChildren<BonusMb>();
            }

            if (bonusMb != null)
            {
                currentBonusesAlive++;
                bonusMb.Destroyed += OnBonusDestroyed;
            }
            else
            {
                Debug.LogWarning("На префабе бонуса нет BonusMb");
            }
        }

        private void OnBonusDestroyed(BonusMb bonusMb)
        {
            if (bonusMb != null)
                bonusMb.Destroyed -= OnBonusDestroyed;

            currentBonusesAlive--;

            if (currentBonusesAlive < 0)
                currentBonusesAlive = 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, checkRadius);
        }
    }
}