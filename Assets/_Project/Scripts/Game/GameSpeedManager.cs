using UnityEngine;

namespace _Project.Scripts.Game
{
    public class GameSpeedManager : MonoBehaviour
    {
        public static GameSpeedManager Instance { get; private set; }

        [SerializeField] private float startSpeed = 5f;
        [SerializeField] private float speedIncreasePerLevel = 1.5f;
        [SerializeField] private float maxSpeed = 15f;

        public float CurrentSpeed { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            CurrentSpeed = startSpeed;
        }

        public void IncreaseSpeed()
        {
            CurrentSpeed += speedIncreasePerLevel;

            if (CurrentSpeed > maxSpeed)
            {
                CurrentSpeed = maxSpeed;
            }

            Debug.Log("Новая скорость игры: " + CurrentSpeed);
        }
    }
}