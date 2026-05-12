using UnityEngine;

public class ScoreSpawner : MonoBehaviour
{
    public GameObject scorePointPrefab;
    private int maxScorePrefab = 3;
    private int currentScorePrefab;

    private float intervalSpawn = 3f;
    private float intervalSpawnTimer;


    void Update()
    {
        if (maxScorePrefab == currentScorePrefab)
            return;
        intervalSpawnTimer += Time.deltaTime;
        if (intervalSpawnTimer <= intervalSpawn)
        {
            intervalSpawnTimer = 0f;
            SpawnPrefab();
            Debug.Log("Спавн");
        }
    }



    void SpawnPrefab()
    {
        Instantiate(scorePointPrefab, transform.position, Quaternion.identity);
        currentScorePrefab++;
    }
}
