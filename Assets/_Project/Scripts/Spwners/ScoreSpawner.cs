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

        private float spawnTimer;
        private int currentBonusesAlive;

        private void Update()
        {
            if (scorePointPrefab == null)
                return;

            if (currentBonusesAlive >= maxScorePrefab)
                return;

            spawnTimer += Time.deltaTime;

            if (spawnTimer < intervalSpawnGates)
                return;

            spawnTimer = 0f;

            SpawnPrefab();
        }

        private void SpawnPrefab()
        {
            GameObject bonusObject = Instantiate(
                scorePointPrefab,
                transform.position,
                Quaternion.identity
            );

            BonusMb bonusMb = bonusObject.GetComponent<BonusMb>();

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
            {
                bonusMb.Destroyed -= OnBonusDestroyed;
            }

            currentBonusesAlive--;

            if (currentBonusesAlive < 0)
            {
                currentBonusesAlive = 0;
            }
        }
    }
}