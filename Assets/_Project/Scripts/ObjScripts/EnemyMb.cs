using System;
using _Project.Scripts.Game;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.ObjScripts
{
    public class EnemyMb : MonoBehaviour
    {
        public event Action<EnemyMb> Destroyed;

        [Header("Enemy Settings")]
        [SerializeField] private int damage = 1;
        [SerializeField] private float lifeTime = 10f;

        private bool wasHit;
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
                DestroyEnemy();
            }
        }

        public void HitPlayer(PlayerHealth playerHealth)
        {
            if (wasHit)
                return;

            if (playerHealth == null)
                return;

            bool damageApplied = playerHealth.TakeDamage(damage);

            if (!damageApplied)
                return;

            wasHit = true;

            if (LevelProgressManager.Instance != null)
            {
                LevelProgressManager.Instance.StartWorldKnockback();
            }

            // ВАЖНО:
            // Здесь больше НЕ уничтожаем врага после столкновения.
            // Он продолжит движение и исчезнет только по lifeTime.
        }

        private void DestroyEnemy()
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