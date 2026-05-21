using System;
using _Project.Scripts.Game;
using UnityEngine;

namespace _Project.Scripts.ObjScripts
{
    public class BonusMb : MonoBehaviour
    {
        public event Action<BonusMb> Destroyed;

        [Header("Score")]
        [SerializeField] private int scoreAmount = 1;

        [Header("Lifetime")]
        [SerializeField] private float lifeTime = 10f;

        private bool isPickedUp;
        private bool isDestroyedNotified;

        private void Update()
        {
            Move();
            LifeTimer();
        }

        private void Move()
        {
            float speed = 5f;

            if (LevelProgressManager.Instance != null)
            {
                speed = LevelProgressManager.Instance.CurrentGameSpeed;
            }

            transform.position += new Vector3(-speed * Time.deltaTime, 0f, 0f);
        }

        private void LifeTimer()
        {
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0f)
            {
                Debug.Log("Бонус исчез по лайфтайму");
                DestroyBonus();
            }
        }

        public void PickUpBonus()
        {
            if (isPickedUp)
                return;
            
            if (LevelProgressManager.Instance != null && LevelProgressManager.Instance.IsTransitioning)
                return;
            
            isPickedUp = true;

            if (LevelProgressManager.Instance != null)
            {
                LevelProgressManager.Instance.AddScore(scoreAmount);
            }

            Debug.Log("Бонус подобран игроком");

            DestroyBonus();
        }

        private void DestroyBonus()
        {
            NotifyDestroyed();
            Destroy(gameObject);
        }

        private void NotifyDestroyed()
        {
            if (isDestroyedNotified)
                return;

            isDestroyedNotified = true;
            Destroyed?.Invoke(this);
        }

        private void OnDestroy()
        {
            NotifyDestroyed();
        }
    }
}