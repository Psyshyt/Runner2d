using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 3;

        private int currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            Debug.Log($"Игрок получил урон: {damage}. Осталось здоровья: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Игрок погиб");
            
            // Time.timeScale = 0f;
        }
    }
}