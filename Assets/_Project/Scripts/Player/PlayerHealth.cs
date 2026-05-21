using System.Collections;
using TMPro;
using _Project.Scripts.Game;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int maxHealth = 3;

        [Header("UI")]
        [SerializeField] private TMP_Text healthText;

        [Header("Damage Protection")]
        [SerializeField] private float invulnerabilityTime = 1f;

        private int currentHealth;
        private bool isInvulnerable;
        private bool isDead;

        public int CurrentHealth => currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
            UpdateUI();
        }

        public bool TakeDamage(int damage)
        {
            if (isDead)
                return false;

            if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
                return false;

            if (isInvulnerable)
                return false;

            currentHealth -= damage;

            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            Debug.Log("Игрок получил урон. HP: " + currentHealth);

            UpdateUI();

            if (currentHealth <= 0)
            {
                Die();
                return true;
            }

            StartCoroutine(InvulnerabilityRoutine());

            return true;
        }

        private IEnumerator InvulnerabilityRoutine()
        {
            isInvulnerable = true;

            yield return new WaitForSeconds(invulnerabilityTime);

            isInvulnerable = false;
        }

        private void UpdateUI()
        {
            if (healthText != null)
            {
                healthText.text = "HP: " + currentHealth + " / " + maxHealth;
            }
        }

        private void Die()
        {
            if (isDead)
                return;

            isDead = true;

            Debug.Log("Игрок погиб");

            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.ShowGameOver();
            }
            else
            {
                Time.timeScale = 0f;
            }
        }
    }
}