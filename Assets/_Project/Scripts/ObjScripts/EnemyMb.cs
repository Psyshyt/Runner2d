using System;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.ObjScripts
{
    public class EnemyMb : MonoBehaviour
    {
        public event Action<EnemyMb> Destroyed;

        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float lifeTime = 10f;
        [SerializeField] private bool destroyAfterHit = true;

        private bool wasHit;

        private void Update()
        {
            Move();
            LifeTimer();
        }

        private void Move()
        {
            transform.position += new Vector3(-moveSpeed * Time.deltaTime, 0f, 0f);
        }

        private void LifeTimer()
        {
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0f)
            {
                Destroy(gameObject);
            }
        }

        public void HitPlayer(PlayerHealth playerHealth)
        {
            if (wasHit)
                return;

            wasHit = true;

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            if (destroyAfterHit)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}