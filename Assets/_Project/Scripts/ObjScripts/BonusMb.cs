using System;
using _Project.Scripts.Game;
using UnityEngine;

namespace _Project.Scripts.ObjScripts
{
    public class BonusMb : MonoBehaviour
    {
        public event Action<BonusMb> Destroyed;

        [SerializeField] private int scoreAmount = 1;
        [SerializeField] private float lifeTime = 10f;

        private bool isPickedUp;

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
                Destroy(gameObject);
            }
        }

        public void PickUpBonus()
        {
            if (isPickedUp)
                return;

            isPickedUp = true;

            if (LevelProgressManager.Instance != null)
            {
                LevelProgressManager.Instance.AddScore(scoreAmount);
            }

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}